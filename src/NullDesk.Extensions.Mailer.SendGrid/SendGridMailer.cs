using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.Core;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    /// SendGrid Email Service.
    /// </summary>
    public class SendGridMailer : IMailer<SendGridMailerSettings>
    {
        /// <summary>
        /// Settings for the mailer instance
        /// </summary>
        /// <returns></returns>
        public SendGridMailerSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridMailer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SendGridMailer(IOptions<SendGridMailerSettings> settings)
        {
            Settings = settings.Value;
        }


        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IDictionary<string, Stream> attachments,
            CancellationToken token)
        {

            var apiKey = Settings.ApiKey;
            var client = new Client(apiKey);

            var from = new Email(Settings.FromEmailAddress, Settings.FromDisplayName);
            var to = new Email(toEmailAddress, toDisplayName);
            var textContent = new Content("text/plain", textBody);
            var htmlContent = new Content("text/html", htmlBody);

            var mail = new Mail(from, subject, to, textContent);
            mail.AddContent(htmlContent);
            mail.MailSettings.SandboxMode.Enable = Settings.IsSandboxMode;

            var response = await client.RequestAsync(method: Client.Methods.POST,
                                                          requestBody: mail.Get(),
                                                          urlPath: "mail/send");
            //TODO: Log status                                                          
            return response.StatusCode == HttpStatusCode.Accepted;
            
        }

        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachmentFiles">The full path to any attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<string> attachmentFiles,
            CancellationToken token)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
