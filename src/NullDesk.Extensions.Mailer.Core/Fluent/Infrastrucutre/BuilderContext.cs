
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public abstract class BuilderContext
    {
        protected MailerMessage Context { get; }

        internal BuilderContext(MailerMessage context)
        {
            Context = context;
        }
    }
}
