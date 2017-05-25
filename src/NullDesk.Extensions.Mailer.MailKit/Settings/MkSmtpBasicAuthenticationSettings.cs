// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Basic authentication settings for MailKit SMTP mailers.
    /// </summary>
    public class MkSmtpBasicAuthenticationSettings : IAuthenticationSettings
    {
        /// <summary>
        ///     If provided, specifies the username used to authenticate with the SMTP server
        /// </summary>
        /// <returns>The username</returns>
        public string UserName { get; set; }

        /// <summary>
        ///     If provided, specifies the password used to authenticate with the SMTP server
        /// </summary>
        /// <returns></returns>
        public string Password { get; set; }

    }
}