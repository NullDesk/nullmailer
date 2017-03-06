using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Mailkit SMTP mailer settings.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.SmtpMailerSettings" />
    public class MkSmtpMailerSettings : SmtpMailerSettings
    {
        /// <summary>
        ///     Gets or sets the template settings.
        /// </summary>
        /// <value>The template settings.</value>
        public MkFileTemplateSettings TemplateSettings { get; set; }

        /// <summary>
        ///     Gets or sets the authentication settings.
        /// </summary>
        /// <value>The authentication settings.</value>
        public MkSmtpAuthenticationSettings AuthenticationSettings { get; set; }
    }
}