
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

                var addAttachments = simpleApp.Option("-a|--attachments",
                    "Include attachment files if specified in the messsage settings (see appsettings.json)",
                    CommandOptionType.NoValue);

                simpleApp.OnExecute(async () =>
                {
                    MessageDeliveryItem result = null;
                    try
                    {
                        if (addAttachments.HasValue())
                        {
                            result = await Mailer
                                .SendMailAsync(
                                    Settings.ToAddress,
                                    Settings.ToDisplayName,
                                    Settings.Subject,
                                    Settings.HtmlBody,
                                    Settings.TextBody,
                                    Settings.AttachmentFiles,
                                    CancellationToken.None);
                        }
                        else
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
                    }
                    catch(Exception ex)
                    { 
                        Reporter.WriteLine($"[Error] {ex.Message}");
                        Reporter.WriteLine(string.Empty);
                    }
                    var message = result == null || !result.IsSuccess  ? "Failed to send email".Red() : "Email sent".Cyan();

                    Reporter.WriteLine(message);

                    return 0;
                });

            }, false);
        }

        private IStandardMailer Mailer { get; }

        private SimpleMessageSettings Settings{get;}

        public SendSimpleMessage(AnsiConsole console, IStandardMailer mailer, IOptions<TestMessageSettings> settings) : base(console)
        {
            Mailer = mailer;
            Settings = settings.Value.SimpleMessageSettings;
        }
    }
}
