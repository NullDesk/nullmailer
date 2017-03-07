using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Mail service for sending messages via SMTP using MailKit.
    /// </summary>
    /// <seealso cref="Mailer{TSettings}" />
    /// <seealso cref="IHistoryMailer" />
    /// <seealso cref="Mailer{MkSmtpMailerSettings}" />
    /// <seealso cref="IHistoryMailer" />
    public class MkSmtpMailer : Mailer<MkSmtpMailerSettings>
    {
        private readonly AsyncLock _mLock = new AsyncLock();


        /// <summary>
        ///     Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        public MkSmtpMailer(
            SmtpClient client,
            IOptions<MkSmtpMailerSettings> settings,
            ILogger<MkSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
            : base(settings.Value, logger, historyStore)
        {
            MailClient = client;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        public MkSmtpMailer(
            IOptions<MkSmtpMailerSettings> settings,
            ILogger<MkSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
            : this(new SmtpClient(), settings, logger, historyStore)
        {
        }

        /// <summary>
        ///     Gets the mail client.
        /// </summary>
        /// <value>The mail client.</value>
        public SmtpClient MailClient { get; }

        /// <summary>
        ///     Delivers a single message using the mailkit framework.
        /// </summary>
        /// <param name="deliveryItem">The delivery item containing the message you wish to send.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;DeliveryItem&gt;.</returns>
        protected override async Task<DeliveryItem> DeliverMessageAsync(DeliveryItem deliveryItem,
            CancellationToken token = new CancellationToken())
        {
            var mkMessage = new MimeMessage
            {
                Priority = MessagePriority.Normal,
                Subject = deliveryItem.ProcessSubstitutions(deliveryItem.Subject)
            };
            mkMessage.From.Add(new MailboxAddress(deliveryItem.FromDisplayName, deliveryItem.FromEmailAddress));

            try
            {
                var box = string.IsNullOrEmpty(deliveryItem.ToDisplayName)
                    ? new MailboxAddress(deliveryItem.ToEmailAddress)
                    : new MailboxAddress(deliveryItem.ToDisplayName, deliveryItem.ToEmailAddress);
                mkMessage.To.Add(box);
            }
            catch (FormatException)
            {
                Logger.LogError(1,
                    "Invalid email address format: {name} <{address}> Subject: {subject}",
                    deliveryItem.ToDisplayName, deliveryItem.ToEmailAddress, deliveryItem.Subject);
                throw;
            }

            mkMessage.Body = (await new BodyBuilder()
                    .GetMkBody(deliveryItem, Settings, Logger, token))
                .AddMkAttachmentStreams(deliveryItem.Attachments)
                .ToMessageBody();

            await SendSmtpMessageAsync(mkMessage, token);

            return deliveryItem;
        }

        /// <summary>
        ///     Send an SMTP message as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        protected async Task SendSmtpMessageAsync(MimeMessage message,
            CancellationToken token = default(CancellationToken))
        {
            using (await _mLock.LockAsync())
            {
                await MailClient.ConnectAsync(Settings.SmtpServer, Settings.SmtpPort, Settings.SmtpUseSsl, token);
                if (Settings.AuthenticationSettings?.Credentials != null)
                {
                    await MailClient.AuthenticateAsync(Settings.AuthenticationSettings.Credentials, token);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Settings.AuthenticationSettings?.UserName) &&
                        !string.IsNullOrEmpty(Settings.AuthenticationSettings?.Password))
                    {
                        await MailClient.AuthenticateAsync(Settings.AuthenticationSettings.UserName,
                            Settings.AuthenticationSettings.Password, token);
                    }
                }
                await MailClient.SendAsync(message, token);
                await MailClient.DisconnectAsync(false, token);
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MailClient.Dispose();
        }
    }
}