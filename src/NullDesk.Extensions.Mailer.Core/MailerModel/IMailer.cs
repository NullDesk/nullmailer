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
        ///     Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        IHistoryStore HistoryStore { get; }

        /// <summary>
        ///     A collection of all pending delivery items tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        ICollection<DeliveryItem> PendingDeliverables { get; set; }

        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <returns></returns>
        ILogger Logger { get; }

        /// <summary>
        ///     Gets the default message sender, usually specified by settings during mailer constructions.
        /// </summary>
        /// <value>The default sender.</value>
        MessageSender DefaultSender { get; }

        /// <summary>
        /// ReSends the message from history data.
        /// </summary>
        /// <param name="id">The delivery item identifier to resend.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// The cancellation token.
        Task<DeliveryItem> ReSendAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Gets a message builder for the mailer's default sender.
        /// </summary>
        /// <returns>MessageBuilder.</returns>
        MessageBuilder.BuildSubjectStep GetMessageBuilder();

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        IEnumerable<Guid> CreateMessage(
            Expression<Func<MessageBuilder.BuildSubjectStep, IBuilderStepsCompleted>> messageBuilder);

        /// <summary>
        ///     Use the fluent builder API to add a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        IEnumerable<Guid> CreateMessage(
            Expression<Func<MessageBuilder.BuildSubjectStep, MailerMessage>> messageBuilder);

        /// <summary>
        ///     Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A collection of delivery item identifiers.</returns>
        IEnumerable<Guid> AddMessage(MailerMessage message);

        /// <summary>
        ///     Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <returns>A collection of delivery item identifiers.</returns>
        IEnumerable<Guid> AddMessages(IEnumerable<MailerMessage> messages);

        /// <summary>
        ///     Attempts to send all un-sent messages tracked by the mailer instance.
        /// </summary>
       /// <param name="token">The cancellation token.</param>
        Task<IEnumerable<DeliveryItem>> SendAllAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Sends one pending message with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="autoCloseConnection">if set to <c>true</c> automaticly close the connection when the message is sent.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt;.</returns>
        /// The cancellation token.
        Task<DeliveryItem> SendAsync(Guid id, bool autoCloseConnection = true, CancellationToken token = default(CancellationToken));
    }
}