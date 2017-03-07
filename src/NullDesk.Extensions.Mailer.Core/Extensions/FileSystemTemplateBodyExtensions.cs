using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class FileSystemTemplateBodyExtensions.
    /// </summary>
    public static class FileSystemTemplateBodyExtensions
    {
        /// <summary>
        ///     Gets a content body from a template body using filesystem templates.
        /// </summary>
        /// <param name="tbody">The tbody.</param>
        /// <param name="message">The message.</param>
        /// <param name="templatePath">The template path.</param>
        /// <param name="htmlTemplateFileExtensions">The HTML template file extensions.</param>
        /// <param name="textTemplateFileExtensions">The text template file extensions.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;MimeEntity&gt;.</returns>
        public static async Task<ContentBody> GetContentBodyFromFileTemplatesAsync(
            this TemplateBody tbody,
            DeliveryItem message,
            string templatePath,
            IEnumerable<string> htmlTemplateFileExtensions,
            IEnumerable<string> textTemplateFileExtensions,
            ILogger logger,
            CancellationToken token = default(CancellationToken))
        {
            var cbody = new ContentBody();
            var templateName = tbody.TemplateName;
            var templateExists = false;
            var directory = new DirectoryInfo(templatePath);

            var htmlTemplate = directory.GetFirstFileForExtensions(templateName, htmlTemplateFileExtensions.ToArray());
            if (htmlTemplate != null)
            {
                cbody.HtmlContent = await htmlTemplate.OpenText().ReadToEndAsync();
                templateExists = true;
            }

            var textTemplate = directory.GetFirstFileForExtensions(templateName, textTemplateFileExtensions.ToArray());
            if (textTemplate != null)
            {
                cbody.PlainTextContent = await textTemplate.OpenText().ReadToEndAsync();
                templateExists = true;
            }

            if (!templateExists)
            {
                var ex = new FileNotFoundException($"No email message template found for TemplateName {templateName}");
                logger.LogError(1, ex, ex.Message);
                throw ex;
            }

            return cbody;
        }
    }
}