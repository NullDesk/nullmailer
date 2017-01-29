using System;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Interface for a message and delivery history store provider
    /// </summary>
    public interface IHistoryStore
    {
        /// <summary>
        /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        Task<Guid> AddAsync(MessageDeliveryItem item, CancellationToken token);


        /// <summary>
        /// Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        Task<MessageDeliveryItem> GetAsync(Guid id, CancellationToken token);
    }
}
