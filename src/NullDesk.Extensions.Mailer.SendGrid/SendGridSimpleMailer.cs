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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    /// Simplified email service for SendGrid. 
    /// </summary>
    public class SendGridSimpleMailer : ISimpleMailer<SendGridMailerSettings>, IHistoryMailer
    {

        /// <summary>
        /// Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        public IHistoryStore HistoryStore { get; }

        /// <summary>
        /// Optional logger
        /// </summary>
        /// <returns></returns>
        protected ILogger Logger { get; }


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
        /// Initializes a new instance of the <see cref="SendGridSimpleMailer"/> class.
        /// </summary>
        /// <remarks>
        /// Overload used by unit tests
        /// </remarks>
        /// <param name="client">The SendGrid client instance</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public SendGridSimpleMailer(
            Client client,
            IOptions<SendGridMailerSettings> settings,
            ILogger<SendGridSimpleMailer> logger = null,
            IHistoryStore historyStore = null)
        {
            Settings = settings.Value;
            MailClient = client;
            Logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance as ILogger;
            HistoryStore = historyStore ?? new NullHistoryStore();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridSimpleMailer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public SendGridSimpleMailer(
            IOptions<SendGridMailerSettings> settings,
            ILogger<SendGridSimpleMailer> logger = null,
            IHistoryStore historyStore = null)
        : this(new Client(settings.Value.ApiKey), settings, logger, historyStore) { }

        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
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
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachmentFiles">The full path to any attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<string> attachmentFiles,
            CancellationToken token)
        {
            var attachments = attachmentFiles.GetStreamsForFileNames(Logger);

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
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
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
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
            Mail mail,
            CancellationToken token)
        {
            var historyItem = new MessageDeliveryItem();

            mail.MailSettings = new MailSettings()
            {
                SandboxMode = new SandboxMode()
                {
                    Enable = Settings.IsSandboxMode
                }
            };

            historyItem.DeliveryProvider = GetType().Name;
            historyItem.CreatedDate = DateTimeOffset.Now;
            historyItem.Subject = string.IsNullOrEmpty(mail.Subject)
                ? mail.Personalization.FirstOrDefault()?.Subject
                : mail.Subject;
            historyItem.ToDisplayName = mail.Personalization.FirstOrDefault()?.Tos.FirstOrDefault()?.Name;
            historyItem.ToEmailAddress = mail.Personalization.FirstOrDefault()?.Tos.FirstOrDefault()?.Address;
            
            try
            {
                var message = mail.Get();
                historyItem.MessageData = message;
                var response = await SendToApiAsync(message);
                historyItem.IsSuccess = response.StatusCode == HttpStatusCode.Accepted ||
                                        (Settings.IsSandboxMode && response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                historyItem.ExceptionMessage = ex.Message;
                Logger.LogError(1, ex, ex.Message);
            }
            finally
            {
                await HistoryStore.AddAsync(historyItem, token);
            }
                                                                  
            return historyItem;
        }

        /// <summary>
        /// ReSends the message from the stored history data.
        /// </summary>
        /// <param name="id">The identifier for the message being resent.</param>
        /// <param name="historyData">The history data.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ReSend(Guid id, string historyData, CancellationToken token)
        {
            var isSuccess = false;
            var mail = JsonConvert.DeserializeObject<Mail>(historyData,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Include,
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml
                });
            var subject = string.IsNullOrEmpty(mail.Subject)
                ? mail.Personalization.FirstOrDefault()?.Subject
                : mail.Subject;
            var toDisplay = mail.Personalization.FirstOrDefault()?.Tos.FirstOrDefault()?.Name;
            var to = mail.Personalization.FirstOrDefault()?.Tos.FirstOrDefault()?.Address;
              
            Logger.LogInformation("Attempting to resend message {id} to {toDisplay}<{to}> about {subject}", id, toDisplay, to, subject);

            try
            {
                var response = await SendToApiAsync(historyData);
                isSuccess = response.StatusCode == HttpStatusCode.Accepted ||
                                        (Settings.IsSandboxMode && response.StatusCode == HttpStatusCode.OK); ;
            }
            catch (Exception ex)
            {
                Logger.LogError(1, ex, "Failed resending message {id} with exception {ex.Message}", id, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// Sends the message through the SendGrid API.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;Response&gt;.</returns>
        protected virtual async Task<Response> SendToApiAsync(string message)
        {
            return await MailClient.RequestAsync(
                Client.Methods.POST,
                message,
                urlPath: "mail/send");
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
        protected virtual async Task<string> StreamToBase64Async(Stream input, CancellationToken token)
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
