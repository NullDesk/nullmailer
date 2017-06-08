using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace NullDesk.Extensions.Mailer.MailKit.Authentication
{
    /// <summary>
    ///     System.Net.Credentials settings for SMTP authentication
    /// </summary>
    public class MkSmtpCredentialsAuthenticator : IMkSmtpAuthenticator
    {
        /// <summary>
        ///     If provided, specifies the credentials used to autheticate with the SMTP server.
        /// </summary>
        /// <remarks>
        ///     Will be used instead of username and password if provided.
        /// </remarks>
        /// <returns></returns>
        public ICredentials Credentials { get; set; }

        /// <summary>
        ///     Authenticate with with the supplied client
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task Authenticate(SmtpClient client, CancellationToken token = new CancellationToken())
        {
            await client.AuthenticateAsync(Credentials, token);
        }
    }
}