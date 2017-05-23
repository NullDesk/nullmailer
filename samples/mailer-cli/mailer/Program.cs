using System;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Mailer.Cli.Commands;
using Sample.Mailer.Cli.Configuration;

namespace Sample.Mailer.Cli
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                args = new[] {"--help"};
            }
            new Startup().Run(args);

            var reporter = ServiceProvider.GetService<AnsiConsole>();

            reporter.WriteLine(string.Empty);
            var app = new CommandLineApplication(false)
            {
                Name = "mailer",
                FullName = "NullDesk Sample Mailer CLI",
                Description = "Send email using the NullDesk Mailer Extensions.",
                AllowArgumentSeparator = true
            };


            app.HelpOption("-?|-h|--help");

            app.ConfigureCliCommand<SendMail>();

            var historySettings = ServiceProvider.GetService<IOptions<MailHistoryDbSettings>>();
            if (historySettings.Value.EnableHistory)
            {
                app.ConfigureCliCommand<DropDb>();
            }
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });
            try
            {
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}