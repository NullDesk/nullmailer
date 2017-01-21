
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

        /// <summary>
        /// If provided, specifies the username used to authenticate with the SMTP server 
        /// </summary>
        /// <returns>The username</returns>
        public string UserName { get; set; }

        /// <summary>
        /// If provided, specifies the password used to authenticate with the SMTP server 
        /// </summary>
        /// <returns></returns>
        public string Password { get; set; }

        /// <summary>
        /// If provided, specifies the credentials used to autheticate with the SMTP server.
        /// </summary>
        /// <remarks>
        /// Will be used instead of username and password if provided.
        /// </remarks>
        /// <returns></returns>
        public System.Net.ICredentials Credentials { get; set; }
    }
}
