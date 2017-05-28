using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    ///     Settings for the SendGrid Mailer.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IMailerSettings" />
    public class SendGridMailerSettings : IMailerSettings
    {
        /// <summary>
        ///     SendGrid API Key
        /// </summary>
        /// <returns></returns>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Indicates if SendGrid should be used in SandBox mode
        /// </summary>
        /// <returns></returns>
        public bool IsSandboxMode { get; set; } = true;

        /// <summary>
        ///     From Email Address
        /// </summary>
        /// <returns></returns>
        public string FromEmailAddress { get; set; }

        /// <summary>
        ///     From display name.
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