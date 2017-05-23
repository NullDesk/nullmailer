using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.Core;
using Sample.Mailer.Cli.Configuration;

namespace Sample.Mailer.Cli.Commands
{
    public class SendSimpleMessage : CliCommand
    {
        public SendSimpleMessage(AnsiConsole console, IMailer mailer,
            IOptions<TestMessageSettings> settings) : base(console)
        {
            Mailer = mailer;
            Settings = settings.Value.SimpleMessageSettings;
        }

        private IMailer Mailer { get; }

        private SimpleMessageSettings Settings { get; }

        public override void Configure(CommandLineApplication app)
        {
            app.Command("simple", simpleApp =>
            {
                simpleApp.HelpOption("-?|-h|--help");
                simpleApp.FullName = "Send Simple Message";
                simpleApp.Description =
                    "Attempts to send a simple test message through the currently configured provider, no template is used";
                simpleApp.AllowArgumentSeparator = true;

                var addAttachments = simpleApp.Option("-a|--attachments",
                    "Include attachment files if specified in the messsage settings (see appsettings.json)",
                    CommandOptionType.NoValue);

                simpleApp.OnExecute(async () =>
                {
                    IEnumerable<DeliveryItem> result = null;
                    try
                    {
                        var dItems = Mailer.CreateMessage(b => b
                            .Subject(Settings.Subject)
                            .And.To(Settings.ToAddress)
                            .WithDisplayName(Settings.ToDisplayName)
                            .And.ForBody()
                            .WithHtml(Settings.HtmlBody)
                            .AndPlainText(Settings.TextBody)
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
                    var message = result == null || !result.All(r => r.IsSuccess)
                        ? "Failed to send email".Red()
                        : "Email sent".Cyan();

                    Reporter.WriteLine(message);

                    return 0;
                });
            }, false);
        }
    }
}