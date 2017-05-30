using Microsoft.Extensions.CommandLineUtils;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework;
using Sample.Mailer.Cli.History;

namespace Sample.Mailer.Cli.Commands
{
    public class DropDb : CliCommand
    {
        public DropDb(AnsiConsole console, IHistoryStore history) : base(console)
        {

            Context = ((EntityHistoryStore<MailerCliHistoryContext>)history).GetHistoryContext();

        }

        private HistoryContext Context { get; }

        public override void Configure(CommandLineApplication app)
        {
            app.Command("drop-db", sendApp =>
            {
                sendApp.HelpOption("-?|-h|--help");
                sendApp.FullName = "Drop History Database";
                sendApp.Description = "Removes the history database if present";
                sendApp.AllowArgumentSeparator = true;

                sendApp.OnExecute(() =>
                {
                    var result = Context.Database.EnsureDeleted();
                    var message = result ? "Database removed".Cyan() : "Failed to remove database".Red();

                    Reporter.WriteLine(message);
                    Context.Dispose();
                    return result ? 0 : 1;

                });
            }, false);
        }
    }
}