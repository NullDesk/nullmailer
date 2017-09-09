using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class MailerServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds the mailer system to dependency injection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<T, TSettings>
        (
            this IServiceCollection services,
            TSettings settings
        )
            where T : class, IMailer<TSettings>
            where TSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(settings));

            return services.TrySetupMailer<T>();
        }

        /// <summary>
        ///     Adds the mailer system to dependency injection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<T, TSettings>
        (
            this IServiceCollection services,
            Func<IServiceProvider, TSettings> settingsFactory
        )
            where T : class, IMailer<TSettings>
            where TSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(settingsFactory));

            return services.TrySetupMailer<T>();
        }

        private static IServiceCollection TrySetupMailer<T>(this IServiceCollection services)
            where T : class, IMailer
        {
            services.Add(ServiceDescriptor.Transient<T, T>());
            if (typeof(T).GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(IMailer)) &&
                services.All(d => d.ServiceType != typeof(IMailer)))
            {
                services.Add(ServiceDescriptor.Transient(typeof(IMailer), typeof(T)));
            }
            return services;
        }

        /// <summary>
        ///     Adds the NullMailer ystem to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddNullMailer
        (
            this IServiceCollection services,
            NullMailerSettings settings
        )
        {
            return services.AddMailer<NullMailer, NullMailerSettings>(settings);
        }

        /// <summary>
        ///     Adds the NullMailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddNullMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, NullMailerSettings> settingsFactory
        )
        {
            return services.AddMailer<NullMailer, NullMailerSettings>(settingsFactory);
        }

        /// <summary>
        ///     Adds the NullHistoryStore to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerNullHistory(this IServiceCollection services)
        {
            return services.AddSingleton<IHistoryStore>(s => NullHistoryStore.Instance);
        }
    }
}