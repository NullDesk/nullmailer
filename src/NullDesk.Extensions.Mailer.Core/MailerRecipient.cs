using System;
using System.Collections.Generic;
using System.Text;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// The email recipient's address and information
    /// </summary>
    public class MailerRecipient : IMailerAddress
    {
        /// <summary>
        /// Gets or sets the recipient's email address.
        /// </summary>
        /// <value>The email address.</value>
        public string EmailAddress { get; set; }


        /// <summary>
        /// Gets or sets the display name for the recipient.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Optional collection of template substitution variables specific to the recipient.
        /// </summary>
        /// <remarks>
        /// When sending email, the values specified here will be merged with, and override, any replacement variables defined on for message as a whole. Use this to supply replacment substitutions that vary from one recipient to another. 
        /// </remarks>
        /// <value>Template substitutions to use for this recipient only.</value>
        public IDictionary<string, string> PersonalizedSubstitutions { get; set; }
    }
}
