using System.Net;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     System.Net.Credentials settings for SMTP authentication
    /// </summary>
    public class MkSmtpCredentialsAuthenticationSettings: IAuthenticationSettings
    {
        /// <summary>
        ///     If provided, specifies the credentials used to autheticate with the SMTP server.
        /// </summary>
        /// <remarks>
        ///     Will be used instead of username and password if provided.
        /// </remarks>
        /// <returns></returns>
        public ICredentials Credentials { get; set; }
    }
}