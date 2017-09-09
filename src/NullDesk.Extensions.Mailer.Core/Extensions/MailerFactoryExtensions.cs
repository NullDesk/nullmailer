using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace NullDesk.Extensions.Mailer.Core.Extensions
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
        public static void AddNullMailer(
            this MailerFactory factory,
            NullMailerSettings mailerSettings,
            ILogger<NullMailer> logger = null,
            IHistoryStore store = null)
        {

            factory.Register<NullMailer, NullMailerSettings>(mailerSettings,
                logger ?? factory.DefaultLoggerFactory?.CreateLogger<NullMailer>(),
                factory.ConfigureHistoryStoreLogger(store));
        }


    }
}