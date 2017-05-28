// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     The email sender's address and information.
    /// </summary>
    public class MessageSender : IMessageAddress
    {
        /// <summary>
        ///     The message reply to address.
        /// </summary>
        /// <value>The reply to address.</value>
        public string ReplyToEmailAddress { get; set; }

        /// <summary>
        ///     The display name for the reply to address.
        /// </summary>
        /// <value>The reply to display name.</value>
        public string ReplyToDisplayName { get; set; }

        /// <summary>
        ///     The email address for the message sender.
        /// </summary>
        /// <value>The sender's email address.</value>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     The display name for the sender .
        /// </summary>
        /// <value>The sender's display name.</value>
        public string DisplayName { get; set; }
    }
}