using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.SendGrid;
using SendGrid;
using System;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {
        /// <summary>
        ///     Register a SendGrid mailer with the factory
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSendGridMailer(
            this MailerFactory factory,
            SendGridMailerSettings mailerSettings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SendGridMailer(
                mailerSettings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<SendGridMailer>(),
                factory.ConfigureHistoryStoreLogger(store)));
        }

        /// <summary>
        ///     Register a SendGrid mailer with the factory
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="clientFunc">The client function.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSendGridMailer(
            this MailerFactory factory,
            Func<SendGridClient> clientFunc,
            SendGridMailerSettings mailerSettings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SendGridMailer(
                clientFunc(),
                mailerSettings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<SendGridMailer>(),
                factory.ConfigureHistoryStoreLogger(store)));
        }

        /// <summary>
        /// Registers a safety mailer proxy for a SendGridMailer.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSafetyMailer(
            this MailerFactory factory,
            SafetyMailerSettings safetyMailerSettings,
            SendGridMailerSettings mailerSettings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SafetyMailer<SendGridMailer>(
                new SendGridMailer
                (
                    mailerSettings,
                    logger ?? factory.DefaultLoggerFactory?.CreateLogger<SendGridMailer>(),
                    factory.ConfigureHistoryStoreLogger(store)
                ),
                safetyMailerSettings));
        }

        /// <summary>
        /// Registers a safety mailer proxy for a SendGridMailer.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="clientFunc">The client function.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        public static void AddSafetyMailer
        (
            this MailerFactory factory,
            SafetyMailerSettings safetyMailerSettings,
            Func<SendGridClient> clientFunc,
            SendGridMailerSettings mailerSettings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore store = null
        )
        {
            factory.Register(() => new SafetyMailer<SendGridMailer>(
                new SendGridMailer
                (
                    clientFunc(),
                    mailerSettings,
                    logger ?? factory.DefaultLoggerFactory?.CreateLogger<SendGridMailer>(),
                    factory.ConfigureHistoryStoreLogger(store)
                ),
                safetyMailerSettings));
        }
    }
}