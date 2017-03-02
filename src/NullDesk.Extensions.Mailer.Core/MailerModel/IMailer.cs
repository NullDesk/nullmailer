using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core.Fluent;

// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Interface IMailer
    /// </summary>
    /// <typeparam name="TSettings">The type of the t settings.</typeparam>
    public interface IMailer<TSettings> : IMailer where TSettings : class, IMailerSettings
    {
        /// <summary>
        ///     Settings for the mailer service
        /// </summary>
        /// <returns></returns>
        TSettings Settings { get; set; }
    }

    /// <summary>
    ///     Interface IMailer
    /// </summary>
    public interface IMailer : IDisposable
    {
        /// <summary>
        ///     A collection of all messages tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        ICollection<DeliveryItem> Deliverables { get; set; }

        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <returns></returns>
        ILogger Logger { get; }

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        void CreateMessage(Expression<Func<MessageBuilder, IBuilderStepsCompleted>> messageBuilder);

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        void CreateMessage(Expression<Func<MessageBuilder, MailerMessage>> messageBuilder);

        /// <summary>
        ///     Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        void AddMessage(MailerMessage message);

        /// <summary>
        ///     Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        void AddMessages(IEnumerable<MailerMessage> messages);

        /// <summary>
        ///     Attempts to send all un-sent messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The token.</param>
        Task<IEnumerable<MessageDeliveryItem>> SendAll(CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Sends one pending message with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt;.</returns>
        Task<MessageDeliveryItem> Send(Guid id, CancellationToken token = default(CancellationToken));
    }
}