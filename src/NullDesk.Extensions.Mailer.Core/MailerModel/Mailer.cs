using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NullDesk.Extensions.Mailer.Core.Fluent;

// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Base IMailer implementation.
    /// </summary>
    /// <seealso cref="IMailer" />
    public abstract class Mailer<TSettings> : IMailer<TSettings> where TSettings : class, IMailerSettings
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Mailer" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected Mailer(ILogger logger = null)
        {
            Logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        ///     A collection of all messages tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        protected ICollection<DeliveryItem> Deliverables => ((IMailer) this).Deliverables;

        /// <summary>
        ///     Settings for the mailer service
        /// </summary>
        /// <value>The settings.</value>
        public abstract TSettings Settings { get; set; }

        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        public virtual void CreateMessage(Expression<Func<MessageBuilder, IBuilderStepsCompleted>> messageBuilder)
        {
            Deliverables.Add(new DeliveryItem(messageBuilder.Compile().Invoke(new MessageBuilder())));
        }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        public virtual void CreateMessage(Expression<Func<MessageBuilder, MailerMessage>> messageBuilder)
        {
            Deliverables.Add(new DeliveryItem(messageBuilder.Compile().Invoke(new MessageBuilder())));
        }

        /// <summary>
        ///     Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        public virtual void AddMessage(MailerMessage message)
        {
            Deliverables.Add(new DeliveryItem(message));
        }

        /// <summary>
        ///     Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        public virtual void AddMessages(IEnumerable<MailerMessage> messages)
        {
            foreach (var message in messages)
                AddMessage(message);
        }

        /// <summary>
        ///     Send all pending messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt; for each message sent.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual async Task<IEnumerable<MessageDeliveryItem>> SendAll(
            CancellationToken token = new CancellationToken())
        {
            var sentItems = new List<MessageDeliveryItem>();
            foreach (var message in Deliverables.Where(m => m.IsSuccess && !string.IsNullOrEmpty(m.ExceptionMessage)))
                sentItems.Add(await Send(message.Id, token));
            return sentItems;
        }

        /// <summary>
        ///     Sends one pending message with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt;.</returns>
        public abstract Task<MessageDeliveryItem> Send(Guid id, CancellationToken token = new CancellationToken());

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        ICollection<DeliveryItem> IMailer.Deliverables { get; set; }
    }
}