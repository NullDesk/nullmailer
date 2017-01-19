using Microsoft.Extensions.CommandLineUtils;
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
                sendApp.HelpOption("-?|-h|--help");

                sendApp.ConfigureCliCommand<SendSimpleMessage>();

                sendApp.OnExecute(() =>
                {
                    sendApp.ShowHelp();
                    return 0;
                });

            }, false);
        }

        public SendMail(AnsiConsole console) : base(console) { }
    }
}
