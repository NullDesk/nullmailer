using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     A HistoryStore that does nothing, used as a default when history is not enabled.
    /// </summary>
    /// <seealso cref="IHistoryStore" />
    public class NullHistoryStore : IHistoryStore
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NullHistoryStore" /> class.
        /// </summary>
        internal NullHistoryStore()
        {
        }

        /// <summary>
        ///     Gets a shared instance of the history store.
        /// </summary>
        /// <value>The instance.</value>
        public static NullHistoryStore Instance { get; } = new NullHistoryStore();

        /// <summary>
        ///     Does nothing.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        public Task<Guid> AddAsync(DeliveryItem item, CancellationToken token = default(CancellationToken))
        {
            if (item.Id == default(Guid))
            {
                item.Id = new Guid();
            }
            return Task.FromResult(item.Id);
        }

        /// <summary>
        ///     Does nothing, returns null.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt; always null.</returns>
        public Task<DeliveryItem> GetAsync(Guid id, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<DeliveryItem>(null);
        }

        /// <summary>
        ///     Does nothing, returns empty collection.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<IEnumerable<DeliveryItem>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult<IEnumerable<DeliveryItem>>(new DeliveryItem[] {});
        }

        /// <summary>
        ///     Does nothing, returns empty collection.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<IEnumerable<DeliveryItem>> SearchAsync(string searchText, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult<IEnumerable<DeliveryItem>>(new DeliveryItem[] {});
        }
    }
}