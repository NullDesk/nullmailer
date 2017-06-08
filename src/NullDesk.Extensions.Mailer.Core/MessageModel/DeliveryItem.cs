using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Flattened mailer message for a single recipient with delivery meta-data.
    /// </summary>
    public class DeliveryItem : DeliverySummary
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeliveryItem" /> class.
        /// </summary>
        public DeliveryItem()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeliveryItem" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipient">The recipient.</param>
        public DeliveryItem(MailerMessage message, MessageRecipient recipient)
        {
            FromEmailAddress = message?.From?.EmailAddress;
            FromDisplayName = message?.From?.DisplayName;
            ReplyToEmailAddress = message?.From?.ReplyToEmailAddress;
            ReplyToDisplayName = message?.From?.ReplyToDisplayName;
            ToEmailAddress = recipient?.EmailAddress;
            ToDisplayName = recipient?.DisplayName;
            Subject = message?.Subject;
            Body = message?.Body;
            Attachments = message?.Attachments;

            //merge personalized and general subs
            Substitutions = new Dictionary<string, string>(recipient?.PersonalizedSubstitutions);
            foreach (var gSub in message?.Substitutions?.Where(s => !Substitutions.ContainsKey(s.Key)) ??
                                 new Dictionary<string, string>())
            {
                Substitutions.Add(gSub.Key, gSub.Value);
            }
        }

        /// <summary>
        ///     Gets or sets the message body.
        /// </summary>
        /// <value>The body.</value>
        [JsonConverter(typeof(MessageBodyJsonConverter))]
        public virtual IMessageBody Body { get; set; }


        /// <summary>
        ///     A collection of attachments to include with the message.
        /// </summary>
        /// <value>The attachments.</value>
        [JsonConverter(typeof(AttachmentStreamJsonConverter))]
        public virtual IDictionary<string, Stream> Attachments { get; set; }


        /// <summary>
        ///     A collection of tokens and replacement values to use with body contents, templates, and message subject.
        /// </summary>
        /// <remarks>
        ///     This collection contains the merged dictionary of personalized substitutions and general message substitutions.
        /// </remarks>
        /// <value>The substitutions.</value>
        public virtual IDictionary<string, string> Substitutions { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is resendable.
        /// </summary>
        /// <value><c>true</c> if this instance is resendable; otherwise, <c>false</c>.</value>
        public virtual bool IsResendable => !Attachments.Any() || Attachments.All(a => a.Value?.Length > 0);


        /// <summary>
        ///     Get the specified content after processing this instance's substitutions.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.String.</returns>
        public virtual string ProcessSubstitutions(string content)
        {
            return content?.PerformContentSubstitution(Substitutions);
        }
    }
}