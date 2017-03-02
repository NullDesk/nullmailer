using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Interface for a message and delivery history store provider
    /// </summary>
    public interface IHistoryStore
    {
        /// <summary>
        ///     Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        Task<Guid> AddAsync(MessageDeliveryItem item, CancellationToken token = default(CancellationToken));


        /// <summary>
        ///     Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        Task<MessageDeliveryItem> GetAsync(Guid id, CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Gets a pagable list of history items from the store.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        Task<IEnumerable<MessageDeliveryItem>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Searches common fields in history items and returns the specific number of matches.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        Task<IEnumerable<MessageDeliveryItem>> SearchAsync(string searchText, int limit = 100,
            CancellationToken token = default(CancellationToken));
    }
}