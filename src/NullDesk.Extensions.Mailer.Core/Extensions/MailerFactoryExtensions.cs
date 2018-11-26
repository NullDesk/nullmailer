using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {
        /// <summary>
        ///     Registers a null mailer with the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        public static void AddNullMailer
        (
            this MailerFactory factory,
            NullMailerSettings mailerSettings,
            ILogger<NullMailer> logger = null,
            IHistoryStore store = null
        )
        {
            factory.Register<NullMailer, NullMailerSettings>(mailerSettings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<NullMailer>(),
                factory.ConfigureHistoryStoreLogger(store));
        }

        /// <summary>
        ///     Registers a safety mailer proxy for the specified mailer type.
        /// </summary>
        /// <typeparam name="TMailer">The type of the t mailer.</typeparam>
        /// <typeparam name="TMailerSettings">The type of the t mailer settings.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        public static void AddSafetyMailer<TMailer, TMailerSettings>
        (
            this MailerFactory factory,
            SafetyMailerSettings safetyMailerSettings,
            TMailerSettings mailerSettings,
            ILogger<NullMailer> logger = null,
            IHistoryStore store = null
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            factory.Register<SafetyMailer<TMailer>, SafetyMailerSettings, TMailer, TMailerSettings>(
                safetyMailerSettings,
                mailerSettings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<NullMailer>(),
                factory.ConfigureHistoryStoreLogger(store));
        }
    }
}