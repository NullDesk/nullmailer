using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;
using NullDesk.Extensions.Mailer.MailKit;
using NullDesk.Extensions.Mailer.SendGrid;
using Sample.Mailer.Cli.Commands;
using Sample.Mailer.Cli.Configuration;


namespace Sample.Mailer.Cli
{
    public class Startup
    {

        private static IConfigurationRoot Config { get; set; }

        public void Run(string[] args)
        {
            Config = AcquireConfiguration();

            var services = ConfigureConsoleServices(new ServiceCollection());
            Program.ServiceProvider = services; 
            ConfigureLogging(services.GetService<ILoggerFactory>());

            var historySettings = services.GetService<IOptions<MailHistoryDbSettings>>();
            if (historySettings.Value.EnableHistory)
            {
                ConfigureHistory(services.GetService<HistoryContext>(), services.GetService<ILogger<Startup>>());
            }
        }

        private void ConfigureHistory(HistoryContext context, ILogger<Startup> logger)
        {
            logger.LogInformation("History database initializing");
            context.Database.Migrate();
            logger.LogInformation("History database initialized");
        }

        private void ConfigureLogging(ILoggerFactory loggerFactory){
            
            loggerFactory.AddConsole(Config.GetSection("Logging"));

            loggerFactory.CreateLogger("startup").LogInformation("Application logging configured");
        }

        private IConfigurationRoot AcquireConfiguration()
        {
            var hasUserSettings = File.Exists($"{Directory.GetCurrentDirectory()}\\appsettings-user.json");

            var appsettingsFileName =
                hasUserSettings
                    ? "appsettings-user.json"
                    : "appsettings.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appsettingsFileName, false, true);

            return builder.Build();
        }

        private IServiceProvider ConfigureConsoleServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();

            //register config options
            services.Configure<MailHistoryDbSettings>(Config.GetSection("MailHistoryDbSettings"));
            services.Configure<TestMessageSettings>(Config.GetSection("TestMessageSettings"));
            services.Configure<MkSmtpMailerSettings>(Config.GetSection("MailSettings:MkSmtpMailerSettings"));
            services.Configure<SendGridMailerSettings>(Config.GetSection("MailSettings:SendGridMailerSettings"));

            var historyDbEnabled = Config.GetValue<bool>("MailHistoryDbSettings:EnableHistory");
            if (historyDbEnabled)
            {
                //setup message delivery history for sql server
                services.AddSingleton<DbContextOptions>(s =>
                {
                    var options = s.GetService<IOptions<MailHistoryDbSettings>>();
                    var builder = new DbContextOptionsBuilder<SqlHistoryContext>()
                        .UseSqlServer(options.Value.ConnectionString);
                    return builder.Options;
                });
                services.AddSingleton<IHistoryStore, EntityHistoryStore<SqlHistoryContext>>();

                //mail history doesn't need this, but our DB cleanup commands do
                services.AddTransient<HistoryContext, SqlHistoryContext>();
            }


          

            //Configure a mailer 
            var activeMailService = Config.GetSection("MailSettings:ActiveMailService")?.Value.ToLowerInvariant();
            switch (activeMailService)
            {
                case "sendgrid":
                    services.AddMailer<SendGridMailer>();
                    break;
                case "mailkit":
                    services.AddMailer<MkSmtpMailer>();
                    break;
            }

            //snazzy
            services.AddSingleton(provider => AnsiConsole.GetOutput(true));

            //add the cli commands
            services.AddTransient<SendMail>();
            services.AddTransient<SendSimpleMessage>();
            services.AddTransient<SendTemplateMessage>();
            services.AddTransient<DropDb>();

            return services.BuildServiceProvider();
        }
    }
}
