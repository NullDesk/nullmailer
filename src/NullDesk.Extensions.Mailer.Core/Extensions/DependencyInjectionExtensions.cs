using System;
using System.Linq;
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
        /// <typeparam name="TMailer"></typeparam>
        /// <typeparam name="TMailerSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            TMailerSettings settings
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(settings));

            return services.TrySetupMailer<TMailer>();
        }

        /// <summary>
        ///     Adds the mailer system to dependency injection for a proxy mailer.
        /// </summary>
        /// <typeparam name="TProxy">The type of the t proxy.</typeparam>
        /// <typeparam name="TProxySettings">The type of the t proxy settings.</typeparam>
        /// <typeparam name="TMailer">The type of the t mailer.</typeparam>
        /// <typeparam name="TMailerSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="proxyMailerSettings">The proxy mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the Proxy as the default IMailer</remarks>
        public static IServiceCollection AddMailer<TProxy, TProxySettings, TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            TMailerSettings settings,
            TProxySettings proxyMailerSettings
        )
            where TProxy : class, IProxyMailer<TProxySettings, TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(settings));
            services.Add(ServiceDescriptor.Singleton(proxyMailerSettings));
            return services.TrySetupProxyMailer<TProxy, TProxySettings, TMailer>();
        }

        /// <summary>
        ///     Adds the mailer system to dependency injection.
        /// </summary>
        /// <typeparam name="TMailer"></typeparam>
        /// <typeparam name="TMailerSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            Func<IServiceProvider, TMailerSettings> settingsFactory
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(settingsFactory));

            return services.TrySetupMailer<TMailer>();
        }

        /// <summary>
        /// Adds the mailer system to dependency injection.
        /// </summary>
        /// <typeparam name="TProxy">The type of the t proxy.</typeparam>
        /// <typeparam name="TProxySettings">The type of the t proxy settings.</typeparam>
        /// <typeparam name="TMailer">The type of the t mailer.</typeparam>
        /// <typeparam name="TMailerSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the settings.</param>
        /// <param name="proxyMailerSettings">The proxy mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<TProxy, TProxySettings, TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            Func<IServiceProvider, TMailerSettings> settingsFactory,
            TProxySettings proxyMailerSettings
        )
            where TProxy : class, IProxyMailer<TProxySettings, TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(settingsFactory));
            services.Add(ServiceDescriptor.Singleton(proxyMailerSettings));

            return services.TrySetupProxyMailer<TProxy, TProxySettings, TMailer>();
        }

        private static IServiceCollection TrySetupProxyMailer<TProxy, TProxySettings, TMailer>(this IServiceCollection services)
            where TProxy : class, IProxyMailer<TProxySettings,TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer
        {
            services.Add(ServiceDescriptor.Transient<TProxy, TProxy>());

            if (services.All(d => d.ServiceType != typeof(IMailer)))
            {
                services.Add(ServiceDescriptor.Transient(typeof(IMailer), typeof(TProxy)));
            }

            return services;
        }

        private static IServiceCollection TrySetupMailer<TMailer>(this IServiceCollection services)
            where TMailer : class, IMailer
        {
            services.Add(ServiceDescriptor.Transient<TMailer, TMailer>());

            if (services.All(d => d.ServiceType != typeof(IMailer)))
            {
                services.Add(ServiceDescriptor.Transient(typeof(IMailer), typeof(TMailer)));
            }

            return services;
        }

        /// <summary>
        ///     Adds the NullMailer to dependency injection.
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
        ///     Adds the SafetyMailer Proxy for NullMailer to dependency injection; uses the safety mailer proxy.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddNullMailer
        (
            this IServiceCollection services,
            NullMailerSettings settings,
            SafetyMailerSettings safetyMailerSettings
        )
        {
            return services.AddMailer<SafetyMailer<NullMailer>, SafetyMailerSettings, NullMailer, NullMailerSettings>(settings, safetyMailerSettings);
        }

        /// <summary>
        ///     Adds the SafetyMailer Proxy for NullMailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the mailer settings.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddNullMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, NullMailerSettings> settingsFactory,
            SafetyMailerSettings safetyMailerSettings
        )
        {
            return services.AddMailer<SafetyMailer<NullMailer>, SafetyMailerSettings, NullMailer, NullMailerSettings>(settingsFactory, safetyMailerSettings);
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