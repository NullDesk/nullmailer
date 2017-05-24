using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    ///     EntityFramework message and delivery history store service
    /// </summary>
    public class EntityHistoryStore<TContext> : IHistoryStore where TContext : HistoryContext
    {
        /// <summary>
        ///     Creates an instance of the EntityHistoryStore
        /// </summary>
        /// <param name="options">The options used to configure the context</param>
        /// <param name="serializeAttachments">if set to <c>true</c> will serialize attachments for storage in the database.</param>
        public EntityHistoryStore(DbContextOptions options, bool serializeAttachments = false)
        {
            DbOptions = options;
            SerializeAttachments = serializeAttachments;
        }

        private DbContextOptions DbOptions { get; }

        /// <summary>
        ///     /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        public async Task<Guid> AddAsync(DeliveryItem item, CancellationToken token = default(CancellationToken))
        {
            using (var context = (TContext) Activator.CreateInstance(typeof(TContext), DbOptions))
            {
                context.MessageHistory.Add(item.FromDeliveryItem(SerializeAttachments));
                await context.SaveChangesAsync(token);
                return item.Id;
            }
        }

        /// <summary>
        ///     Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
       /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public async Task<DeliveryItem> GetAsync(Guid id, CancellationToken token = default(CancellationToken))
        {
            using (var context = (TContext) Activator.CreateInstance(typeof(TContext), DbOptions))
            {
                return (await context.FindAsync<EntityHistoryDeliveryItem>(new object[] {id}, token))
                    ?.FromDeliveryItem();
            }
        }

        /// <summary>
        ///     Gets a list of history items from the store.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
       /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public async Task<IEnumerable<DeliveryItem>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            using (var context = (TContext) Activator.CreateInstance(typeof(TContext), DbOptions))
            {
                return
                    await context.MessageHistory.OrderByDescending(i => i.CreatedDate)
                        .Skip(offset)
                        .Take(limit)
                        .Select(i => i.FromDeliveryItem())
                        .ToListAsync(token);
            }
        }

        /// <summary>
        ///     search as an asynchronous operation.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
       /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public async Task<IEnumerable<DeliveryItem>> SearchAsync(string searchText, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            using (var context = (TContext) Activator.CreateInstance(typeof(TContext), DbOptions))
            {
                return await context.MessageHistory
                    .Where(
                        i =>
                            i.ToEmailAddress.Contains(searchText) || i.ToDisplayName.Contains(searchText) ||
                            i.Subject.Contains(searchText))
                    .OrderByDescending(i => i.CreatedDate)
                    .Take(limit)
                    .Select(i => i.FromDeliveryItem())
                    .ToListAsync(token);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to serialize attachments for use in the history store. If not enabled,
        ///     messages with attachments cannot be resent from history.
        /// </summary>
        /// <value><c>true</c> if attachments should be serialized; otherwise, <c>false</c>.</value>
        public bool SerializeAttachments { get; set; }
    }
}