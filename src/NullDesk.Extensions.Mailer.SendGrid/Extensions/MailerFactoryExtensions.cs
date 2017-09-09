using System;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.SendGrid;
using SendGrid;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {
        /// <summary>
        ///     Register a SendGrid mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSendGridMailer(
            this MailerFactory factory,
            SendGridMailerSettings settings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SendGridMailer(
                settings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<SendGridMailer>(),
                store ?? factory.DefaultHistoryStore));
        }

        /// <summary>
        ///     Register a SendGrid mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="clientFunc">The client function.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSendGridMailer(
            this MailerFactory factory,
            Func<SendGridClient> clientFunc,
            SendGridMailerSettings settings,
            ILogger<SendGridMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SendGridMailer(
                clientFunc(),
                settings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<SendGridMailer>(),
                store ?? factory.DefaultHistoryStore));
        }
    }
}