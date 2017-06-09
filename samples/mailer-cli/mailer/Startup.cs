using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.History.EntityFramework;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;
using NullDesk.Extensions.Mailer.MailKit;
using NullDesk.Extensions.Mailer.MailKit.Authentication;
using NullDesk.Extensions.Mailer.SendGrid;
using Sample.Mailer.Cli.Commands;
using Sample.Mailer.Cli.Configuration;

// ReSharper disable once CheckNamespace
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
        }


        private void ConfigureLogging(ILoggerFactory loggerFactory)
        {
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

            services.Configure<SqlEntityHistoryStoreSettings>(Config.GetSection("MailHistoryDbSettings"));

            services.AddMailerSqlHistory(s => s.GetService<IOptions<SqlEntityHistoryStoreSettings>>().Value);



            ////alternate way to instantiate history
            //var connectionString = Config.GetValue<string>("MailHistoryDbSettings:ConnectionString");
            //services.AddMailerSqlHistory(settings => settings.ConnectionString = connectionString);

            ////another alternate way to instantiate history
            //var connectionString = Config.GetValue<string>("MailHistoryDbSettings:ConnectionString");
            //services.AddMailerSqlHistory(new SqlEntityHistoryStoreSettings { ConnectionString = connectionString });

            //Configure a mailer 
            var activeMailService = Config.GetSection("MailSettings:ActiveMailService")?.Value.ToLowerInvariant();
            switch (activeMailService)
            {
                case "sendgrid":
                    services.AddSendGridMailer(s => s.GetService<IOptions<SendGridMailerSettings>>().Value);
                    break;
                case "mailkit":
                    services.AddMkSmtpMailer(s => s.GetService<IOptions<MkSmtpMailerSettings>>().Value);
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