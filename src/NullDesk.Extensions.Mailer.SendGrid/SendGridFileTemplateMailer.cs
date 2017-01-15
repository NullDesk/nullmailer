using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.Core;
using Microsoft.Extensions.Options;

namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    /// SendGrid File Template based Email Service.
    /// </summary>
    public class SendGridFileTemplateMailer: SendGridMailer, ITemplateMailer<MailerFileTemplateSettings>
    {
         /// <summary>
        /// Settings for the mailer instance
        /// </summary>
        /// <returns></returns>
        public MailerFileTemplateSettings TemplateSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridMailer"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="templateSettings"></param>
        public SendGridFileTemplateMailer(
            IOptions<SendGridMailerSettings> settings,
            IOptions<MailerFileTemplateSettings> templateSettings
            ) :base(settings)
        {
           TemplateSettings = templateSettings.Value;
        }

        /// <summary>
        /// Send mail using a template.
        /// </summary>
        /// <param name="templateName">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachmentFiles">A collection of paths to attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        public Task<bool> SendMailAsync(
            string templateName, 
            string toEmailAddress, 
            string toDisplayName, 
            string subject, 
            IDictionary<string, string> replacementVariables, 
            IEnumerable<string> attachmentFiles, 
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send mail using a template.
        /// </summary>
        /// <param name="templateName">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        public Task<bool> SendMailAsync(
            string templateName, 
            string toEmailAddress, 
            string toDisplayName, 
            string subject, 
            IDictionary<string, string> replacementVariables, 
            IDictionary<string, Stream> attachments, 
            CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
