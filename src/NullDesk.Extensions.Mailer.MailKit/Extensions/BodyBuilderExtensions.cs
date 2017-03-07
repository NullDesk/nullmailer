using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MimeKit;
using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Class BodyBuilderExtensions.
    /// </summary>
    public static class BodyBuilderExtensions
    {
        /// <summary>
        ///     Adds the attachment streams.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="attachments">The attachments.</param>
        public static BodyBuilder AddMkAttachmentStreams(this BodyBuilder builder,
            IDictionary<string, Stream> attachments)
        {
            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    builder.Attachments.Add(attachment.Key, attachment.Value);
                }
            }
            return builder;
        }

        /// <summary>
        ///     Gets the body for the message.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="message">The message.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="token">The token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;MimeKit.BodyBuilder&gt;.</returns>
        public static async Task<BodyBuilder> GetMkBody(
            this BodyBuilder builder,
            DeliveryItem message,
            MkSmtpMailerSettings settings,
            ILogger logger,
            CancellationToken token = default(CancellationToken))
        {
            ContentBody cbody;

            if (message?.Body is TemplateBody body)
            {
                cbody = await body.GetContentBodyFromFileTemplatesAsync(
                    message,
                    settings.TemplateSettings.TemplatePath,
                    settings.TemplateSettings.HtmlTemplateFileExtensions,
                    settings.TemplateSettings.TextTemplateFileExtension,
                    logger,
                    token);
            }
            else
            {
                cbody = message?.Body as ContentBody;
            }

            return builder.GetMimeEntityForContentBody(message, cbody);
        }


        /// <summary>
        ///     Gets the MIME entity the content body.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="message">The message.</param>
        /// <param name="cbody">The cbody.</param>
        /// <returns>BodyBuilder.</returns>
        public static BodyBuilder GetMimeEntityForContentBody(this BodyBuilder builder, DeliveryItem message,
            ContentBody cbody)
        {
            if (cbody != null)
            {
                builder.TextBody = message.ProcessSubstitutions(cbody.PlainTextContent);
                builder.HtmlBody = message.ProcessSubstitutions(cbody.HtmlContent);
            }

            return builder;
        }
    }
}