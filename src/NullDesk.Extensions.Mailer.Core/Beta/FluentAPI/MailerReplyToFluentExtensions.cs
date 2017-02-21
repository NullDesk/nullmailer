// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// MailerReplyTo Fluent API.
    /// </summary>
    public static class MailerReplyToFluentExtensions
    {
        /// <summary>
        /// Adds the specified email address to the sender's info.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerReplyTo.</returns>
        public static MailerReplyTo FromAddress(this MailerReplyTo sender, string emailAddress)
        {
            sender.EmailAddress = emailAddress;
            return sender;
        }

        /// <summary>
        /// Adds a display name to the sender's info.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerReplyTo.</returns>
        public static MailerReplyTo WithName(this MailerReplyTo sender, string displayName)
        {
            sender.DisplayName = displayName;
            return sender;
        }
    }
}