using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.History;

namespace NullDesk.Extensions.Mailer.Core.History
{
    /// <summary>
    /// A HistoryStore that does nothing, used as a default when history is not enabled.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.History.IHistoryStore" />
    public class NullHistoryStore: IHistoryStore
    {

        /// <summary>
        /// Adds the history item to the history store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>Task.</returns>
        public Task AddAsync(HistoryItem item, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
