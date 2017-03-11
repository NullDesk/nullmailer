using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Interface for mailers that support message and delivery history stores
    /// </summary>
    public interface IHistoryMailer
    {
        /// <summary>
        ///     Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        IHistoryStore HistoryStore { get; }

        /// <summary>
        ///     ReSends the message from history data.
        /// </summary>
        /// <param name="id">The delivery item identifier to resend.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<DeliveryItem> ReSend(Guid id, CancellationToken token);
    }
}