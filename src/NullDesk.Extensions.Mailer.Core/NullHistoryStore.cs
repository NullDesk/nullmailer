using System;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// A HistoryStore that does nothing, used as a default when history is not enabled.
    /// </summary>
    /// <seealso cref="IHistoryStore" />
    public class NullHistoryStore: IHistoryStore
    {
        /// <summary>
        /// Does nothing.
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
            return Task.FromResult(item.Id);
        }

        /// <summary>
        /// Does nothing, returns null.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt; always null.</returns>
        public Task<MessageDeliveryItem> GetAsync(Guid id, CancellationToken token)
        {
            return null;
        }
    }
}
