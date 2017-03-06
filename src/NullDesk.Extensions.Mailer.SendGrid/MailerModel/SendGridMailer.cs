using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;
using SendGrid.Helpers.Mail;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    ///     Mail service for delivering messages via the SendGrid API
    /// </summary>
    /// <seealso cref="SendGridMailerSettings" />
    /// <seealso cref="IHistoryMailer" />
    public class SendGridMailer : Mailer<SendGridMailerSettings>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SendGridMailer" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        public SendGridMailer(
            SendGridClient client,
            IOptions<SendGridMailerSettings> settings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore historyStore = null)
            : base(settings.Value, logger, historyStore)
        {
            MailClient = client;
           
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SendGridMailer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        public SendGridMailer(
            IOptions<SendGridMailerSettings> settings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore historyStore = null)
            : this(new SendGridClient(settings.Value.ApiKey), settings, logger, historyStore)
        {
        }

        /// <summary>
        ///     Gets or sets the mail client.
        /// </summary>
        /// <value>The mail client.</value>
        public SendGridClient MailClient { get; set; }
       

        /// <summary>
        ///     deliver a message as an asynchronous operation.
        /// </summary>
        /// <param name="deliveryItem">The delivery item containing the message you wish to send.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;DeliveryItem&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override async Task<DeliveryItem> DeliverMessageAsync(DeliveryItem deliveryItem,
            CancellationToken token = new CancellationToken())
        {
            var sgFrom = new EmailAddress(deliveryItem.FromEmailAddress, deliveryItem.FromDisplayName);
            var sgTo = new EmailAddress(deliveryItem.ToEmailAddress, deliveryItem.ToDisplayName);
            var sgMessage = new SendGridMessage
            {
                From = sgFrom,
                Personalizations = new List<Personalization>
                {
                    new Personalization
                    {
                        Tos = new List<EmailAddress> {sgTo},
                        Subject = deliveryItem.ProcessSubstitutions(deliveryItem.Subject),
                        Substitutions = new Dictionary<string, string>(deliveryItem.Substitutions)
                    }
                }
            };

            if (deliveryItem.Body is ContentBody body)
            {
                sgMessage.HtmlContent = deliveryItem.ProcessSubstitutions(body.HtmlContent);
                sgMessage.PlainTextContent = deliveryItem.ProcessSubstitutions(body.PlainTextContent);
            }
            else
            {
                sgMessage.SetTemplateId(((TemplateBody) deliveryItem.Body).TemplateName);
            }

            await AddAttachmentStreamsAsync(sgMessage, deliveryItem.Attachments, token);

            var sgResponse = await SendToApiAsync(sgMessage, token);


            var isSuccess = sgResponse.StatusCode == HttpStatusCode.Accepted ||
                            Settings.IsSandboxMode && sgResponse.StatusCode == HttpStatusCode.OK;

            if (isSuccess)
            {
                //TODO: Enhance delivery item to store arbitrary data from mail service provider
                return deliveryItem;
            }

            throw new Exception(
                $"Unable to delivery message; SendGrid response HTTP StatusCode is: {sgResponse.StatusCode}");
        }

        /// <summary>
        ///     Sends the message through the SendGrid API.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;Response&gt;.</returns>
        protected virtual async Task<Response> SendToApiAsync(SendGridMessage message,
            CancellationToken token = default(CancellationToken))
        {
            return await MailClient.SendEmailAsync(message, token);
        }

        /// <summary>
        ///     Adds the attachment streams.
        /// </summary>
        /// <param name="mail">The mail object to which the attachments should be added.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        protected virtual async Task AddAttachmentStreamsAsync(SendGridMessage mail,
            IDictionary<string, Stream> attachments, CancellationToken token = default(CancellationToken))
        {
            if (attachments != null && attachments.Any())
            {
                var sgAttachments = new List<Attachment>();
                foreach (var stream in attachments)
                {
                    sgAttachments.Add(new Attachment
                    {
                        Content = await StreamToBase64Async(stream.Value, token),
                        Filename = stream.Key,
                        Disposition = "attachment"
                    });
                }

                mail.AddAttachments(sgAttachments);
            }
        }

        /// <summary>
        ///     Converts a Stream to a base 64 encoded string
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <remarks>Will read and dispose the stream</remarks>
        protected virtual async Task<string> StreamToBase64Async(Stream input,
            CancellationToken token = default(CancellationToken))
        {
            MemoryStream ms;
            if (input is MemoryStream stream)
            {
                ms = stream;
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

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MailClient = null;
        }
    }
}