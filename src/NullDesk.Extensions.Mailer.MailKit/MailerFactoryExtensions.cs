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
        /// Register a MailKit simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public static void RegisterMkSimpleSmtpMailer(
            this MailerFactory factory,
            MkSmtpMailerSettings settings,
            ILogger<MkSimpleSmtpMailer> logger = null)
        {
            factory.RegisterMailer(() =>
                new MkSimpleSmtpMailer(
                    new OptionsWrapper<MkSmtpMailerSettings>(settings),
                    logger));
        }

        /// <summary>
        /// Register a MailKit simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="client">The SMTP client.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        public static void RegisterMkSimpleSmtpMailer(
            this MailerFactory factory,
            SmtpClient client,
            MkSmtpMailerSettings settings,
            ILogger<MkSimpleSmtpMailer> logger = null)
        {
            factory.RegisterMailer(() =>
                new MkSimpleSmtpMailer(
                    client,
                    new OptionsWrapper<MkSmtpMailerSettings>(settings),
                    logger));
        }

        /// <summary>
        /// Register a MailKit mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="templateSettings">The template settings.</param>
        /// <param name="logger">The logger.</param>
        public static void RegisterMkSmtpMailer(
            this MailerFactory factory,
            MkSmtpMailerSettings mailerSettings,
            FileTemplateMailerSettings templateSettings,
            ILogger<MkSmtpMailer> logger = null)
        {
            factory.RegisterMailer(() =>
                new MkSmtpMailer(
                    new OptionsWrapper<MkSmtpMailerSettings>(mailerSettings),
                    new OptionsWrapper<FileTemplateMailerSettings>(templateSettings),
                    logger));
        }

        /// <summary>
        /// Register a MailKit mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="client">The SMTP client.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="templateSettings">The template settings.</param>
        /// <param name="logger">The logger.</param>
        public static void RegisterSendGridMailer(
            this MailerFactory factory,
            SmtpClient client,
            MkSmtpMailerSettings mailerSettings,
            FileTemplateMailerSettings templateSettings,
            ILogger<MkSmtpMailer> logger = null)
        {
            factory.RegisterMailer(() =>
                new MkSmtpMailer(
                    client,
                    new OptionsWrapper<MkSmtpMailerSettings>(mailerSettings),
                    new OptionsWrapper<FileTemplateMailerSettings>(templateSettings),
                    logger));

        }
    }
}
