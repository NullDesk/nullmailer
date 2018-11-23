// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Settings for use with SafetyMailer.
    /// </summary>
    public class SafetyMailerSettings: IProxyMailerSettings
    {
        
        /// <summary>
        ///     The safe recipient email address that will be used instead of the messages original recipient email address.
        /// </summary>
        /// <value>The safe recipient email address.</value>
        public string SafeRecipientEmailAddress { get; set; }


        /// <summary>
        ///     Text to prepend to the recipient display name when safe recipient address has been overwritten; defaults to
        ///     "(safe)".
        /// </summary>
        /// <value>The prepend display name text.</value>
        public string PrependDisplayNameWithText { get; set; } = "(safe)";
    }
}