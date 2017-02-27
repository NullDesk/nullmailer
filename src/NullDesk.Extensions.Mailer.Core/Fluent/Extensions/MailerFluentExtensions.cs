
namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    /// <summary>
    /// Mailer Fluent API.
    /// </summary>
    public static class MailerFluentExtensions
    {
        /// <summary>
        /// Creates a message and adds it to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="mailer">The mailer instance.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage CreateMessage(this IMailer mailer)
        {
            var message = MailerMessage.Create();
            return mailer.AddMessage(message);
            
        }

        /// <summary>
        /// Creates a message sender.
        /// </summary>
        /// <param name="mailer">The mailer.</param>
        /// <returns>MailerReplyTo.</returns>
        public static MessageSender CreateSender(this IMailer mailer)
        {
            return new MessageSender();
        }

        /// <summary>
        /// Creates a message recipient.
        /// </summary>
        /// <param name="mailer">The mailer.</param>
        /// <returns>MailerRecipient.</returns>
        public static MessageRecipient CreateRecipient(this IMailer mailer)
        {
            return new MessageRecipient();
        }
    }
}
