
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

                var addAttachments = templateApp.Option("-a|--attachments",
                 "Include attachment files if specified in the messsage settings (see appsettings.json)",
                 CommandOptionType.NoValue);

                templateApp.OnExecute(async () =>
                {
                    var result = false;
                    try
                    {

                        if (addAttachments.HasValue())
                        {
                            result = await Mailer
                                .SendMailAsync(
                                    Settings.Template,
                                    Settings.ToAddress,
                                    Settings.ToDisplayName,
                                    Settings.Subject,
                                    Settings.ReplacementVariables,
                                    Settings.AttachmentFiles,
                                    CancellationToken.None);
                        }
                        else
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

        private IStandardMailer Mailer { get; }

        private TemplateMessageSettings Settings { get; }

        public SendTemplateMessage(AnsiConsole console, IStandardMailer mailer, IOptions<TestMessageSettings> settings) : base(console)
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
