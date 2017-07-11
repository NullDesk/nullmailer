using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     In-memory History Store.
    /// </summary>
    /// <seealso cref="IHistoryStore" />
    /// <remarks>Useful for unit tests.</remarks>
    public class InMemoryHistoryStore : IHistoryStore<StandardHistoryStoreSettings>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InMemoryHistoryStore" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">An optional logger.</param>
        public InMemoryHistoryStore(StandardHistoryStoreSettings settings = null, ILogger logger = null)
        {
            Logger = logger ?? NullLogger.Instance;
            Settings = settings ?? new StandardHistoryStoreSettings();
        }

        private List<string> Items { get; } = new List<string>();


        /// <summary>
        ///     Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task&lt;Guid&gt; the ID of the message.</returns>
        public Task<Guid> AddAsync(DeliveryItem item, CancellationToken token = default(CancellationToken))
        {
            var id = Guid.Empty;
            if (Settings.IsEnabled)
            {
                if (item.Id == default(Guid))
                {
                    item.Id = new Guid();
                }
                if (!Settings.StoreAttachmentContents)
                {
                    item.Attachments = item.Attachments.Select(i => new KeyValuePair<string, Stream>(i.Key, null))
                        .ToDictionary(k => k.Key, k => k.Value);
                }
                if (string.IsNullOrEmpty(item.SourceApplicationName) &&
                    !string.IsNullOrEmpty(Settings.SourceApplicationName))
                {
                    item.SourceApplicationName = Settings.SourceApplicationName;
                }

                Items.Add(JsonConvert.SerializeObject(item));
                id = item.Id;
            }
            return Task.FromResult(id);
        }

        /// <summary>
        ///     Gets the history item from the store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        public Task<DeliveryItem> GetAsync(Guid id, CancellationToken token = default(CancellationToken))
        {
            return Settings.IsEnabled
                ? Task.FromResult(Items
                    .Select(JsonConvert.DeserializeObject<DeliveryItem>)
                    .OrderByDescending(i => i.CreatedDate)
                    .FirstOrDefault(i => i.Id == id))
                : null;
        }

        /// <summary>
        ///     Gets a pagable list of history items from the store.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<DeliverySummary>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return
                Settings.IsEnabled
                    ? Task.FromResult(Items
                        .Select(JsonConvert.DeserializeObject<DeliverySummary>)
                        .OrderByDescending(i => i.CreatedDate)
                        .Skip(offset)
                        .Take(limit))
                    : Task.FromResult<IEnumerable<DeliverySummary>>(new DeliverySummary[] { });
        }

        /// <summary>
        ///     Searches common fields in history items and returns the specific number of matches.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="sourceApplicationName">Optional, if supplied limits the search to just the supplied source application.</param>
        /// <param name="startDate">Optional start date for range searches.</param>
        /// <param name="endDate">Optional end date for range searches.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>Searches the sender, reply to, and recipient email and display names, and the subject</remarks>
        public Task<IEnumerable<DeliverySummary>> SearchAsync(
            string searchText,
            int limit = 100,
            string sourceApplicationName = null,
            DateTimeOffset? startDate = null,
            DateTimeOffset? endDate = null,
            CancellationToken token = new CancellationToken())
        {
            return Settings.IsEnabled
                ? Task.FromResult(GetSearchResults(searchText, limit, sourceApplicationName, startDate, endDate))
                : Task.FromResult<IEnumerable<DeliverySummary>>(new DeliverySummary[] { });
        }


        /// <summary>
        ///     Optional logger
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }


        /// <summary>
        ///     The history store settings.
        /// </summary>
        /// <value>The settings.</value>
        public StandardHistoryStoreSettings Settings { get; set; }

        private IEnumerable<DeliverySummary> GetSearchResults(
            string searchText,
            int limit,
            string sourceApplicationName,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate)
        {
            var results = Items
                .Select(JsonConvert.DeserializeObject<DeliverySummary>);
            results = results.Where(
                    i =>
                        (i.ToEmailAddress?.Contains(searchText) ?? false)
                        || (i.ToDisplayName?.Contains(searchText) ?? false)
                        || (i.Subject?.Contains(searchText) ?? false)
                        || (i.FromEmailAddress?.Contains(searchText) ?? false)
                        || (i.FromDisplayName?.Contains(searchText) ?? false)
                        || (i.ReplyToEmailAddress?.Contains(searchText) ?? false)
                        || (i.ReplyToDisplayName?.Contains(searchText) ?? false))
                .OrderByDescending(i => i.CreatedDate)
                .Take(limit);
            if (!string.IsNullOrEmpty(sourceApplicationName))
            {
                results = results.Where(
                    i => i.SourceApplicationName.Equals(sourceApplicationName, StringComparison.OrdinalIgnoreCase));
            }
            if (startDate.HasValue)
            {
                results = results.Where(i => i.CreatedDate >= startDate);
            }
            if (endDate.HasValue)
            {
                results = results.Where(i => i.CreatedDate <= endDate);
            }
            return results;
        }
    }
}