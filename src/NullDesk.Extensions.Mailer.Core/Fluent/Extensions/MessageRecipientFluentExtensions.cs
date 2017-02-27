using System.Collections.Generic;

namespace NullDesk.Extensions.Mailer.Core.Fluent.Extensions
{
    /// <summary>
    /// MailerRecipient Fluent API.
    /// </summary>
    public static class MessageRecipientFluentExtensions
    {
        /// <summary>
        /// Adds the specified email address to the sender's info.
        /// </summary>
        /// <param name="recipient">The sender.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerReplyTo.</returns>
        public static MessageRecipient ToAddress(this MessageRecipient recipient, string emailAddress)
        {
            recipient.EmailAddress = emailAddress;
            return recipient;
        }

        /// <summary>
        /// Adds a display name to the sender's info.
        /// </summary>
        /// <param name="recipient">The sender.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerReplyTo.</returns>
        public static MessageRecipient WithDisplayName(this MessageRecipient recipient, string displayName)
        {
            recipient.DisplayName = displayName;
            return recipient;
        }

        /// <summary>
        /// Adds one or more personalized replacement variables for the recipient.
        /// </summary>
        /// <remarks>
        /// Substitutions can be applied to the subject, html body, text body, and templates</remarks>
        /// <param name="recipient">The message.</param>
        /// <param name="substitutions">The substitutions.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        public static MessageRecipient WithSubstitutions(this MessageRecipient recipient, IDictionary<string, string> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                recipient.PersonalizedSubstitutions.Add(substitution);
            }
            return recipient;
        }

        /// <summary>
        /// Adds a personalized replacement variable for the recipient.
        /// </summary>
        /// <param name="recipient">The message.</param>
        /// <param name="key">The token to replace.</param>
        /// <param name="value">The value to substitue for the token.</param>
        /// <returns>NullDesk.Extensions.Mailer.Core.Beta.MailerMessage.</returns>
        /// <remarks>Substitutions can be applied to the subject, html body, text body, and templates</remarks>
        public static MessageRecipient WithSubstitution(this MessageRecipient recipient, string key, string value)
        {
            recipient.PersonalizedSubstitutions.Add(new KeyValuePair<string, string>(key, value));

            return recipient;
        }
    }
}