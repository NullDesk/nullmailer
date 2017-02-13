using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    /// Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {

        /// <summary>
        /// Adds a SendGrid simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSendGridSimpleMailer(
            this MailerFactory factory,
            SendGridMailerSettings settings,
            ILogger<SendGridSimpleMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SendGridSimpleMailer(
                new OptionsWrapper<SendGridMailerSettings>(settings),
                logger,
                store));
        }

        /// <summary>
        /// Adds a SendGrid simplified mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="clientFunc">The client function.</param>
        /// <param name="settings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The history store.</param>
        public static void AddSendGridSimpleMailer(
            this MailerFactory factory,
            Func<SendGridClient> clientFunc,
            SendGridMailerSettings settings,
            ILogger<SendGridSimpleMailer> logger = null,
            IHistoryStore store = null)
        {
            factory.Register(() => new SendGridSimpleMailer(
                clientFunc(),
                new OptionsWrapper<SendGridMailerSettings>(settings),
                logger,
                store));
        }

        /// <summary>
        /// Adds a SendGrid mailer with the factory.
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
                new OptionsWrapper<SendGridMailerSettings>(settings),
                logger,
                store));
        }

        /// <summary>
        /// Adds a SendGrid mailer with the factory.
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
                new OptionsWrapper<SendGridMailerSettings>(settings),
                logger,
                store));
        }
    }
}
