using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.Core;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;

namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    /// Standard message and template mail service using SendGrid.
    /// </summary>
    public class SendGridMailer : SendGridSimpleMailer, IStandardMailer<SendGridMailerSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridMailer" /> class.
        /// </summary>
        /// <remarks>
        /// This overload could be used by unit tests, but the sendgrid client doesn't lend well to testability as of beta 9.0.5.
        /// </remarks>
        /// <param name="client">The SendGrid client instance</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public SendGridMailer(
            SendGridClient client,
            IOptions<SendGridMailerSettings> settings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore historyStore = null) :
        base(client, settings, logger, historyStore)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridMailer"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public SendGridMailer(
            IOptions<SendGridMailerSettings> settings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore historyStore = null) :
        this(new SendGridClient(settings.Value.ApiKey), settings, logger, historyStore)
        { }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <param name="template">The template file identifier; should be the filename without extension or file name suffix (specified in settings).</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject; will be ignored by sendgrid if the template specifies a subject of its own.</param>
        /// <param name="replacementVariables">The replacement variables. The key should include the delimiters needed to locate text which should be replaced.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>The template file will be located using the folder and filename from the supplied service settings.</remarks>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            CancellationToken token = default(CancellationToken))
        {
            return await SendMailAsync(
                template,
                toEmailAddress,
                toDisplayName,
                subject,
                replacementVariables,
                new List<string>(),
                token
            );
        }

        /// <summary>
        /// Send mail using a template.
        /// </summary>
        /// <param name="template">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject; will be ignored by sendgrid if the template specifies a subject of its own.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachmentFiles">A collection of paths to attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token = default(CancellationToken))
        {
            var attachments = attachmentFiles.GetAttachmentStreamsForFile();

            return await SendMailAsync(template, toEmailAddress, toDisplayName, subject, replacementVariables, attachments, token);
        }

        /// <summary>
        /// Send mail using a template.
        /// </summary>
        /// <param name="template">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject; will be ignored by sendgrid if the template specifies a subject of its own.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IDictionary<string, Stream> attachments,
            CancellationToken token = default(CancellationToken))
        {
            var mfrom = new EmailAddress(Settings.FromEmailAddress, Settings.FromDisplayName);
            var mto = new EmailAddress(toEmailAddress, toDisplayName);

            var mail = new SendGridMessage
            {
                From = mfrom,
                TemplateId = template,
                Personalizations = new List<Personalization>()
                {
                    new Personalization
                    {
                        Tos = new List<EmailAddress> {mto},
                        Subject = subject,
                        Substitutions = new Dictionary<string, string>(replacementVariables)
                    }
                }
            };


            await AddAttachmentStreamsAsync(mail, attachments, token);

            return await SendMailAsync(mail, token);
        }
    }
}
