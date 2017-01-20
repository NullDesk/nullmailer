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

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    /// SMTP File Template EMail Service.
    /// </summary>
    public class MkSmtpFileTemplateMailer : MkSmtpMailer, ITemplateMailer<FileTemplateMailerSettings>
    {
        /// <summary>
        /// Template settings
        /// </summary>
        /// <value>The template settings.</value>
        public FileTemplateMailerSettings TemplateSettings { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MkSmtpFileTemplateMailer" /> class.
        /// </summary>
        /// <param name="client">The smtp client instance to use for sending messages.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="templateSettings">The template settings.</param>
        /// <remarks>Overload used by unit tests</remarks>
        public MkSmtpFileTemplateMailer(
            SmtpClient client,
            IOptions<SmtpMailerSettings> settings,
            IOptions<FileTemplateMailerSettings> templateSettings)
        : base(client, settings)
        {
            TemplateSettings = templateSettings.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MkSmtpFileTemplateMailer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="templateSettings">The template settings.</param>
        public MkSmtpFileTemplateMailer(
            IOptions<SmtpMailerSettings> settings,
            IOptions<FileTemplateMailerSettings> templateSettings)
        : base(settings)
        {
            TemplateSettings = templateSettings.Value;
        }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <remarks>
        /// The template file will be located using the folder and filename from the supplied service settings. 
        /// </remarks>
        /// <param name="templateName">The template file identifier; should be the filename without extension or file name suffix (specified in settings).</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables. The key should include the delimiters needed to locate text which should be replaced.</param>
        /// <param name="attachmentFiles">The full path for any attachment files to include in the outgoing message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SendMailAsync(
            string templateName,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token)
        {
            var body = await GetBodyForTemplate(templateName, replacementVariables, attachmentFiles, token);
            return await SendMailAsync(toEmailAddress, toDisplayName, subject, body, token);
        }

        /// <summary>
        /// Send mail using a template file.
        /// </summary>
        /// <param name="templateName">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        public async Task<bool> SendMailAsync(
            string templateName, 
            string toEmailAddress, 
            string toDisplayName, 
            string subject,
            IDictionary<string, string> replacementVariables, 
            IDictionary<string, Stream> attachments, 
            CancellationToken token)
        {
            var body = await GetBodyForTemplate(templateName, replacementVariables, attachments, token);
            return await SendMailAsync(toEmailAddress, toDisplayName, subject, body, token);

        }

        private async Task<MimeEntity> GetBodyForTemplate(
            string templateName,
            IDictionary<string, string> replacementVariables, 
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
            return await GetBodyForTemplate(templateName, replacementVariables, attachments, token);
        }


        private async Task<MimeEntity> GetBodyForTemplate(
            string templateName, 
            IDictionary<string, string> replacementVariables, 
            IDictionary<string, Stream> attachments, 
            CancellationToken token)
        {
            var bodyBuilder = new BodyBuilder();
            var templateExists = false;
            var directory = new DirectoryInfo(TemplateSettings.TemplatePath);

            var htmlTemplate = directory.GetFirstFileForExtensions(templateName, TemplateSettings.HtmlTemplateFileExtensions.ToArray());
            if (htmlTemplate != null)
            {
                bodyBuilder.HtmlBody = await htmlTemplate.ToMessageAsync(replacementVariables, token);
                templateExists = true;
            }

            var textTemplate = directory.GetFirstFileForExtensions(templateName, TemplateSettings.TextTemplateFileExtension.ToArray());
            if (textTemplate != null)
            {
                bodyBuilder.TextBody = await textTemplate.ToMessageAsync(replacementVariables, token);
                templateExists = true;
            }

            if (!templateExists)
            {
                throw new ArgumentException($"No email message template found for TemplateId {templateName}");
            }

            AddAttachmentStreams(bodyBuilder, attachments);

            return bodyBuilder.ToMessageBody();
        }

    }
}
