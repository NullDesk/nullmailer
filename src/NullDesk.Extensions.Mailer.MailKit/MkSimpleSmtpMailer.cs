﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.Core.History;
using NullDesk.Extensions.Mailer.History;

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    /// Simplified SMTP email service using MailKit.
    /// </summary>
    public class MkSimpleSmtpMailer : ISimpleMailer<MkSmtpMailerSettings>
    {

        /// <summary>
        /// Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        protected IHistoryStore HistoryStore { get; }

        /// <summary>
        /// Optional logger
        /// </summary>
        /// <returns></returns>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the mail client.
        /// </summary>
        /// <value>The mail client.</value>
        public SmtpClient MailClient { get; }

        /// <summary>
        /// Settings for the mailer instance
        /// </summary>
        /// <returns></returns>
        public MkSmtpMailerSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MkSimpleSmtpMailer" /> class.
        /// </summary>
        /// <param name="client">The smtp client instance to use for sending messages.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        /// <remarks>Overload used by unit tests</remarks>
        public MkSimpleSmtpMailer(
            SmtpClient client,
            IOptions<MkSmtpMailerSettings> settings,
            ILogger<MkSimpleSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
        {
            Settings = settings.Value;
            MailClient = client;
            Logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance as ILogger;
            HistoryStore = historyStore ?? new NullHistoryStore();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MkSimpleSmtpMailer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public MkSimpleSmtpMailer(
            IOptions<MkSmtpMailerSettings> settings,
            ILogger<MkSimpleSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
        : this(new SmtpClient(), settings, logger, historyStore) { }

        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual async Task<bool> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            CancellationToken token)
        {
            return await SendMailAsync(
                toEmailAddress,
                toDisplayName,
                subject,
                htmlBody,
                textBody,
                new List<string>(),
                token);
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
            var attachments = attachmentFiles.GetStreamsForFileNames(Logger);

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
            message.From.Add(new MailboxAddress(Settings.FromDisplayName, Settings.FromEmailAddress));

            try
            {
                var box = string.IsNullOrEmpty(toDisplayName)
                    ? new MailboxAddress(toEmailAddress)
                    : new MailboxAddress(toDisplayName, toEmailAddress);
                message.To.Add(box);
            }
            catch (FormatException ex)
            {
                Logger.LogError(1, ex, "Invalid email address format: {toDisplayName} <{toEmailAddress}> Subject: {subject}", toEmailAddress, toDisplayName);
                throw ex;
            }

            message.Subject = subject;
            message.Priority = MessagePriority.Normal;
            message.Body = body;

            var historyItem = new HistoryItem()
            {
                DeliveryProvider = GetType().Name,
                MessageDate = DateTimeOffset.Now,
                Subject = message.Subject,
                ToDisplayName = toDisplayName,
                ToEmailAddress = toEmailAddress,
                IsSuccess = false
            };

            try
            {
                using (var ms = new MemoryStream())
                {
                    message.WriteTo(FormatOptions.Default, ms, token);
                    var sr = new StreamReader(ms);
                    var mData = await sr.ReadToEndAsync();
                    historyItem.MessageData = mData;
                }

                await MailClient.ConnectAsync(Settings.SmtpServer, Settings.SmtpPort, Settings.SmtpUseSsl, token);

                if (Settings.AuthenticationSettings?.Credentials != null)
                {
                    MailClient.Authenticate(Settings.AuthenticationSettings.Credentials, token);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Settings.AuthenticationSettings?.UserName) &&
                        !string.IsNullOrEmpty(Settings.AuthenticationSettings?.Password))
                    {
                        MailClient.Authenticate(Settings.AuthenticationSettings.UserName,
                            Settings.AuthenticationSettings.Password, token);
                    }
                }

                await MailClient.SendAsync(message, token);
                historyItem.IsSuccess = true;

                await MailClient.DisconnectAsync(true, token);
            }
            catch (Exception ex)
            {
                historyItem.ExceptionMessage = ex.Message;
                Logger.LogError(1, ex, ex.Message);
            }
            finally
            {
                await HistoryStore.AddAsync(historyItem, token);
            }
            return historyItem.IsSuccess;
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
