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
        public Task<IEnumerable<DeliveryItem>> GetAsync(int offset = 0, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return
                Settings.IsEnabled
                    ? Task.FromResult(Items
                        .Select(JsonConvert.DeserializeObject<DeliveryItem>)
                        .OrderByDescending(i => i.CreatedDate)
                        .Skip(offset)
                        .Take(limit))
                    : null;
        }

        /// <summary>
        ///     Searches common fields in history items and returns the specific number of matches.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;HistoryItem&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<DeliveryItem>> SearchAsync(string searchText, int limit = 100,
            CancellationToken token = new CancellationToken())
        {
            return Settings.IsEnabled
                ? Task.FromResult(Items
                    .Select(JsonConvert.DeserializeObject<DeliveryItem>)
                    .Where(i =>
                        i.ToEmailAddress.Contains(searchText) || i.ToDisplayName.Contains(searchText) ||
                        i.Subject.Contains(searchText))
                    .OrderByDescending(i => i.CreatedDate)
                    .Take(limit))
                : null;
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
    }
}