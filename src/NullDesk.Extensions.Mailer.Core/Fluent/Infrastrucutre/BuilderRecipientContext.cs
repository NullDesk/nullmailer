
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public abstract class BuilderRecipientContext : BuilderContext
    {
        protected MailerRecipient Recipient { get; }

        internal BuilderRecipientContext(MailerMessage context, MailerRecipient recipient) : base(context)
        {
            Recipient = recipient;
        }
    }
}
