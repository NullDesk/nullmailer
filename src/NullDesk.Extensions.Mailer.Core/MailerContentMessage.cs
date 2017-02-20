namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// A mail message where the body is provided by the caller.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.MailerMessage" />
    public class MailerContentMessage : MailerMessage
    {
        /// <summary>
        /// An HTML message body.
        /// </summary>
        /// <remarks>
        /// If substitutions are provided, they will be used here.
        /// </remarks>
        /// <value>The content of the HTML.</value>
        public string HtmlContent { get; set; }

        /// <summary>
        /// A plain text message body.
        /// </summary>
        /// <remarks>
        /// If substitutions are provided, they will be used here.
        /// </remarks>
        /// <value>The content of the plain text.</value>
        public string PlainTextContent { get; set; }
    }
}