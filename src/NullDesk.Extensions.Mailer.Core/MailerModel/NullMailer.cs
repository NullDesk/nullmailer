using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     A mailer instance that does nothing.
    /// </summary>
    /// <seealso cref="Mailer{NullMailerSettings}" />
    public class NullMailer : Mailer<NullMailerSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullMailer" /> class.
        /// </summary>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="historyStore">The history store.</param>
        public NullMailer(IOptions<NullMailerSettings> settings, ILogger logger = null, IHistoryStore historyStore = null) : base(settings.Value, logger, historyStore)
        {
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            //do nothing
        }

        /// <summary>
        ///     When overridden in a derived class, uses the mailer's underlying mail delivery service to send the specified
        ///     message .
        /// </summary>
        /// <param name="deliveryItem">The delivery item containing the message you wish to send.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;DeliveryItem&gt;.</returns>
        protected override Task<DeliveryItem> DeliverMessageAsync(DeliveryItem deliveryItem,
            CancellationToken token = new CancellationToken())
        {
            deliveryItem.IsSuccess = true;
            return Task.FromResult(deliveryItem);
        }
    }
}