using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        ///     Adds a SendGrid mailer with the factory.
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
        ///     Adds a SendGrid mailer with the factory.
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