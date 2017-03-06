// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     A message body containing html and/or plain text content
    /// </summary>
    public class ContentBody : IMessageBody
    {
        /// <summary>
        ///     An HTML message body.
        /// </summary>
        /// <remarks>
        ///     If substitutions are provided, they will be used here.
        /// </remarks>
        /// <value>The content of the HTML.</value>
        public string HtmlContent { get; set; }

        /// <summary>
        ///     A plain text message body.
        /// </summary>
        /// <remarks>
        ///     If substitutions are provided, they will be used here.
        /// </remarks>
        /// <value>The content of the plain text.</value>
        public string PlainTextContent { get; set; }

        /// <summary>
        ///     Creates a message body.
        /// </summary>
        /// <returns>ContentBody.</returns>
        public static ContentBody Create()
        {
            return new ContentBody();
        }
    }
}