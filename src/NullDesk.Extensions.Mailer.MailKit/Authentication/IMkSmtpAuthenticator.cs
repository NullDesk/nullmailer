using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace NullDesk.Extensions.Mailer.MailKit.Authentication
{
    /// <summary>
    ///     Interface IMkSmtpAuthenticator
    /// </summary>
    public interface IMkSmtpAuthenticator
    {
        /// <summary>
        ///     Authenticate with with the supplied client
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task Authenticate(SmtpClient client, CancellationToken token = default(CancellationToken));
    }
}