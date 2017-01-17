
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.Core;

namespace Sample.Mailer.Cli.Commands
{
    public class SendMail : CliCommand
    {
        public override void Configure(CommandLineApplication app)
        {
            app.Command("send", sendApp =>
            {
                sendApp.HelpOption("-?|-h|--help");
                sendApp.FullName = "Attempts to send a test Email";
                sendApp.Description = "Sends Email";
                sendApp.AllowArgumentSeparator = true;

                sendApp.OnExecute(async () =>
                {
                    var result = false;
                    try
                    {
                        result = await Mailer
                            .SendMailAsync(
                                "noone@nowhere.com",
                                "Mr. Nobody",
                                "Test Mail",
                                null,
                                "This is a test",
                                new List<string>(),
                                CancellationToken.None);
                    }
                    catch(Exception ex)
                    { 
                        Reporter.WriteLine($"[Error] {ex.Message}");
                        Reporter.WriteLine(string.Empty);
                    }
                    var message = result ? "Email sent".Cyan() : "Failed to send email".Red();

                    Reporter.WriteLine(message);

                    return 0;
                });

            }, false);
        }

        private ITemplateMailer Mailer { get; }

        public SendMail(AnsiConsole console, ITemplateMailer mailer) : base(console)
        {
            Mailer = mailer;
        }
    }
}
