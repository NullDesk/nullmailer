namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    /// <summary>
    ///     MailerReplyTo Fluent API.
    /// </summary>
    public static class MessageSenderFluentExtensions
    {
        /// <summary>
        ///     Adds the specified email address to the sender's info.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>MessageSender.</returns>
        public static MessageSender FromAddress(this MessageSender sender, string emailAddress)
        {
            sender.EmailAddress = emailAddress;
            return sender;
        }

        /// <summary>
        ///     Adds a display name to the sender's info.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns>MessageSender.</returns>
        public static MessageSender WithDisplayName(this MessageSender sender, string displayName)
        {
            sender.DisplayName = displayName;
            return sender;
        }
    }
}