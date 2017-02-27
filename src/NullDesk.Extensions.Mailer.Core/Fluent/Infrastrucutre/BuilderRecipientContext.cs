
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    public abstract class BuilderRecipientContext : BuilderContext
    {
        protected MessageRecipient Recipient { get; }

        internal BuilderRecipientContext(MailerMessage context, MessageRecipient recipient) : base(context)
        {
            Recipient = recipient;
        }
    }
}
