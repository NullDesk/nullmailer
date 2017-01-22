using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    /// Mailkit SMTP mailer settings.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.SmtpMailerSettings" />
    public class MkSmtpMailerSettings: SmtpMailerSettings
    {
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
