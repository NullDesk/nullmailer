using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// Attempts to send all un-sent messages tracked by the mailer instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt; for each message sent.</returns>
        Task<IEnumerable<MessageDeliveryItem>> Send(CancellationToken token = default(CancellationToken));

    }
}
