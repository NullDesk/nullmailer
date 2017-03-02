using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Simplified mailer interface
    /// </summary>
    /// <typeparam name="TSettings">The type of the mailer settings.</typeparam>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.ISimpleMailer" />
    public interface ISimpleMailer<TSettings> : ISimpleMailer where TSettings : class, IMailerSettings
    {
        /// <summary>
        ///     Settings for the mailer service
        /// </summary>
        /// <returns></returns>
        TSettings Settings { get; set; }
    }

    /// <summary>
    ///     Simplified mailer
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.ISimpleMailer" />
    public interface ISimpleMailer : IDisposable
    {
        /// <summary>
        ///     Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachmentFiles">The full path to any attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<string> attachmentFiles,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Send mail as an asynchronous operation.
        /// </summary>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        Task<MessageDeliveryItem> SendMailAsync(
            string toEmailAddress,
            string toDisplayName,
            string subject,
            string htmlBody,
            string textBody,
            IDictionary<string, Stream> attachments,
            CancellationToken token = default(CancellationToken));
    }
}