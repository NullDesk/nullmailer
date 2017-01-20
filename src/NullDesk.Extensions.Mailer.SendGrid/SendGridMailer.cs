using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Gets or sets the mail client.
        /// </summary>
        /// <value>The mail client.</value>
        public Client MailClient { get; set; }

        /// <summary>
        /// Settings for the mailer instance
        /// </summary>
        /// <returns></returns>
        public SendGridMailerSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridMailer"/> class.
        /// </summary>
        /// <remarks>
        /// Overload used by unit tests
        /// </remarks>
        /// <param name="client">The SendGrid client instance</param>
        /// <param name="settings">The settings.</param>
        public SendGridMailer(Client client, IOptions<SendGridMailerSettings> settings)
        {
            Settings = settings.Value;
            MailClient = client;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridMailer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SendGridMailer(IOptions<SendGridMailerSettings> settings) 
            : this(new Client(settings.Value.ApiKey), settings) { }

        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            CancellationToken token)
        {
            return await SendMailAsync(
                toEmailAddress,
                toDisplayName,
                subject,
                htmlBody,
                textBody,
                new List<string>(),
                token);
        }

        /// <summary>
        /// /// Send mail as an asynchronous operation.
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
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<string> attachmentFiles,
            CancellationToken token)
        {
            IDictionary<string, Stream> attachments = null;
            if (attachmentFiles != null)
            {
                attachments = new Dictionary<string, Stream>();
                foreach (var attachmentFile in attachmentFiles)
                {
                    if (!File.Exists(attachmentFile))
                    {
                        throw new ArgumentException($"Unable to find email attachment with file name: {attachmentFile}");
                    }
                    var f = new FileInfo(attachmentFile);
                    attachments.Add(f.Name, f.OpenRead());
                }
            }
            return await SendMailAsync(toEmailAddress, toDisplayName, subject, htmlBody, textBody, attachments, token);
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
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IDictionary<string, Stream> attachments,
            CancellationToken token)
        {
            var from = new Email(Settings.FromEmailAddress, Settings.FromDisplayName);
            var to = new Email(toEmailAddress, toDisplayName);
            var textContent = new Content("text/plain", textBody);
            var htmlContent = new Content("text/html", htmlBody);

            var mail = new Mail(from, subject, to, textContent);
            mail.AddContent(htmlContent);

            await AddAttachmentStreamsAsync(mail, attachments, token);

            return await SendMailAsync(mail, token);
        }


        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="mail">The mail object to sendx</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual async Task<bool> SendMailAsync(
            Mail mail,
            CancellationToken token)
        {
            mail.MailSettings = new MailSettings()
            {
                SandboxMode = new SandboxMode()
                {
                    Enable = Settings.IsSandboxMode
                }
            };
            var message = mail.Get();
            var response = await MailClient.RequestAsync(
                Client.Methods.POST,
                message,
                urlPath: "mail/send");
            //TODO: Log status                                                          
            return response.StatusCode == HttpStatusCode.Accepted || (Settings.IsSandboxMode && response.StatusCode == HttpStatusCode.OK);

        }

        /// <summary>
        /// Adds the attachment streams.
        /// </summary>
        /// <param name="mail">The mail object to which the attachments should be added.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        protected virtual async Task AddAttachmentStreamsAsync(Mail mail, IDictionary<string, Stream> attachments, CancellationToken token)
        {
            if (attachments != null && attachments.Any())
            {
                foreach (var stream in attachments)
                {
                    var attachment = new Attachment
                    {
                        Content = await StreamToBase64Async(stream.Value, token),
                        Filename = stream.Key,
                        Disposition = "attachment"
                    };
                    mail.AddAttachment(attachment);
                }
            }
        }

        /// <summary>
        /// Converts a Stream to a base 64 encoded string
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <remarks>Will read and dispose the stream</remarks>
        protected async Task<string> StreamToBase64Async(Stream input, CancellationToken token)
        {
            MemoryStream ms;
            if (input is MemoryStream)
            {
                ms = (MemoryStream)input;
            }
            else
            {
                ms = new MemoryStream();
                await input.CopyToAsync(ms, 81920, token);
            }
            using (ms)
            {
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
