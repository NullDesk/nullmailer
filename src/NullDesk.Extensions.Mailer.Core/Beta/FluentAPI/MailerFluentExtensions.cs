// ReSharper disable CheckNamespace
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Mailer Fluent API.
    /// </summary>
    public static class MailerFluentExtensions
    {
        /// <summary>
        /// Creates a message and adds it to the list of pending messages tracked by the mailer.
        /// </summary>
        /// <param name="mailer">The mailer instance.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage CreateMessage(this IMailer mailer)
        {
            var message = MailerMessage.Create();
            mailer.Messages.Add(message);
            return message;
        }

        /// <summary>
        /// Creates a message and adds it to the list of pending message tracked by the mailer.
        /// </summary>
        /// <param name="mailer">The mailer.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage CreateMessage(this IMailer mailer, string templateName)
        {
            var message = MailerMessage.Create(templateName);
            mailer.Messages.Add(message);
            return message;
        }
    }
}
