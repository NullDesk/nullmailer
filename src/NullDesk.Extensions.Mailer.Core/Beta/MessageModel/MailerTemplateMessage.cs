// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// A mail message where external templates provide the message's contents.
    /// </summary>
    /// <seealso cref="MailerMessage" />
    public class MailerTemplateMessage : MailerMessage
    {
        /// <summary>
        /// The name of the template to send.
        /// </summary>
        /// <value>The template.</value>
        public string TemplateName { get; set; }

    }
}