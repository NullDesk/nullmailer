
using System;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.Core;
using Sample.Mailer.Cli.Configuration;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.SendGrid;

namespace Sample.Mailer.Cli.Commands
{
    public class SendTemplateMessage : CliCommand
    {
        public override void Configure(CommandLineApplication app)
        {
            app.Command("template", templateApp =>
            {
                templateApp.HelpOption("-?|-h|--help");
                templateApp.FullName = "Send Template Message";
                templateApp.Description = "Attempts to send a template based test message through the currently configured provider";
                templateApp.AllowArgumentSeparator = true;

                templateApp.OnExecute(async () =>
                {
                    var result = false;
                    try
                    {
                        result = await Mailer
                            .SendMailAsync(
                                Settings.Template,
                                Settings.ToAddress,
                                Settings.ToDisplayName,
                                Settings.Subject,
                                Settings.ReplacementVariables,
                                CancellationToken.None);
                    }
                    catch (Exception ex)
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

        private TemplateMessageSettings Settings { get; }

        public SendTemplateMessage(AnsiConsole console, ITemplateMailer mailer, IOptions<TestMessageSettings> settings) : base(console)
        {
            Mailer = mailer;
            if (mailer is SendGridMailer)
            {
                Settings = settings.Value.SendGridTemplateMessage;
            }
            else
            {
                Settings = settings.Value.MailKitTemplateMessage;
            }
        }
    }
}
