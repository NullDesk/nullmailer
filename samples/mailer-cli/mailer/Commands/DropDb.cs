using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using NullDesk.Cli;
using NullDesk.Extensions.Mailer.History.EntityFramework;

namespace Sample.Mailer.Cli.Commands
{
    public class DropDb : CliCommand
    {
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
                    return result ? 0 : 1;
                });

            }, false);
        }

        private HistoryContext Context { get; }

        public DropDb(AnsiConsole console, HistoryContext context) : base(console)
        {
            Context = context;
        }
    }
}
