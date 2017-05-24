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
using NullDesk.Extensions.Mailer.Core.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    ///     Mail service for delivering messages via the SendGrid API
    /// </summary>
    /// <seealso cref="SendGridMailerSettings" />
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
        /// Delivers the message asynchronous.
        /// </summary>
        /// <param name="deliveryItem">The delivery item.</param>
        /// <param name="autoCloseConnection">If set to <c>true</c> will close connection immediately after delivering the message.
        /// If caller is sending multiple messages, optionally set to false to leave the mail service connection open.</param>
       /// <param name="token">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.String&gt;.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <remarks>The implementor should return a provider specific ID value.</remarks>
        protected override async Task<string> DeliverMessageAsync(
            DeliveryItem deliveryItem,
            bool autoCloseConnection = true, //not used in this mailer
            CancellationToken token = default(CancellationToken))
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


            var isSuccess = sgResponse?.StatusCode == HttpStatusCode.Accepted ||
                            Settings.IsSandboxMode && sgResponse?.StatusCode == HttpStatusCode.OK;

            if (isSuccess)
            {
                return sgResponse.Headers?.GetValues("X-Message-Id").FirstOrDefault();
            }

            throw new Exception(
                $"Unable to deliver message; SendGrid response HTTP StatusCode is: {sgResponse?.StatusCode}");
        }

        /// <summary>
        /// Close mail client connection as an asynchronous operation.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        /// The cancellation token.
        /// <remarks>Used to close connections if DeliverMessageAsync was used with autoCloseConnection set to false.</remarks>
        protected override async Task CloseMailClientConnectionAsync(CancellationToken token = new CancellationToken())
        {
            await Task.CompletedTask;
            //don nothing
        }

        /// <summary>
        ///     Sends the message through the SendGrid API.
        /// </summary>
        /// <param name="message">The message.</param>
       /// <param name="token">The cancellation token.</param>
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
       /// <param name="token">The cancellation token.</param>
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
                        Content = await stream.Value.ToBase64String(),
                        Filename = stream.Key,
                        Disposition = "attachment"
                    });
                }

                mail.AddAttachments(sgAttachments);
            }
        }


        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MailClient = null;
            base.Dispose();
        }
    }
}