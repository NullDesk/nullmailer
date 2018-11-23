using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Factory class for obtaining mailer instances.
    /// </summary>
    public class MailerFactory : IMailerFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MailerFactory" /> class.
        /// </summary>
        /// <param name="defaultLoggerFactory">The default logger factory.</param>
        /// <param name="defaultHistoryStore">The default history store.</param>
        public MailerFactory(ILoggerFactory defaultLoggerFactory = null, IHistoryStore defaultHistoryStore = null)
        {
            DefaultLoggerFactory = defaultLoggerFactory;

            DefaultHistoryStore = defaultHistoryStore ?? NullHistoryStore.Instance;
        }

        /// <summary>
        ///     The default history store to use when no history store is supplied for registrations.
        /// </summary>
        /// <value>The default history store.</value>
        public IHistoryStore DefaultHistoryStore { get; }

        /// <summary>
        ///     The default logger factory to use when no loggers are supplied for registrations.
        /// </summary>
        /// <value>The default logger factory.</value>
        public ILoggerFactory DefaultLoggerFactory { get; }

        /// <summary>
        ///     Gets a collection of registered mailer functions.
        /// </summary>
        /// <value>The mailers.</value>
        public List<Func<IMailer>> MailerRegistrations { get; } = new List<Func<IMailer>>();

        /// <summary>
        /// Gets an instance of the first registered mailer or proxy.
        /// </summary>
        /// <returns>IMailer.</returns>
        public virtual IMailer GetMailer()
        {
            var func = MailerRegistrations.FirstOrDefault(m => m
                                                                   .GetType()
                                                                   .GetTypeInfo()
                                                                   .GenericTypeArguments
                                                                   .FirstOrDefault()
                                                                   ?
                                                                   .GetTypeInfo()
                                                                   .ImplementedInterfaces.Any(
                                                                       i => i == typeof(IMailer)) ?? false);
            return func?.Invoke();
        }

        /// <summary>
        ///     Registers a function that can be use to create a configured mailer instance.
        /// </summary>
        /// <typeparam name="TMailer"></typeparam>
        /// <param name="mailerFunc">The mailer function.</param>
        public virtual void Register<TMailer>(Func<TMailer> mailerFunc) where TMailer : class, IMailer
        {
            MailerRegistrations.Add(mailerFunc);
        }

        /// <summary>
        ///     Gets an instance of a registered mailer for the specified type.
        /// </summary>
        /// <remarks>
        ///     If more than one function for the mailer type exists, will use the first matching function.
        ///     Will return the first function for either the specified type, or a proxy of the specified type.
        /// </remarks>
        /// <typeparam name="TMailer">The type of mailer instance you wish to create</typeparam>
        /// <returns>IMailer</returns>
        public virtual IMailer GetMailer<TMailer>() where TMailer : class, IMailer
        {
            var mailer =
                MailerRegistrations.FirstOrDefault(
                    m =>
                        m.GetType().GenericTypeArguments.FirstOrDefault()?.AssemblyQualifiedName ==
                        typeof(TMailer).AssemblyQualifiedName ||
                        m.GetType().GenericTypeArguments.FirstOrDefault()?.GenericTypeArguments.FirstOrDefault()
                            ?.AssemblyQualifiedName == typeof(TMailer).AssemblyQualifiedName);
            return mailer?.Invoke();
        }

        /// <summary>
        ///     Registers the specified mailer type.
        /// </summary>
        /// <typeparam name="TMailer"></typeparam>
        /// <typeparam name="TMailerSettings">The type of the t settings.</typeparam>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        public virtual void Register<TMailer, TMailerSettings>(TMailerSettings settings, ILogger logger = null,
            IHistoryStore store = null)
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            Register(() =>
            {
                var ctor = typeof(TMailer).GetConstructor(new[] {typeof(TMailerSettings), typeof(ILogger), typeof(IHistoryStore)});
                return (TMailer) ctor?.Invoke(
                    new object[]
                    {
                        settings,
                        logger ?? DefaultLoggerFactory?.CreateLogger(typeof(TMailer)) ?? NullLogger.Instance,
                        ConfigureHistoryStoreLogger(store)
                    });
            });
        }

        /// <summary>
        /// Registers the specified mailer type using a safety mailer proxy.
        /// </summary>
        /// <typeparam name="TProxy">The type of the t proxy.</typeparam>
        /// <typeparam name="TProxySettings">The type of the t proxy settings.</typeparam>
        /// <typeparam name="TMailer">The type of the t mailer.</typeparam>
        /// <typeparam name="TMailerSettings">The type of the t settings.</typeparam>
        /// <param name="settings">The settings.</param>
        /// <param name="proxySettings">The proxy settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        public virtual void RegisterProxy<TProxy, TProxySettings, TMailer, TMailerSettings>(
            IProxyMailerSettings proxySettings,
            TMailerSettings settings,
            ILogger logger = null,
            IHistoryStore store = null)
            where TProxy : class, IProxyMailer<TProxySettings, TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            Register(() =>
            {
                var mailerCtor = typeof(TMailer).GetConstructor(new[]
                {
                    typeof(TMailerSettings),
                    typeof(ILogger), typeof(IHistoryStore)
                });
                var realMailer = (TMailer)mailerCtor?.Invoke(
                    new object[]
                    {
                        settings,
                        logger ?? DefaultLoggerFactory?.CreateLogger(typeof(TMailer)) ?? NullLogger.Instance,
                        ConfigureHistoryStoreLogger(store)
                    });
                var proxyCtor = typeof(TProxy).GetConstructor(new[]
                {
                    typeof(TMailer),
                    typeof(TProxySettings)
                });
                return (TProxy) proxyCtor?.Invoke(
                    new object[]
                    {
                        realMailer,
                        proxySettings
                    });
            });
        }

        /// <summary>
        ///     Configures the history store logger.
        /// </summary>
        /// <param name="historyStore">The history store.</param>
        /// <returns>IHistoryStore.</returns>
        public IHistoryStore ConfigureHistoryStoreLogger(IHistoryStore historyStore)
        {
            historyStore = historyStore ?? DefaultHistoryStore;
            if (DefaultLoggerFactory != null && historyStore?.Logger is NullLogger)
            {
                historyStore.Logger = DefaultLoggerFactory.CreateLogger(historyStore.GetType());
            }

            return historyStore;
        }
    }
}