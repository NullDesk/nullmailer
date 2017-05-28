// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class NullMailerSettings.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IMailerSettings" />
    public class NullMailerSettings : IMailerSettings
    {
        /// <summary>
        ///     Dummy From Email Address
        /// </summary>
        /// <value>From email address.</value>
        public string FromEmailAddress { get; set; }

        /// <summary>
        ///     Dummy From display name.
        /// </summary>
        /// <value>From display name.</value>
        public string FromDisplayName { get; set; }

        /// <summary>
        ///     Reply to email address.
        /// </summary>
        /// <value>The reply to email address.</value>
        public string ReplyToEmailAddress { get; set; }

        /// <summary>
        ///     Reply to display name.
        /// </summary>
        /// <value>The display name of the reply to address.</value>
        public string ReplyToDisplayName { get; set; }
    }
}