using System;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core.History
{
    /// <summary>
    /// Interface for mailers that support message and delivery history stores
    /// </summary>
    public interface IHistoryMailer
    {

        /// <summary>
        /// Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        IHistoryStore HistoryStore { get; }

        /// <summary>
        /// ReSends the message from history data.
        /// </summary>
        /// <param name="id">The identifier for the message being resent.</param>
        /// <param name="historyData">The history data.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ReSend(Guid id, string historyData, CancellationToken token);
    }
}
