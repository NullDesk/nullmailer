using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core.History
{
    /// <summary>
    /// In-memory History Store.
    /// </summary>
    /// <remarks>
    /// Useful for unit tests.
    /// </remarks>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.History.IHistoryStore" />
    public class MemoryHistoryStore : IHistoryStore
    {
        private List<MessageDeliveryItem> Items { get; } = new List<MessageDeliveryItem>();


        /// <summary>
        /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        public Task<Guid> AddAsync(MessageDeliveryItem item, CancellationToken token)
        {
            if (item.Id == default(Guid))
            {
                item.Id = new Guid();
            }
            Items.Add(item);
            return Task.FromResult(item.Id);
        }

        /// <summary>
        /// Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<MessageDeliveryItem> GetAsync(Guid id, CancellationToken token)
        {
            return Task.FromResult(Items.FirstOrDefault(i => i.Id == id));
        }
    }
}
