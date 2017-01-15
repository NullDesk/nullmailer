using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    /// SMTP EMail Service.
    /// </summary>
    public class MailKitSmtpMailer : IMailer<SmtpMailerSettings>, IDisposable
    {
        private SmtpClient MailClient { get; }

        /// <summary>
        /// Settings for the mailer instance
        /// </summary>
        /// <returns></returns>
        public SmtpMailerSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitSmtpMailer"/> class.
        /// </summary>
        /// <remarks>
        /// Overload used by unit tests
        /// </remarks>
        /// <param name="client">The smtp client instance to use for sending messages.</param>
        /// <param name="settings">The settings.</param>
        public MailKitSmtpMailer(SmtpClient client, IOptions<SmtpMailerSettings> settings) : this(settings)
        {
            MailClient = client;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitSmtpMailer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MailKitSmtpMailer(IOptions<SmtpMailerSettings> settings)
        {
            Settings = settings.Value;
            if (MailClient == null)
            {
                MailClient = new SmtpClient();
            }
        }

        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachmentFiles">A collection of paths to attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
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
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IDictionary<string, Stream> attachments,
            CancellationToken token)
        {
            var bodyBuilder = new BodyBuilder()
            {
                TextBody = textBody,
                HtmlBody = htmlBody
            };
            AddAttachmentStreams(bodyBuilder, attachments);

            return await SendMailAsync(toEmailAddress, toDisplayName, subject, bodyBuilder.ToMessageBody(), token);

        }


        /// <summary>
        /// send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="FormatException"></exception>
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            MimeEntity body,
            CancellationToken token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Settings.FromDisplayName, Settings.FromEmail));

            try
            {
                var box = string.IsNullOrEmpty(toDisplayName)
                    ? new MailboxAddress(toEmailAddress)
                    : new MailboxAddress(toDisplayName, toEmailAddress);
                message.To.Add(box);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Invalid Email Address: {toDisplayName} <{toEmailAddress}>. Subject: {subject}.", ex);
            }

            message.Subject = subject;
            message.Priority = MessagePriority.Normal;
            message.Body = body;

            await MailClient.ConnectAsync(Settings.SmtpServer, Settings.SmtpPort, Settings.SmtpUseSsl, token);
            await MailClient.SendAsync(message, token);
            await MailClient.DisconnectAsync(true, token);

            return true;
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MailClient.Dispose();
        }


        /// <summary>
        /// Adds the attachment streams.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="attachments">The attachments.</param>
        protected virtual void AddAttachmentStreams(BodyBuilder builder, IDictionary<string, Stream> attachments)
        {
            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    builder.Attachments.Add(attachment.Key, attachment.Value);
                }
            }
        }
    }
}
