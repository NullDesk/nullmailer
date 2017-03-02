using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NullDesk.Extensions.Mailer.Core;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    /// Standard message and template SMTP mail service using MailKit.
    /// </summary>
    public class MkSmtpMailer : MkSimpleSmtpMailer, IStandardMailer<MkSmtpMailerSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <remarks>Overload used by unit tests</remarks>
        /// <param name="client">The smtp client instance to use for sending messages.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public MkSmtpMailer(
            SmtpClient client,
            IOptions<MkSmtpMailerSettings> settings,
            ILogger<MkSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
        : base(client, settings, logger, historyStore) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public MkSmtpMailer(
            IOptions<MkSmtpMailerSettings> settings,
            ILogger<MkSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
        : base(settings, logger, historyStore) { }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <param name="template">The template file identifier; should be the filename without extension or file name suffix (specified in settings).</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
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
        /// Send mail using a template file.
        /// </summary>
        /// <param name="template">The template file identifier; should be the filename without extension or file name suffix (specified in settings).</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables. The key should include the delimiters needed to locate text which should be replaced.</param>
        /// <param name="attachmentFiles">The full path for any attachment files to include in the outgoing message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>The template file will be located using the folder and filename from the supplied service settings.</remarks>
        public virtual async Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token = default(CancellationToken))
        {
            var body = await GetBodyForTemplateAsync(template, replacementVariables, attachmentFiles, token);
            subject = subject.TemplateReplace(replacementVariables);
            return await SendMailAsync(toEmailAddress, toDisplayName, subject, body, token);
        }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <param name="template">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
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
            var body = await GetBodyForTemplateAsync(template, replacementVariables, attachments, token);
            subject = subject.TemplateReplace(replacementVariables);
            return await SendMailAsync(toEmailAddress, toDisplayName, subject, body, token);

        }

        /// <summary>
        /// Gets the body for template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="replacementVariables">The replacement variables.</param>
        /// <param name="attachmentFiles">The attachment files.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;MimeEntity&gt;.</returns>
        protected virtual async Task<MimeEntity> GetBodyForTemplateAsync(
            string template,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token = default(CancellationToken))
        {
            var attachments = attachmentFiles.GetAttachmentStreamsForFile();

            return await GetBodyForTemplateAsync(template, replacementVariables, attachments, token);
        }


        /// <summary>
        /// Gets the body for template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="replacementVariables">The replacement variables.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;MimeEntity&gt;.</returns>
        protected virtual async Task<MimeEntity> GetBodyForTemplateAsync(
            string template,
            IDictionary<string, string> replacementVariables,
            IDictionary<string, Stream> attachments,
            CancellationToken token = default(CancellationToken))
        {
            var bodyBuilder = new BodyBuilder();
            var templateExists = false;
            var directory = new DirectoryInfo(Settings.TemplateSettings.TemplatePath);

            var htmlTemplate = directory.GetFirstFileForExtensions(template, Settings.TemplateSettings.HtmlTemplateFileExtensions.ToArray());
            if (htmlTemplate != null)
            {
                bodyBuilder.HtmlBody = await htmlTemplate.ToMessageAsync(replacementVariables, token);
                templateExists = true;
            }

            var textTemplate = directory.GetFirstFileForExtensions(template, Settings.TemplateSettings.TextTemplateFileExtension.ToArray());
            if (textTemplate != null)
            {
                bodyBuilder.TextBody = await textTemplate.ToMessageAsync(replacementVariables, token);
                templateExists = true;
            }

            if (!templateExists)
            {
                var ex = new FileNotFoundException($"No email message template found for TemplateId {template}");
                Logger.LogError(1, ex, ex.Message);
                throw ex;
            }

            AddAttachmentStreams(bodyBuilder, attachments);

            return bodyBuilder.ToMessageBody();
        }

    }
}
