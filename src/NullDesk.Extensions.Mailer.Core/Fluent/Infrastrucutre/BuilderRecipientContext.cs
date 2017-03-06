// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core.Fluent
{
    /// <summary>
    ///     Base fluent context for the message recipient builder.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.Fluent.BuilderContext" />
    public abstract class BuilderRecipientContext : BuilderContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuilderRecipientContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="recipient">The recipient.</param>
        internal BuilderRecipientContext(MailerMessage context, MessageRecipient recipient) : base(context)
        {
            Recipient = recipient;
        }

        /// <summary>
        ///     The recipient.
        /// </summary>
        /// <value>The recipient.</value>
        protected MessageRecipient Recipient { get; }
    }
}