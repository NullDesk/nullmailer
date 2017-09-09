using System;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.MailKit;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {
        /// <summary>
        ///     Register a MailKit mailer with the factory.
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
                    mailerSettings,
                    logger ?? factory.DefaultLoggerFactory?.CreateLogger<MkSmtpMailer>(),
                    store ?? factory.DefaultHistoryStore));
        }

        /// <summary>
        ///     Register a MailKit mailer with the factory.
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
                    mailerSettings,
                    logger ?? factory.DefaultLoggerFactory?.CreateLogger<MkSmtpMailer>(),
                    store ?? factory.DefaultHistoryStore));
        }
    }
}