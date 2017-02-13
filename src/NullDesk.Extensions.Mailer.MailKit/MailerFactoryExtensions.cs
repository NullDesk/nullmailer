using System;
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
        /// <param name="store">The history store.</param>
        public static void AddMkSimpleSmtpMailer(
            this MailerFactory factory,
            MkSmtpMailerSettings settings,
            ILogger<MkSimpleSmtpMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() =>
                new MkSimpleSmtpMailer(
                    new OptionsWrapper<MkSmtpMailerSettings>(settings),
                    logger,
                    store));
        }

        /// <summary>
        /// Adds a MailKit simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="clientFunc">The client function.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddMkSimpleSmtpMailer(
            this MailerFactory factory,
            Func<SmtpClient> clientFunc,
            MkSmtpMailerSettings settings,
            ILogger<MkSimpleSmtpMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() =>
                new MkSimpleSmtpMailer(
                    clientFunc(),
                    new OptionsWrapper<MkSmtpMailerSettings>(settings),
                    logger,
                    store));
        }

        /// <summary>
        /// Adds a MailKit mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddMkSmtpMailer(
            this MailerFactory factory,
            MkSmtpMailerSettings mailerSettings,
            ILogger<MkSmtpMailer> logger = null, 
            IHistoryStore store = null)
        {
            factory.Register(() =>
                new MkSmtpMailer(
                    new OptionsWrapper<MkSmtpMailerSettings>(mailerSettings),
                    logger, 
                    store));
        }

        /// <summary>
        /// Adds a MailKit mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="clientFunc">The client function.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddMkSmtpMailer(
            this MailerFactory factory,
            Func<SmtpClient> clientFunc,
            MkSmtpMailerSettings mailerSettings,
            ILogger<MkSmtpMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() =>
                new MkSmtpMailer(
                    clientFunc(),
                    new OptionsWrapper<MkSmtpMailerSettings>(mailerSettings),
                    logger,
                    store));

        }
    }
}
