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
        ///     Require SSL connection.
        /// </summary>
        /// <remarks>When <c>false</c>, SSL may still be used if the SMTP server indicates that it supports it.</remarks>
        /// <value><c>true</c> if using SSL; otherwise, <c>false</c>.</value>
        public bool SmtpRequireSsl { get; set; } = false;

        /// <summary>
        ///     Indicates if validation for server SSL certificates is used, set <c>false</c> for untrusted or self-signed
        ///     certificates.
        /// </summary>
        /// <value><c>true</c> if [disable SSL server certificate validation]; otherwise, <c>false</c>.</value>
        public bool EnableSslServerCertificateValidation { get; set; } = true;

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