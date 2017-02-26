
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// A message body that uses an external template.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.IMessageBody" />
    public class TemplateBody : IMessageBody
    {
        /// <summary>
        /// The name of the template to send.
        /// </summary>
        /// <value>The template.</value>
        public string TemplateName { get; set; }
    }
}
