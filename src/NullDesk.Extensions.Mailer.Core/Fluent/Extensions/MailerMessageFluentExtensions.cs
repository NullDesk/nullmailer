using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    /// <summary>
    ///     Fluent mailer API.
    /// </summary>
    public static class MailerMessageFluentExtensions
    {
        /// <summary>
        ///     Creates a body of the specified type and adds it to the message.
        /// </summary>
        /// <typeparam name="T">The body type</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="bodyAction">Configure the new message body.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithBody<T>(
            this MailerMessage message, Action<T> bodyAction) where T : class, IMessageBody
        {
            var body = Activator.CreateInstance<T>();
            bodyAction(body);
            message.Body = body;
            return message;
        }

        /// <summary>
        ///     Adds the specified body to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="body">The body.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithBody(this MailerMessage message, IMessageBody body)
        {
            message.Body = body;
            return message;
        }

        /// <summary>
        ///     Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The action to setup the sender.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage From(this MailerMessage message, Action<MessageSender> sender)
        {
            message.From = new MessageSender();
            sender(message.From);
            return message;
        }

        /// <summary>
        ///     Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage From(this MailerMessage message, MessageSender sender)
        {
            message.From = sender;
            return message;
        }

        /// <summary>
        ///     Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="replyToEmailAddress">The reply to email address.</param>
        /// <param name="replyToDisplayName">Display name of the reply to.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage From(this MailerMessage message, string emailAddress, string displayName = null,
            string replyToEmailAddress = null, string replyToDisplayName = null)
        {
            message.From = new MessageSender
            {
                EmailAddress = emailAddress,
                DisplayName = displayName,
                ReplyToEmailAddress = replyToEmailAddress,
                ReplyToDisplayName = replyToDisplayName
            };
            return message;
        }

        /// <summary>
        ///     Add the sender's info to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipientAction">The recipient action.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, Action<MessageRecipient> recipientAction)
        {
            var newRecipient = new MessageRecipient();
            recipientAction(newRecipient);
            message.Recipients.Add(newRecipient);
            return message;
        }

        /// <summary>
        ///     Adds a recipient to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipient">The recipient.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, MessageRecipient recipient)
        {
            message.Recipients.Add(recipient);
            return message;
        }

        /// <summary>
        ///     Adds a recipient to the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="personalizedSubstitutions">The personalized substitutions.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, string emailAddress, string displayName = null,
            IDictionary<string, string> personalizedSubstitutions = null)
        {
            var recipient = new MessageRecipient
            {
                EmailAddress = emailAddress,
                DisplayName = displayName,
                PersonalizedSubstitutions = personalizedSubstitutions ?? new Dictionary<string, string>()
            };
            message.Recipients.Add(recipient);
            return message;
        }

        /// <summary>
        ///     Adds a collection of recipients the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage To(this MailerMessage message, IEnumerable<MessageRecipient> recipients)
        {
            foreach (var recipient in recipients)
            {
                message.Recipients.Add(recipient);
            }

            return message;
        }

        /// <summary>
        ///     Adds an attachment to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <param name="attachmentContent">Content of the attachment.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithAttachment(this MailerMessage message, string attachmentName,
            Stream attachmentContent)
        {
            message.Attachments.Add(attachmentName, attachmentContent);
            return message;
        }

        /// <summary>
        ///     Adds an attachment to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithAttachment(this MailerMessage message, string fileName)
        {
            message.Attachments.Add(fileName.GetAttachmentStreamForFile());
            return message;
        }

        /// <summary>
        ///     Adds a collection of attachments to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithAttachments(this MailerMessage message, IDictionary<string, Stream> attachments)
        {
            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }

            return message;
        }

        /// <summary>
        ///     Adds a collection of attachments to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithAttachments(this MailerMessage message, IEnumerable<string> attachments)
        {
            foreach (var attachment in attachments ?? new string[0])
            {
                message.Attachments.Add(attachment.GetAttachmentStreamForFile());
            }

            return message;
        }

        /// <summary>
        ///     Adds a subject for the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>MailerMessage.</returns>
        public static MailerMessage WithSubject(this MailerMessage message, string subject)
        {
            message.Subject = subject;
            return message;
        }

        /// <summary>
        ///     Adds one or more replacement variables to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="substitutions">The substitutions.</param>
        /// <returns>MailerMessage.</returns>
        /// <remarks>Substitutions can be applied to the subject, html body, text body, and templates</remarks>
        public static MailerMessage WithSubstitutions(this MailerMessage message,
            IDictionary<string, string> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                message.Substitutions.Add(substitution);
            }

            return message;
        }

        /// <summary>
        ///     Adds a replacement variable to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The token to replace.</param>
        /// <param name="value">The value to substitue for the token.</param>
        /// <returns>MailerMessage.</returns>
        /// <remarks>Substitutions can be applied to the subject, html body, text body, and templates</remarks>
        public static MailerMessage WithSubstitution(this MailerMessage message, string key, string value)
        {
            message.Substitutions.Add(new KeyValuePair<string, string>(key, value));

            return message;
        }
    }
}