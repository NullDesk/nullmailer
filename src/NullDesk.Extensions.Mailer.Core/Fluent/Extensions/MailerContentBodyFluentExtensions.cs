namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    /// <summary>
    ///     Class MailerContentBodyFluentExtensions.
    /// </summary>
    public static class MailerContentBodyFluentExtensions
    {
        /// <summary>
        ///     Adds an HTML email body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="html">The HTML.</param>
        /// <returns>ContentBody.</returns>
        public static ContentBody WithHtml(this ContentBody body, string html)
        {
            body.HtmlContent = html;
            return body;
        }

        /// <summary>
        ///     Adds a plain text email body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="text">The text.</param>
        /// <returns>ContentBody.</returns>
        public static ContentBody WithPlainText(this ContentBody body, string text)
        {
            body.PlainTextContent = text;
            return body;
        }
    }
}