
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
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
                    IEnumerable<DeliveryItem> result = null;
                    try
                    {

                        var dItems = Mailer.CreateMessage(b => b
                            .Subject(Settings.Subject)
                            .And.To(Settings.ToAddress).WithDisplayName(Settings.ToDisplayName)
                            .And.ForTemplate(Settings.Template)
                            .And.WithSubstitutions(Settings.ReplacementVariables)
                            .And.WithAttachments(Settings.AttachmentFiles)
                            .Build()
                             );
                        result = await Mailer.SendAllAsync(CancellationToken.None);

                    }
                    catch (Exception ex)
                    {
                        Reporter.WriteLine($"[Error] {ex.Message}");
                        Reporter.WriteLine(string.Empty);
                    }
                    var message = result == null || !result.All(r => r.IsSuccess) ? "Failed to send email".Red() : "Email sent".Cyan();

                    Reporter.WriteLine(message);

                    return 0;
                });

            }, false);
        }

        private IMailer Mailer { get; }

        private TemplateMessageSettings Settings { get; }

        public SendTemplateMessage(AnsiConsole console, IMailer mailer, IOptions<TestMessageSettings> settings) : base(console)
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
