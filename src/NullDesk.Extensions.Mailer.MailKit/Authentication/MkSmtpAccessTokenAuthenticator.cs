using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace NullDesk.Extensions.Mailer.MailKit.Authentication
{
    /// <summary>
    ///     For oAuth and similar token based SMTP authentication
    /// </summary>
    public class MkSmtpAccessTokenAuthenticator : IMkSmtpAuthenticator
    {
        /// <summary>
        ///     The username used to authenticate with the SMTP server
        /// </summary>
        /// <returns>The username</returns>
        public string UserName { get; set; }

        /// <summary>
        ///     A func that can obtain a valid access token for authenticating with the SMTP server.
        /// </summary>
        /// <value>The access token.</value>
        public Func<string> AccessTokenFactory { get; set; }

        /// <summary>
        ///     Authenticate with with the supplied client
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task Authenticate(SmtpClient client, CancellationToken token = new CancellationToken())
        {
            await client.AuthenticateAsync(UserName, AccessTokenFactory(), token);
        }
    }
}