using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Interface IMailer
    /// </summary>
    public interface IMailer
    {
        /// <summary>
        /// A collection of all messages tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        ICollection<MailerMessage> Messages { get; set; }

        /// <summary>
        /// Optional logger
        /// </summary>
        /// <returns></returns>
        ILogger Logger { get; }

        /// <summary>
        /// Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        MailerMessage AddMessage(MailerMessage message);

        /// <summary>
        /// Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        IEnumerable<MailerMessage> AddMessages(IEnumerable<MailerMessage> messages);

        /// <summary>
        /// Attempts to send all un-sent messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt; for each message sent.</returns>
        Task<IEnumerable<MessageDeliveryItem>> Send(CancellationToken token = default(CancellationToken));

    }
}
