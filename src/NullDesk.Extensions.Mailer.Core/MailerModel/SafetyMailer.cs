using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core.Fluent;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     A proxy implementation of IMailer that can ensure that all recipient addresses are always overridden with a known
    ///     'safe' recipient email address.
    /// </summary>
    /// <remarks>
    ///     Useful for deploying non-production versions of applications with email enabled, but ensuring that real users never
    ///     accidentally receive email.
    ///     The display name for the recipient will be modified to show the original display name and original destination
    ///     email address.
    /// </remarks>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IMailer" />
    public class SafetyMailer<TMailer> : IMailer, IProxyMailer<SafetyMailerSettings, TMailer>
        where TMailer : class, IMailer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SafetyMailer{TMailer}" /> class.
        /// </summary>
        /// <param name="mailer">The mailer.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <exception cref="ArgumentException">Safety mailer cannot proxy another implementation of ISafetyMailer</exception>
        public SafetyMailer(TMailer mailer, SafetyMailerSettings safetyMailerSettings)
        {
            if (string.IsNullOrWhiteSpace(safetyMailerSettings.SafeRecipientEmailAddress))
            {
                throw new ArgumentException(
                    "Safety mailer cannot enable safe recipients when SafeRecipientEmailAddress is not specified");
            }

            Mailer = mailer;

            SafetySettings = safetyMailerSettings;
        }

        private SafetyMailerSettings SafetySettings { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Mailer.Dispose();
        }

        /// <summary>
        ///     Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        public IHistoryStore HistoryStore => Mailer.HistoryStore;

        /// <summary>
        ///     A collection of all pending delivery items tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        public ICollection<DeliveryItem> PendingDeliverables
        {
            get => Mailer.PendingDeliverables;
            set => Mailer.PendingDeliverables = value;
        }


        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger
        {
            get => Mailer.Logger;
            set => Mailer.Logger = value;
        }

        /// <summary>
        ///     Gets the default message sender, usually specified by settings during mailer constructions.
        /// </summary>
        /// <value>The default sender.</value>
        public MessageSender DefaultSender => Mailer.DefaultSender;


        /// <summary>
        ///     ReSends the message from history data.
        /// </summary>
        /// <param name="id">The delivery item identifier to resend.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// The cancellation token.
        public Task<DeliveryItem> ReSendAsync(Guid id, CancellationToken token)
        {
            EnsureSafeRecipients();
            return Mailer.ReSendAsync(id, token);
        }

        /// <summary>
        ///     Gets a message builder for the mailer's default sender.
        /// </summary>
        /// <returns>MessageBuilder.</returns>
        public MessageBuilder.BuildSubjectStep GetMessageBuilder()
        {
            return Mailer.GetMessageBuilder();
        }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public IEnumerable<Guid> CreateMessage(
            Expression<Func<MessageBuilder.BuildSubjectStep, IBuilderStepsCompleted>> messageBuilder)
        {
            return Mailer.CreateMessage(messageBuilder);
        }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public IEnumerable<Guid> CreateMessage(
            Expression<Func<MessageBuilder.BuildSubjectStep, MailerMessage>> messageBuilder)
        {
            return Mailer.CreateMessage(messageBuilder);
        }

        /// <summary>
        ///     Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public IEnumerable<Guid> AddMessage(MailerMessage message)
        {
            return Mailer.AddMessage(message);
        }

        /// <summary>
        ///     Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public IEnumerable<Guid> AddMessages(IEnumerable<MailerMessage> messages)
        {
            return Mailer.AddMessages(messages);
        }

        /// <summary>
        ///     Attempts to send all un-sent messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;DeliveryItem&gt;&gt;.</returns>
        public Task<IEnumerable<DeliveryItem>> SendAllAsync(CancellationToken token = default(CancellationToken))
        {
            EnsureSafeRecipients();
            return Mailer.SendAllAsync(token);
        }

        /// <summary>
        ///     Sends one pending message with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="autoCloseConnection">if set to <c>true</c> automaticly close the connection when the message is sent.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt;.</returns>
        /// The cancellation token.
        public Task<DeliveryItem> SendAsync(Guid id, bool autoCloseConnection = true,
            CancellationToken token = default(CancellationToken))
        {
            EnsureSafeRecipients();
            return Mailer.SendAsync(id, autoCloseConnection, token);
        }

        /// <summary>
        ///     Gets the underlying mailer instance wrapped by the proxy.
        /// </summary>
        /// <value>The mailer.</value>
        public TMailer Mailer { get; }

        /// <summary>
        ///     The settings for the proxy mailer.
        /// </summary>
        /// <value>The settings.</value>
        public SafetyMailerSettings Settings { get; set; }

        private void EnsureSafeRecipients()
        {
            foreach (var pendingDeliverable in PendingDeliverables)
            {
                pendingDeliverable.ToDisplayName =
                    $"{GetPrependDisplayNameText()}{GetSafetyDisplayName(pendingDeliverable)} <'{pendingDeliverable.ToEmailAddress}'>";
                pendingDeliverable.ToEmailAddress = SafetySettings.SafeRecipientEmailAddress;
            }
        }

        private string GetSafetyDisplayName(DeliveryItem pendingDeliverable)
        {
            return string.IsNullOrWhiteSpace(pendingDeliverable.ToDisplayName)
                ? pendingDeliverable.ToEmailAddress
                : pendingDeliverable.ToDisplayName;
        }

        private string GetPrependDisplayNameText()
        {
            return string.IsNullOrWhiteSpace(SafetySettings.PrependDisplayNameWithText)
                ? string.Empty
                : $"{SafetySettings.PrependDisplayNameWithText} ";
        }
    }
}