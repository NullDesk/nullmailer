// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     History store settings
    /// </summary>
    public interface IHistoryStoreSettings
    {
        /// <summary>
        ///     Indicates if delivery history is enabled.
        /// </summary>
        /// <value><c>true</c> if history is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }

        /// <summary>
        ///     Indicates whether to store attachment content with the delivery history.
        /// </summary>
        /// <remarks>
        ///     If <c>false</c> messages in history will not be resendable if the original had attachments.
        /// </remarks>
        /// <value><c>true</c> if content of attachments should be included in history otherwise, <c>false</c>.</value>
        bool StoreAttachmentContents { get; set; }

        /// <summary>
        ///     The name of the application to be include in history.
        /// </summary>
        /// <remarks>Use to give a name to the system recording history.</remarks>
        /// <value>The name of the delivery provider.</value>
        string SourceApplicationName { get; set; }
    }
}