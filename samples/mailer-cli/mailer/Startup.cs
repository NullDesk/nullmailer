using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
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

            ConfigureConsoleServices(new ServiceCollection());
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

        private void ConfigureConsoleServices(IServiceCollection services)
        {
            services.AddOptions();

          
            services.Configure<SmtpMailerSettings>(Config.GetSection("MailSettings:SmtpMailerSettings"));
            services.Configure<FileTemplateMailerSettings>(Config.GetSection("MailSettings:FileTemplateMailerSettings"));
            services.Configure<SendGridMailerSettings>(Config.GetSection("MailSettings:SendGridMailerSettings"));
            
            services.Configure<TestMessageSettings>(Config.GetSection("TestMessageSettings"));

            var activeService = Config.GetSection("MailSettings:ActiveMailService").Value;

            //add both template mailer types 
            services.AddTransient<SendGridTemplateMailer>();
            services.AddTransient<MkSmtpFileTemplateMailer>();

            //check which is the active type based on config setting
            var templateMailerType = activeService.Equals("sendgrid", StringComparison.OrdinalIgnoreCase)
                ? typeof(SendGridTemplateMailer)
                : typeof(MkSmtpFileTemplateMailer);

            //add the actual interface type we'll use when asking for a template mailer
            services.AddTransient(s => (ITemplateMailer)s.GetService(templateMailerType));


            services.AddSingleton(provider => AnsiConsole.GetOutput(true));

            //add the cli commands
            services.AddTransient<SendMail>();
            services.AddTransient<SendSimpleMessage>();

            Program.ServiceProvider = services.BuildServiceProvider();
        }
    }
}
