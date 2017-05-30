using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    ///     EntityFramework message and delivery history store service
    /// </summary>
    public class EntityHistoryStore<TContext> : IHistoryStore<EntityHistoryStoreSettings> where TContext : HistoryContext
    {
        /// <summary>
        ///     Creates an instance of the EntityHistoryStore
        /// </summary>
        /// <param name="settings">The history store settings.</param>
        /// <param name="logger">An optional logger.</param>
        /// <param name="serializeAttachments">if set to <c>true</c> will serialize attachments for storage in the database.</param>
        public EntityHistoryStore(EntityHistoryStoreSettings settings, ILogger logger = null, bool serializeAttachments = false)
        {
            Settings = settings;
            Logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Gets an instance of the history context.
        /// </summary>
        /// <returns>HistoryContext.</returns>
        public HistoryContext GetHistoryContext()
        {
            return (TContext) Activator.CreateInstance(typeof(TContext), Settings.DbOptions);
        }

        /// <summary>
        ///     /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        public async Task<Guid> AddAsync(DeliveryItem item, CancellationToken token = default(CancellationToken))
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), Settings.DbOptions))
            {
                context.MessageHistory.Add(item.ToEntityHistoryDeliveryItem(Settings.StoreAttachmentContents));
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
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), Settings.DbOptions))
            {
                return (await context.FindAsync<EntityHistoryDeliveryItem>(new object[] { id }, token))
                    ?.ToDeliveryItem();
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
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), Settings.DbOptions))
            {
                return
                    await context.MessageHistory.OrderByDescending(i => i.CreatedDate)
                        .Skip(offset)
                        .Take(limit)
                        .Select(i => i.ToDeliveryItem())
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
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), Settings.DbOptions))
            {
                return await context.MessageHistory
                    .Where(
                        i =>
                            i.ToEmailAddress.Contains(searchText) || i.ToDisplayName.Contains(searchText) ||
                            i.Subject.Contains(searchText))
                    .OrderByDescending(i => i.CreatedDate)
                    .Take(limit)
                    .Select(i => i.ToDeliveryItem())
                    .ToListAsync(token);
            }
        }

        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public EntityHistoryStoreSettings Settings { get; set; }
    }
}