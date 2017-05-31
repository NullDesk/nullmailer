using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Mail service for sending messages via SMTP using MailKit.
    /// </summary>
    /// <seealso cref="Mailer{TSettings}" />
    /// <seealso cref="Mailer{MkSmtpMailerSettings}" />
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
            MkSmtpMailerSettings settings,
            ILogger<MkSmtpMailer> logger = null,
            IHistoryStore historyStore = null)
            : base(settings, logger, historyStore)
        {
            MailClient = client;
            if (!settings.EnableSslServerCertificateValidation)
            {
                MailClient.ServerCertificateValidationCallback = (s, c, ch, e) => true;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MkSmtpMailer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        public MkSmtpMailer(
            MkSmtpMailerSettings settings,
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
        ///     Deliver message as an asynchronous operation.
        /// </summary>
        /// <param name="deliveryItem">The delivery item containing the message you wish to send.</param>
        /// <param name="autoCloseConnection">
        ///     If set to <c>true</c> will close connection immediately after delivering the message.
        ///     If caller is sending multiple messages, optionally set to false to leave the mail service connection open.
        /// </param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;String&gt; a service provider specific message ID.</returns>
        /// <remarks>The implementor should return a provider specific ID value.</remarks>
        protected override async Task<string> DeliverMessageAsync(
            DeliveryItem deliveryItem,
            bool autoCloseConnection = true,
            CancellationToken token = default(CancellationToken))
        {
            var mkMessage = new MimeMessage
            {
                Priority = MessagePriority.Normal,
                Subject = deliveryItem.ProcessSubstitutions(deliveryItem.Subject)
            };
            mkMessage.From.Add(new MailboxAddress(deliveryItem.FromDisplayName, deliveryItem.FromEmailAddress));

            if (!string.IsNullOrEmpty(deliveryItem.ReplyToEmailAddress))
            {
                mkMessage.ReplyTo.Add(
                    new MailboxAddress(deliveryItem.ReplyToDisplayName, deliveryItem.ReplyToEmailAddress));
            }
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

            return await SendSmtpMessageAsync(mkMessage, autoCloseConnection, token);
        }

        /// <summary>
        ///     Close mail client connection as an asynchronous operation.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <remarks>Used to close connections if DeliverMessageAsync was used with autoCloseConnection set to false.</remarks>
        protected override async Task CloseMailClientConnectionAsync(
            CancellationToken token = default(CancellationToken))
        {
            if (MailClient.IsConnected)
            {
                await MailClient.DisconnectAsync(false, token);
            }
        }

        /// <summary>
        ///     Send an SMTP message as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="autoCloseConnection">if set to <c>true</c> [automatic close connection].</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task.</returns>
        protected async Task<string> SendSmtpMessageAsync(MimeMessage message,
            bool autoCloseConnection,
            CancellationToken token = default(CancellationToken))
        {
            using (await _mLock.LockAsync())
            {
                if (!MailClient.IsConnected)
                {
                    await MailClient.ConnectAsync(Settings.SmtpServer, Settings.SmtpPort, Settings.SmtpRequireSsl,
                        token);
                }
                try
                {
                    if (!MailClient.IsAuthenticated)
                    {
                        await AuthenticateMailClient(token);
                    }

                    await MailClient.SendAsync(message, token);
                }
                finally
                {
                    if (autoCloseConnection)
                    {
                        await MailClient.DisconnectAsync(false, token);
                    }
                }
            }
            return message.MessageId;
        }

        private async Task AuthenticateMailClient(CancellationToken token)
        {
            var settings = Settings.AuthenticationSettings?.GetSettingsForAuthenticationMode();
            var authType = settings?.GetType().Name;
            switch (authType)
            {
                case nameof(MkSmtpCredentialsAuthenticationSettings):
                    var ccred = ((MkSmtpCredentialsAuthenticationSettings) settings).Credentials;
                    await MailClient.AuthenticateAsync(ccred, token);
                    break;
                case nameof(MkSmtpAccessTokenAuthenticationSettings):
                    var acred = (MkSmtpAccessTokenAuthenticationSettings) settings;
                    await MailClient.AuthenticateAsync(acred.UserName, acred.AccessToken, token);
                    break;
                case nameof(MkSmtpBasicAuthenticationSettings):
                    var bcred = (MkSmtpBasicAuthenticationSettings) settings;
                    MailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                    await MailClient.AuthenticateAsync(bcred.UserName, bcred.Password, token);
                    break;
                // ReSharper disable once RedundantEmptySwitchSection
                default:
                    //no authentication used
                    break;
            }
        }


        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MailClient.Dispose();
            base.Dispose();
        }
    }
}