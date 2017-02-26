using System;
using System.Collections.Generic;
using System.IO;

namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    /// <summary>
    /// Fluent mailer API.
    /// </summary>
    public static class MailerMessageFluentExtensions
    {
        /// <summary>
        /// Creates a body of the specified type and adds it to the message.
        /// </summary>
        /// <typeparam name="T">The body type</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="bodyAction">Configure the new message body.</param>
        /// <returns>MailerContentMessage.</returns>
        public static MailerMessage WithBody<T>(
            this MailerMessage message, Action<T> bodyAction) where T : class, IMessageBody
        {
            var body = Activator.CreateInstance<T>();
            bodyAction(body);
            message.Body = body;
            return message;
        }

        /// <summary>
        /// Adds the specified body to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="body">The body.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithBody(this MailerMessage message, IMessageBody body)
        {
            message.Body = body;
            return message;
        }

        ///// <summary>
        ///// Adds the template name to the message, will convert MailerContentMessage to MailerTemplateMessage if necessary.
        ///// </summary>
        ///// <param name="message">The message.</param>
        ///// <param name="templateName">Name of the template.</param>
        ///// <returns>MailerTemplateMessage.</returns>
        //public static MailerMessage WithBody(this MailerMessage message, string templateName)
        //{
        //    message.Body = new TemplateBody { TemplateName = "templateName" };
        //    return message;
        //}

        /// <summary>
        /// Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage From(this MailerMessage message, Action<MailerReplyTo> replyTo)
        {
            message.From = new MailerReplyTo();
            replyTo(message.From);
            return message;
        }

        /// <summary>
        /// Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage From(this MailerMessage message, MailerReplyTo sender)
        {
            message.From = sender;
            return message;
        }

        /// <summary>
        /// Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage From(this MailerMessage message, string emailAddress, string displayName = null)
        {
            message.From = new MailerReplyTo() { EmailAddress = emailAddress, DisplayName = displayName };
            return message;
        }


        /// <summary>
        /// Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipientAction">The recipient action.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, Action<MailerRecipient> recipientAction)
        {
            var newRecipient = new MailerRecipient();
            recipientAction(newRecipient);
            message.Recipients.Add(newRecipient);
            return message;
        }

        /// <summary>
        /// Adds a recipient to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipient">The recipient.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, MailerRecipient recipient)
        {
            message.Recipients.Add(recipient);
            return message;
        }

        /// <summary>
        /// Adds a recipient to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="personalizedSubstitutions">The personalized substitutions.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, string emailAddress, string displayName = null, IDictionary<string, string> personalizedSubstitutions = null)
        {
            var recipient = new MailerRecipient()
            {
                EmailAddress = emailAddress,
                DisplayName = displayName,
                PersonalizedSubstitutions = personalizedSubstitutions ?? new Dictionary<string, string>()
            };
            message.Recipients.Add(recipient);
            return message;
        }

        /// <summary>
        /// Adds a collection of recipients the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, IEnumerable<MailerRecipient> recipients)
        {
            foreach (var recipient in recipients)
            {
                message.Recipients.Add(recipient);
            }
            return message;
        }

        /// <summary>
        /// Adds an attachment to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <param name="attachmentContent">Content of the attachment.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage WithAttachment(this MailerMessage message, string attachmentName, Stream attachmentContent)
        {
            message.Attachments.Add(attachmentName, attachmentContent);
            return message;
        }

        /// <summary>
        /// Adds an attachment to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage WithAttachment(this MailerMessage message, string fileName)
        {
            message.Attachments.Add(fileName.GetAttachmentStreamForFile());
            return message;
        }

        /// <summary>
        /// Adds a collection of attachments to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage WithAttachments(this MailerMessage message, IDictionary<string, Stream> attachments)
        {
            foreach (var attachment in attachments)
            {
                message.Attachments.Add(attachment);
            }
            return message;
        }

        /// <summary>
        /// Adds a collection of attachments to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage WithAttachments(this MailerMessage message, IEnumerable<string> attachments)
        {
            foreach (var attachment in attachments)
            {
                message.Attachments.Add(attachment.GetAttachmentStreamForFile());
            }
            return message;
        }

        /// <summary>
        /// Adds a subject for the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage WithSubject(this MailerMessage message, string subject)
        {
            message.Subject = subject;
            return message;
        }

        /// <summary>
        /// Adds one or more replacement variables to the message.
        /// </summary>
        /// <remarks>
        /// Substitutions can be applied to the subject, html body, text body, and templates</remarks>
        /// <param name="message">The message.</param>
        /// <param name="substitutions">The substitutions.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MailerMessage WithSubstitutions(this MailerMessage message, IDictionary<string, string> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                message.Substitutions.Add(substitution);
            }
            return message;
        }

        /// <summary>
        /// Adds a replacement variable to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The token to replace.</param>
        /// <param name="value">The value to substitue for the token.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        /// <remarks>Substitutions can be applied to the subject, html body, text body, and templates</remarks>
        public static MailerMessage WithSubstitution(this MailerMessage message, string key, string value)
        {
            message.Substitutions.Add(new KeyValuePair<string, string>(key, value));

            return message;
        }
    }
}