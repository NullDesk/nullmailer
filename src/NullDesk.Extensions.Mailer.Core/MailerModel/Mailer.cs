using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NullDesk.Extensions.Mailer.Core.Fluent;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;

// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Base IMailer implementation.
    /// </summary>
    /// <seealso cref="IMailer" />
    public abstract class Mailer<TSettings> : IMailer<TSettings> where TSettings : class, IMailerSettings
    {
        private readonly AsyncLock _deliverablesLock = new AsyncLock();


        /// <summary>
        ///     Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        public IHistoryStore HistoryStore { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mailer" /> class.
        /// </summary>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        protected Mailer(TSettings settings, ILogger logger = null, IHistoryStore historyStore = null)
        {
            Settings = settings;
            Logger = logger ?? NullLogger.Instance;
            HistoryStore = historyStore ?? NullHistoryStore.Instance;
        }

        /// <summary>
        ///     A collection of all messages tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        protected ICollection<DeliveryItem> DeliveryItems => ((IMailer)this).Deliverables;

        /// <summary>
        ///     Settings for the mailer service
        /// </summary>
        /// <value>The settings.</value>
        public TSettings Settings { get; set; }

        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the default message sender, usually specified by settings during mailer constructions.
        /// </summary>
        /// <value>The default sender.</value>
        public MessageSender DefaultSender => new MessageSender().FromAddress(Settings.FromEmailAddress).WithDisplayName(Settings.FromDisplayName);

        /// <summary>
        /// Gets a message builder for the mailer's default sender.
        /// </summary>
        /// <returns>MessageBuilder.</returns>
        public MessageBuilder.BuildSubjectStep GetMessageBuilder()
        {
            return new MessageBuilder().ForSettings(Settings);
        }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public virtual IEnumerable<Guid> CreateMessage(
            Expression<Func<MessageBuilder.BuildSubjectStep, IBuilderStepsCompleted>> messageBuilder)
        {
            var message = messageBuilder.Compile().Invoke(new MessageBuilder().ForSettings(Settings)).Build();
            return AddMessage(message);
        }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public virtual IEnumerable<Guid> CreateMessage(
            Expression<Func<MessageBuilder.BuildSubjectStep, MailerMessage>> messageBuilder)
        {
            var message = messageBuilder.Compile().Invoke(new MessageBuilder().ForSettings(Settings));
            return AddMessage(message);
        }

        /// <summary>
        ///     Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public virtual IEnumerable<Guid> AddMessages(IEnumerable<MailerMessage> messages)
        {
            var ids = new List<Guid>();
            foreach (var m in messages)
            {
                ids.AddRange(AddMessage(m));
            }
            return ids;
        }

        /// <summary>
        ///     Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        public virtual IEnumerable<Guid> AddMessage(MailerMessage message)
        {
            CheckIsDeliverable(message);
            var items = message.Recipients.Select(recipient =>
                new DeliveryItem(message, recipient)).ToList();

            using (_deliverablesLock.LockAsync().Result)
            {
                foreach (var i in items)
                {
                    Logger.LogDebug(
                        "Mailer added new delivery item ID '{deliveryItemId}' to '{to}' with subject '{subject}'",
                        i.Id,
                        i.ToEmailAddress,
                        i.Subject);
                    ((IMailer)this).Deliverables.Add(i);
                }
            }

            return items.Select(i => i.Id);
        }

        /// <summary>
        ///     Send all pending messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt; for each message sent.</returns>
        public virtual async Task<IEnumerable<DeliveryItem>> SendAllAsync(
            CancellationToken token = new CancellationToken())
        {
            var sentItems = new List<DeliveryItem>();
            var sendIds = new List<Guid>();
            using (await _deliverablesLock.LockAsync())
            {
                foreach (var message in
                    ((IMailer)this).Deliverables.Where(m => !m.IsSuccess && string.IsNullOrEmpty(m.ExceptionMessage)))
                {
                    sendIds.Add(message.Id);
                }
            }
            foreach (var id in sendIds)
            {
                sentItems.Add(await SendAsync(id, token));
            }
            return sentItems;
        }

        /// <summary>
        ///     Sends one pending delivery item with the specified identifier.
        /// </summary>
        /// <param name="id">The delivery item identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt;.</returns>
        public virtual async Task<DeliveryItem> SendAsync(Guid id, CancellationToken token = new CancellationToken())
        {
            using (await _deliverablesLock.LockAsync())
            {
                var deliveryItem = ((IMailer)this).Deliverables.FirstOrDefault(d => d.Id == id);
                try
                {
                    deliveryItem.ProviderMessageId = await DeliverMessageAsync(deliveryItem, token);
                    deliveryItem.IsSuccess = true;
                    deliveryItem.DeliveryProvider = GetType().Name;
                    Logger.LogInformation(
                        "Mailer delivery {result} for delivery item '{deliveryItemId}' sent to '{to}' with subject '{subject}'",
                        deliveryItem.IsSuccess ? "success" : "failure",
                        deliveryItem.Id,
                        deliveryItem.ToEmailAddress,
                        deliveryItem.Subject
                        );
                }
                catch (Exception ex)
                {
                    Logger.LogError(1, ex, "Mailer delivery {result} for delivery item '{deliveryItemId}' with exception {exceptionMessage}", "error", id, ex.Message);
                    deliveryItem.ExceptionMessage = $"{ex.Message}\n\n{ex.StackTrace}";
                }
                finally
                {
                    await HistoryStore.AddAsync(deliveryItem, token);
                }
                return deliveryItem;
            }
        }

        /// <summary>
        /// ReSends the message from history data.
        /// </summary>
        /// <param name="id">The delivery item identifier to resend.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.ArgumentException">
        /// Message with the specified id not found in the history store, unable to resend message.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// DeliveryItem is not re-sendable. The message may have originally been delivered with attachments, but attachment serialization may have been disabled for the history store
        /// </exception>
        public virtual async Task<DeliveryItem> ReSendAsync(Guid id, CancellationToken token)
        {
            var di = await HistoryStore.GetAsync(id, token);
            if (di == null)
            {
                throw new ArgumentException($"Mailer re-send error for delivery item ID '{id}'. Delivery item not found in the history store, unable to resend message");
            }
            if (!di.IsResendable)
            {
                throw new InvalidOperationException(
                    $"Mailer re-send error for delivery item ID '{id}'. Delivery item is not re-sendable. The message may have originally been delivered with attachments, but attachment serialization may have been disabled for the history store");
            }

            di.Id = Guid.NewGuid();
            di.ProviderMessageId = null;
            di.IsSuccess = false;
            di.CreatedDate = DateTimeOffset.Now;
            di.ExceptionMessage = null;


            

            Logger.LogInformation(
                "Mailer preparing to re-send delivery item ID '{deliveryItemId}' to '{to}' with subject '{subject}'",
                di.Id,
                di.ToEmailAddress,
                di.Subject);
            ((IMailer)this).Deliverables.Add(di);
            return (await SendAsync(di.Id, token));
        }

        ICollection<DeliveryItem> IMailer.Deliverables { get; set; } = new Collection<DeliveryItem>();


        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            var unsent = DeliveryItems.Where(m => !m.IsSuccess && string.IsNullOrEmpty(m.ExceptionMessage)).ToArray();
            if (unsent.Any())
            {
                var b = new StringBuilder();
                b.AppendLine("Mailer instance disposed with {numMessages} undelivered messages");
                b.AppendLine("Instance type '{mailerType}'");
                b.AppendLine("Undelivered Messages:");
                foreach (var u in unsent)
                {
                    b.AppendLine($"    Delivery item ID '{u.Id}' to '{u.ToEmailAddress}' with subject '{u.Subject}'");
                }
                Logger.LogWarning(b.ToString(), unsent.Count(), GetType().Name);
            }
            ((IMailer)this).Deliverables = null;
        }

        /// <summary>
        /// When overridden in a derived class, uses the mailer's underlying mail delivery service to send the specified
        /// message and return the service's native message identifier (or null if not applicable).
        /// </summary>
        /// <param name="deliveryItem">The delivery item containing the message you wish to send.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;String&gt; a service provider specific message ID.</returns>
        /// <remarks>The implementor should return a provider specific ID value.</remarks>
        protected abstract Task<string> DeliverMessageAsync(DeliveryItem deliveryItem,
            CancellationToken token = new CancellationToken());

        private void CheckIsDeliverable(MailerMessage message)
        {
            if (!message.IsDeliverable)
            {
                var ex =
                    new ArgumentException(
                        $"MailerMessage with subject '{message.Subject}' is not valid for conversion into a DeliveryItem. Make sure all messages have a sender, body, and at least one recipient specified");
                Logger.LogError(1, ex, ex.Message);
                throw ex;
            }
        }


    }
}