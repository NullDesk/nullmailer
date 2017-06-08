using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace NullDesk.Extensions.Mailer.MailKit.Authentication
{
    /// <summary>
    ///     Basic authentication settings for MailKit SMTP mailers.
    /// </summary>
    public class MkSmtpBasicAuthenticator : IMkSmtpAuthenticator
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


        /// <summary>
        ///     Authenticate SMTP connection with the supplied client
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task Authenticate(SmtpClient client, CancellationToken token)
        {
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(UserName, Password, token);
        }
    }
}