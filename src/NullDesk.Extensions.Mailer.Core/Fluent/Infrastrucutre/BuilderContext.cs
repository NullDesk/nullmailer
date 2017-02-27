
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public abstract class BuilderContext : IBuilderContext
    {
        MailerMessage IBuilderContext.Message { get; set; }

        internal BuilderContext(MailerMessage context)
        {
            ((IBuilderContext) this).Message = context;
        }

        protected MailerMessage Context => ((IBuilderContext) this).Message;
    }
}
