using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///  A simplified mailer that does nothing
    /// </summary>
    /// <remarks>Does nothing, useful for tests</remarks>
    public class NullSimpleMailer : ISimpleMailer, IHistoryMailer
    {

        /// <summary>
        /// Gets the history store.
        /// </summary>
        /// <value>The history store.</value>
        public IHistoryStore HistoryStore { get; }

        /// <summary>
        /// Optional logger
        /// </summary>
        /// <returns></returns>
        protected ILogger Logger { get; }

        /// <summary>
        /// Creates an instance of the NullSimpleMailer
        /// </summary>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <param name="historyStore">Optional history store provider.</param>
        public NullSimpleMailer(ILogger<NullSimpleMailer> logger = null, IHistoryStore historyStore = null)
        {
            Logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance as ILogger;
            HistoryStore = historyStore ?? new NullHistoryStore();
        }

        private Task<MessageDeliveryItem> PretendToSendAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            CancellationToken token = default(CancellationToken))
        {
            var hi = new MessageDeliveryItem
            {
                Id = Guid.NewGuid(),
                IsSuccess = true,
                DeliveryProvider = this.GetType().Name,
                CreatedDate = DateTimeOffset.Now,
                Subject = subject,
                MessageData = "Message sent with a null mailer",
                ToDisplayName = toDisplayName,
                ToEmailAddress = toEmailAddress,
            };
            HistoryStore.AddAsync(hi, token).Wait(token);
            return Task.FromResult(hi);
        }
        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        public Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            CancellationToken token = default(CancellationToken))
        {
            return PretendToSendAsync(toEmailAddress, toDisplayName, subject, token);
        }

        /// <summary>
        /// /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachmentFiles">The full path to any attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        public Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<string> attachmentFiles,
            CancellationToken token = default(CancellationToken))
        {
            return PretendToSendAsync(toEmailAddress, toDisplayName, subject, token);
        }

        /// <summary>
        /// Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        public Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IDictionary<string, Stream> attachments,
            CancellationToken token = default(CancellationToken))
        {
            return PretendToSendAsync(toEmailAddress, toDisplayName, subject, token);
        }

        /// <summary>
        /// ReSends the message from history data.
        /// </summary>
        /// <param name="id">The identifier for the message being resent.</param>
        /// <param name="historyData">The history data.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ReSend(Guid id, string historyData, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}