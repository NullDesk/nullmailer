// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     The email sender's address and information.
    /// </summary>
    public class MessageSender : IMessageAddress
    {
        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets the display name for the reply address.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
    }
}