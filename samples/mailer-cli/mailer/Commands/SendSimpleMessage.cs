
using System;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.Core;
using Sample.Mailer.Cli.Configuration;
using Microsoft.Extensions.Options;

namespace Sample.Mailer.Cli.Commands
{
    public class SendSimpleMessage : CliCommand
    {
        public override void Configure(CommandLineApplication app)
        {
            app.Command("simple", simpleApp =>
            {
                simpleApp.HelpOption("-?|-h|--help");
                simpleApp.FullName = "Send Simple Message";
                simpleApp.Description = "Attempts to send a simple test message through the currently configured provider, no template is used";
                simpleApp.AllowArgumentSeparator = true;

                simpleApp.HelpOption("-?|-h|--help");

                simpleApp.OnExecute(async () =>
                {
                    var result = false;
                    try
                    {
                        result = await Mailer
                            .SendMailAsync(
                                Settings.ToAddress,
                                Settings.ToDisplayName,
                                Settings.Subject,
                                Settings.HtmlBody,
                                Settings.TextBody,
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

        private SimpleMessageSettings Settings{get;}

        public SendSimpleMessage(AnsiConsole console, ITemplateMailer mailer, IOptions<TestMessageSettings> settings) : base(console)
        {
            Mailer = mailer;
            Settings = settings.Value.SimpleMessageSettings;
        }
    }
}
