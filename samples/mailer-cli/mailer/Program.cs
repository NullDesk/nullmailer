using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.Core;
using Sample.Mailer.Cli.Commands;

namespace Sample.Mailer.Cli
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                args = new[] { "--help" };
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
