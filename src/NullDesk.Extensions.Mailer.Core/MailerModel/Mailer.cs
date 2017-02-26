using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Base IMailer implementation.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IMailer" />
    public abstract class Mailer: IMailer
    {
        /// <summary>
        /// Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// A collection of all messages tracked by this mailer instance.
        /// </summary>
        /// <value>The messages.</value>
        public ICollection<MailerMessage> Messages { get; set; } = new Collection<MailerMessage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mailer"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected Mailer(ILogger logger =null)
        {
            Logger = logger ?? NullLogger.Instance;
        }

        
        /// <summary>
        /// Adds a message to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        public virtual MailerMessage AddMessage(MailerMessage message)
        {
  
            Messages.Add(message);
            return message;
        }

        /// <summary>
        /// Adds a collection of messages to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <returns>Task&lt;MailerMessage&gt;.</returns>
        public virtual IEnumerable<MailerMessage> AddMessages(IEnumerable<MailerMessage> messages)
        {
            return messages.Select(AddMessage).ToList();
        }

        /// <summary>
        /// Send all pending messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt; for each message sent.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public abstract Task<IEnumerable<MessageDeliveryItem>> Send(CancellationToken token = new CancellationToken());

    }
}
