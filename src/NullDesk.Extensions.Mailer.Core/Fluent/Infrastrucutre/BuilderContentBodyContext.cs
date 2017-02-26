
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public abstract class BuilderContentBodyContext : BuilderContext
    {
        protected ContentBody Body{ get; }

        internal BuilderContentBodyContext(MailerMessage context, ContentBody body) : base(context)
        {
            Body =body;
        }
    }
}
