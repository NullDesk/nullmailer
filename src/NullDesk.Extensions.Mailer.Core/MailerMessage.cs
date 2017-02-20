using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Represents an email message compatible with all NullDesk Mailers.
    /// </summary>
    public abstract class MailerMessage
    {
        /// <summary>
        /// The reply to information for the message.
        /// </summary>
        /// <value>From.</value>
        public MailerReplyTo From { get; set; }

        /// <summary>
        /// The message recipients.
        /// </summary>
        /// <value>The recipients.</value>
        public IEnumerable<MailerRecipient> Recipients { get; set; }

        /// <summary>
        /// The message subject.
        /// </summary>
        /// <remarks>
        /// If substitutions are provided, they will be used here. Some services may ignore this value when using templates, others will use this value in place of any subject defined in the template.
        /// </remarks>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>
        /// A collection of tokens and replacement values to use with body contents, templates, and message subject.
        /// </summary>
        /// <remarks>
        /// To override any of these values on a per-recipient basis, supply overriding values to the recipients' PersonalizedSubstitutions property
        /// </remarks>
        /// <value>The substitution tokens and replacement values.</value>
        public IDictionary<string, string> Substitutions { get; set; }

        /// <summary>
        /// A collection of attachments to include with the message.
        /// </summary>
        /// <value>The attachments.</value>
        public IDictionary<string, Stream> Attachments { get; set; }
    }
}
