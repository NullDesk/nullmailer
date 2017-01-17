
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Settings for SMTP Email Services.
    /// </summary>
    public class SmtpMailerSettings : IMailerSettings
    {
        /// <summary>
        /// From email address.
        /// </summary>
        /// <value>From email.</value>
        public string FromEmailAddress { get; set; }

        /// <summary>
        /// From display name.
        /// </summary>
        /// <value>From display name.</value>
        public string FromDisplayName { get; set; }

        /// <summary>
        /// The SMTP server host name.
        /// </summary>
        /// <value>The SMTP server.</value>
        public string SmtpServer { get; set; }

        /// <summary>
        /// SMTP server port number.
        /// </summary>
        /// <value>The SMTP port.</value>
        public int SmtpPort { get; set; } = 25;

        /// <summary>
        /// Should SMTP use an SSL connections.
        /// </summary>
        /// <value><c>true</c> if using SSL; otherwise, <c>false</c>.</value>
        public bool SmtpUseSsl { get; set; } = false;
    }
}
