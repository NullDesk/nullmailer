using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// A mailer instance that does nothing.
    /// </summary>
    /// <seealso cref="Mailer{NullMailerSettings}" />
    public class NullMailer: Mailer<NullMailerSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullMailer"/> class.
        /// </summary>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public NullMailer(NullMailerSettings settings, ILogger logger = null) : base(settings, logger)
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IEnumerable&lt;MessageDeliveryItem&gt;&gt;.</returns>
        public override Task<DeliveryItem> Send(Guid id, CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(new DeliveryItem(new MailerMessage(){Id = id}));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            //do nothing
        }
    }
}
