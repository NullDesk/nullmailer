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
        ///     Gets an instance of the first registered standard mailer.
        /// </summary>
        /// <value>The mailer.</value>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="mailerFunc">The mailer function.</param>
        public virtual void Register<T>(Func<T> mailerFunc) where T : class, IMailer
        {
            MailerRegistrations.Add(mailerFunc);
        }

        /// <summary>
        ///     Gets an instance of a registered mailer for the specified type.
        /// </summary>
        /// <remarks>
        ///     If more than one function for the mailer type exists, will use the first matching function.
        /// </remarks>
        /// <typeparam name="T">The type of mailer instance you wish to create</typeparam>
        /// <returns>T.</returns>
        public virtual T GetMailer<T>() where T : class, IMailer
        {
            var mailer =
                MailerRegistrations.FirstOrDefault(
                    m =>
                        m.GetType().GenericTypeArguments.FirstOrDefault()?.AssemblyQualifiedName ==
                        typeof(T).AssemblyQualifiedName);
            return mailer?.Invoke() as T;
        }

        /// <summary>
        ///     Registers the specified mailer type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSettings">The type of the t settings.</typeparam>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        public virtual void Register<T, TSettings>(TSettings settings, ILogger logger = null,
            IHistoryStore store = null)
            where TSettings : class, IMailerSettings
            where T : Mailer<TSettings>
        {
            Register(() =>
            {
                var ctor = typeof(T).GetConstructor(new[] { typeof(TSettings), typeof(ILogger), typeof(IHistoryStore) });
                return (T)ctor.Invoke(
                    new object[]
                    {
                        settings,
                        logger ?? DefaultLoggerFactory?.CreateLogger(typeof(T)) ?? NullLogger.Instance,
                        store ?? DefaultHistoryStore
                    });
            });
        }
    }
}