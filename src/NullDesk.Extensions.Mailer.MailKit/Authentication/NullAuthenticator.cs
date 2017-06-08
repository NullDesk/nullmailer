using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace NullDesk.Extensions.Mailer.MailKit.Authentication
{
    /// <summary>
    ///     Empty Authenticator used when no authentication is configured.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.MailKit.Authentication.IMkSmtpAuthenticator" />
    public class NullAuthenticator : IMkSmtpAuthenticator
    {
        /// <summary>
        ///     Authenticate with with the supplied client
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task Authenticate(SmtpClient client, CancellationToken token = new CancellationToken())
        {
            return Task.CompletedTask;
        }
    }
}