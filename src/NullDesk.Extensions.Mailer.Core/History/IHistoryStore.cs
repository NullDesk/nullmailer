using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.History;

namespace NullDesk.Extensions.Mailer.Core.History
{
    /// <summary>
    /// Interface for a message and delivery history store provider
    /// </summary>
    public interface IHistoryStore
    {
        /// <summary>
        /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task.</returns>
        Task AddAsync(HistoryItem item, CancellationToken token);
    }
}
