using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    /// Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {

        /// <summary>
        /// Adds a MailKit simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public static void AddMkSimpleSmtpMailer(
            this MailerFactory factory,
            MkSmtpMailerSettings settings,
            ILogger<MkSimpleSmtpMailer> logger = null)
        {
            factory.Register(() =>
                new MkSimpleSmtpMailer(
                    new OptionsWrapper<MkSmtpMailerSettings>(settings),
                    logger));
        }

        /// <summary>
        /// Adds a MailKit simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="client">The SMTP client.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public static void AddMkSimpleSmtpMailer(
            this MailerFactory factory,
            SmtpClient client,
            MkSmtpMailerSettings settings,
            ILogger<MkSimpleSmtpMailer> logger = null)
        {
            factory.Register(() =>
                new MkSimpleSmtpMailer(
                    client,
                    new OptionsWrapper<MkSmtpMailerSettings>(settings),
                    logger));
        }

        /// <summary>
        /// Adds a MailKit mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public static void AddMkSmtpMailer(
            this MailerFactory factory,
            MkSmtpMailerSettings mailerSettings,
            ILogger<MkSmtpMailer> logger = null)
        {
            factory.Register(() =>
                new MkSmtpMailer(
                    new OptionsWrapper<MkSmtpMailerSettings>(mailerSettings),
                    logger));
        }

        /// <summary>
        /// Adds a MailKit mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="client">The SMTP client.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public static void AddMkSmtpMailer(
            this MailerFactory factory,
            SmtpClient client,
            MkSmtpMailerSettings mailerSettings,
            ILogger<MkSmtpMailer> logger = null)
        {
            factory.Register(() =>
                new MkSmtpMailer(
                    client,
                    new OptionsWrapper<MkSmtpMailerSettings>(mailerSettings),
                    logger));

        }
    }
}
