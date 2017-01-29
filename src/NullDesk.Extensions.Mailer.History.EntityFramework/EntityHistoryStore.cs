using System;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.Core;
using Microsoft.EntityFrameworkCore;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    /// EntityFramework message and delivery history store service
    /// </summary>
    public class EntityHistoryStore<TContext> : IHistoryStore where TContext : HistoryContext
    {
        private DbContextOptions DbOptions { get; }

        /// <summary>
        /// Creates an instance of the EntityHistoryStore
        /// </summary>
        /// <param name="options">The options used to configure the context</param>
        public EntityHistoryStore(DbContextOptions options)
        {
            DbOptions = options;
        }

        /// <summary>
        /// /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        public async Task<Guid> AddAsync(MessageDeliveryItem item, CancellationToken token)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), DbOptions))
            {
                context.MessageHistory.Add(item);
                await context.SaveChangesAsync(token);
                return item.Id;
            }
        }

        /// <summary>
        /// Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<MessageDeliveryItem> GetAsync(Guid id, CancellationToken token)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), DbOptions))
            {
                return context.FindAsync<MessageDeliveryItem>(new object[] { id }, token);
            }
        }
    }

}