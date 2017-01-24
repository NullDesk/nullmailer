using System;
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
    public class MkSmtpMailer : MkSmtpSimpleMailer, ITemplateMailer<FileTemplateMailerSettings>
    {
        /// <summary>
        /// Template settings
        /// </summary>
        /// <value>The template settings.</value>
        public FileTemplateMailerSettings TemplateSettings { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <remarks>Overload used by unit tests</remarks>
        /// <param name="client">The smtp client instance to use for sending messages.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="templateSettings">The template settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        public MkSmtpMailer(
            SmtpClient client,
            IOptions<MkSmtpMailerSettings> settings,
            IOptions<FileTemplateMailerSettings> templateSettings,
            ILogger<MkSmtpMailer> logger = null)
        : base(client, settings, logger)
        {
            TemplateSettings = templateSettings.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="templateSettings">The template settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        public MkSmtpMailer(
            IOptions<MkSmtpMailerSettings> settings,
            IOptions<FileTemplateMailerSettings> templateSettings,
            ILogger<MkSmtpMailer> logger = null)
        : base(settings, logger)
        {
            TemplateSettings = templateSettings.Value;
        }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <remarks>
        /// The template file will be located using the folder and filename from the supplied service settings. 
        /// </remarks>
        /// <param name="template">The template file identifier; should be the filename without extension or file name suffix (specified in settings).</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables. The key should include the delimiters needed to locate text which should be replaced.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            CancellationToken token)
        {
            return await SendMailAsync(
                template,
                toEmailAddress,
                toDisplayName,
                subject,
                replacementVariables,
                new List<string>() { },
                token
            );
        }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <remarks>
        /// The template file will be located using the folder and filename from the supplied service settings. 
        /// </remarks>
        /// <param name="template">The template file identifier; should be the filename without extension or file name suffix (specified in settings).</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables. The key should include the delimiters needed to locate text which should be replaced.</param>
        /// <param name="attachmentFiles">The full path for any attachment files to include in the outgoing message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token)
        {
            var body = await GetBodyForTemplate(template, replacementVariables, attachmentFiles, token);
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
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        public async Task<bool> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IDictionary<string, Stream> attachments,
            CancellationToken token)
        {
            var body = await GetBodyForTemplate(template, replacementVariables, attachments, token);
            subject = subject.TemplateReplace(replacementVariables);
            return await SendMailAsync(toEmailAddress, toDisplayName, subject, body, token);

        }

        private async Task<MimeEntity> GetBodyForTemplate(
            string template,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token)
        {
            var attachments = attachmentFiles.GetStreamsForFileNames(Logger);
      
            return await GetBodyForTemplate(template, replacementVariables, attachments, token);
        }


        private async Task<MimeEntity> GetBodyForTemplate(
            string template,
            IDictionary<string, string> replacementVariables,
            IDictionary<string, Stream> attachments,
            CancellationToken token)
        {
            var bodyBuilder = new BodyBuilder();
            var templateExists = false;
            var directory = new DirectoryInfo(TemplateSettings.TemplatePath);

            var htmlTemplate = directory.GetFirstFileForExtensions(template, TemplateSettings.HtmlTemplateFileExtensions.ToArray());
            if (htmlTemplate != null)
            {
                bodyBuilder.HtmlBody = await htmlTemplate.ToMessageAsync(replacementVariables, token);
                templateExists = true;
            }

            var textTemplate = directory.GetFirstFileForExtensions(template, TemplateSettings.TextTemplateFileExtension.ToArray());
            if (textTemplate != null)
            {
                bodyBuilder.TextBody = await textTemplate.ToMessageAsync(replacementVariables, token);
                templateExists = true;
            }

            if (!templateExists)
            {
                var ex = new ArgumentException($"No email message template found for TemplateId {template}");
                Logger.LogError(1,ex, ex.Message);
                throw ex;
            }

            AddAttachmentStreams(bodyBuilder, attachments);

            return bodyBuilder.ToMessageBody();
        }

    }
}
