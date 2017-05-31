namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Standard History Store Settings.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IHistoryStoreSettings" />
    public class StandardHistoryStoreSettings : IHistoryStoreSettings
    {
        /// <summary>
        ///     Indicates if delivery history is enabled. Default is true.
        /// </summary>
        /// <value><c>true</c> if history is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled { get; set; } = true;


        /// <summary>
        ///     Indicates whether to store attachment content with the delivery history. Default is false.
        /// </summary>
        /// <value><c>true</c> if content of attachments should be included in history otherwise, <c>false</c>.</value>
        /// <remarks>If <c>false</c> messages in history will not be resendable if the original had attachments.</remarks>
        public bool StoreAttachmentContents { get; set; } = false;
    }
}