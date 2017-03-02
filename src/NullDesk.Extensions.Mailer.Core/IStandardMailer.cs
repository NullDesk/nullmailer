using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Standard template mailer interface
    /// </summary>
    /// <typeparam name="TSettings">The type of the mailer settings.</typeparam>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IStandardMailer" />
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.ISimpleMailer{TSettings}" />
    public interface IStandardMailer<TSettings>
        : IStandardMailer, ISimpleMailer<TSettings>
        where TSettings : class, IMailerSettings
    {
    }

    /// <summary>
    ///     Standard template mailer interface
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.ISimpleMailer" />
    public interface IStandardMailer : ISimpleMailer
    {
        /// <summary>
        ///     Send mail using a template.
        /// </summary>
        /// <param name="template">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Send mail using a template.
        /// </summary>
        /// <param name="template">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachmentFiles">A collection of paths to attachment files to include in the message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IEnumerable<string> attachmentFiles,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        ///     Send mail using a template.
        /// </summary>
        /// <param name="template">The template identifier.</param>
        /// <param name="toEmailAddress">To email address.</param>
        /// <param name="toDisplayName">To display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="replacementVariables">The replacement variables to use in the template.</param>
        /// <param name="attachments">A dictionary of attachments as streams</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Task&lt;MessageDeliveryItem&gt;.</returns>
        /// <remarks>It is up to the implementing class to decide how to locate and use the specified template.</remarks>
        Task<MessageDeliveryItem> SendMailAsync(
            string template,
            string toEmailAddress,
            string toDisplayName,
            string subject,
            IDictionary<string, string> replacementVariables,
            IDictionary<string, Stream> attachments,
            CancellationToken token = default(CancellationToken));
    }
}