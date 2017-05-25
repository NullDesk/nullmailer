// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Settings for oAuth or similar token based SMTP authentication
    /// </summary>
    public class MkSmtpAccessTokenAuthenticationSettings: IAuthenticationSettings
    {
        /// <summary>
        ///     The username used to authenticate with the SMTP server
        /// </summary>
        /// <returns>The username</returns>
        public string UserName { get; set; }

        /// <summary>
        ///     The access token used to authenticate with the SMTP server.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }
    }
}