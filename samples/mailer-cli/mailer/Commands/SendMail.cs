using Microsoft.Extensions.CommandLineUtils;

namespace Sample.Mailer.Cli.Commands
{
    public class SendMail : CliCommand
    {
        public SendMail(AnsiConsole console) : base(console)
        {
        }

        public override void Configure(CommandLineApplication app)
        {
            app.Command("send", sendApp =>
            {
                sendApp.HelpOption("-?|-h|--help");
                sendApp.FullName = "Attempts to send a test Email";
                sendApp.Description = "Sends Email";
                sendApp.AllowArgumentSeparator = true;


                sendApp.ConfigureCliCommand<SendSimpleMessage>();
                sendApp.ConfigureCliCommand<SendTemplateMessage>();

                sendApp.OnExecute(() =>
                {
                    sendApp.ShowHelp();
                    return 0;
                });
            }, false);
        }
    }
}