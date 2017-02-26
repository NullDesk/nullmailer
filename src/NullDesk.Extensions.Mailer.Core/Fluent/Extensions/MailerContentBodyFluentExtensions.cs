namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    public static class MailerContentBodyFluentExtensions
    {
        public static ContentBody WithHtml(this ContentBody body, string html)
        {
            body.HtmlContent = html;
            return body;
        }

        public static ContentBody WithPlainText(this ContentBody body, string text)
        {
            body.PlainTextContent = text;
            return body;
        }
    }
}
