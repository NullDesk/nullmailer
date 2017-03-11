using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     In-memory History Store.
    /// </summary>
    /// <remarks>
    ///     Useful for unit tests.
    /// </remarks>
    /// <seealso cref="IHistoryStore" />
    public class InMemoryHistoryStore : IHistoryStore
    {
        private List<DeliveryItem> Items { get; } = new List<DeliveryItem>();


        /// <summary>
        ///     Adds the history item to the history store.
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
            if (!SerializeAttachments)
            {
                item.Attachments = item.Attachments.Select(i => new KeyValuePair<string, Stream>(i.Key, null)).ToDictionary(k=> k.Key, k => k.Value);
            }
            Items.Add(item);
            return Task.FromResult(item.Id);
        }

        /// <summary>
        ///     Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<DeliveryItem> GetAsync(Guid id, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(Items.OrderByDescending(i => i.CreatedDate).FirstOrDefault(i => i.Id == id));
        }

        /// <summary>
        ///     Gets a pagable list of history items from the store.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<DeliveryItem>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(Items
                .OrderByDescending(i => i.CreatedDate)
                .Skip(offset)
                .Take(limit));
        }

        /// <summary>
        ///     Searches common fields in history items and returns the specific number of matches.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<DeliveryItem>> SearchAsync(string searchText, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(Items
                .Where(
                    i =>
                        i.ToEmailAddress.Contains(searchText) || i.ToDisplayName.Contains(searchText) ||
                        i.Subject.Contains(searchText))
                .OrderByDescending(i => i.CreatedDate)
                .Take(limit));
        }

        /// <summary>
        /// Gets or sets a value indicating whether to serialize attachments for use in the history store. If not enabled, messages with attachments cannot be resent from history.
        /// </summary>
        /// <value><c>true</c> if attachments should be serialized; otherwise, <c>false</c>.</value>
        public bool SerializeAttachments { get; set; }
    }
}