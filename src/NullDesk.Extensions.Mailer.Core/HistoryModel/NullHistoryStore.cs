using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     A HistoryStore that does nothing, used as a default when history is not enabled.
    /// </summary>
    /// <seealso cref="IHistoryStore" />
    public class NullHistoryStore : IHistoryStore<StandardHistoryStoreSettings>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NullHistoryStore" /> class.
        /// </summary>
        internal NullHistoryStore()
        {
            Logger = NullLogger.Instance;
            Settings = new StandardHistoryStoreSettings();
        }

        /// <summary>
        ///     Gets a shared instance of the history store.
        /// </summary>
        /// <value>The instance.</value>
        public static NullHistoryStore Instance { get; } = new NullHistoryStore();

        /// <summary>
        ///     Gets or sets a value indicating whether to serialize attachments for use in the history store. If not enabled,
        ///     messages with attachments cannot be resent from history.
        /// </summary>
        /// <value><c>true</c> if attachments should be serialized; otherwise, <c>false</c>.</value>
        public bool SerializeAttachments { get; set; } = false;

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
        /// <param name="token">The cancellation token.</param>
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
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<IEnumerable<DeliverySummary>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult<IEnumerable<DeliverySummary>>(new DeliverySummary[] { });
        }

        /// <summary>
        /// Does nothing, returns empty collection.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="sourceApplicationName">Optional, if supplied limits the search to just the supplied source application.</param>
        /// <param name="startDate">Optional start date for range searches.</param>
        /// <param name="endDate">Optional end date for range searches.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        /// <remarks>Searches the sender, reply to, and recipient email and display names, and the subject</remarks>
        public Task<IEnumerable<DeliverySummary>> SearchAsync(
            string searchText, 
            int limit = 100,
            string sourceApplicationName = null,
            DateTimeOffset? startDate = null,
            DateTimeOffset? endDate = null,
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult<IEnumerable<DeliverySummary>>(new DeliverySummary[] { });
        }

        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public StandardHistoryStoreSettings Settings { get; set; }
    }
}