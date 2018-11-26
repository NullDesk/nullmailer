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
        /// <param name="mailerSettings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            TMailerSettings mailerSettings
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(mailerSettings));

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
        /// <param name="mailerSettings">The settings.</param>
        /// <param name="proxyMailerSettings">The proxy mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the Proxy as the default IMailer</remarks>
        public static IServiceCollection AddMailer<TProxy, TProxySettings, TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            TMailerSettings mailerSettings,
            TProxySettings proxyMailerSettings
        )
            where TProxy : class, IProxyMailer<TProxySettings, TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(mailerSettings));
            services.Add(ServiceDescriptor.Singleton(proxyMailerSettings));
            return services.TrySetupProxyMailer<TProxy, TProxySettings, TMailer>();
        }

        /// <summary>
        ///     Adds the mailer system to dependency injection.
        /// </summary>
        /// <typeparam name="TMailer"></typeparam>
        /// <typeparam name="TMailerSettings">The type of settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettingsFactory">A func that returns the settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            Func<IServiceProvider, TMailerSettings> mailerSettingsFactory
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(mailerSettingsFactory));

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
        /// <param name="mailerSettingsFactory">A func that returns the settings.</param>
        /// <param name="proxyMailerSettingsFactory">A func that returns the proxy mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMailer<TProxy, TProxySettings, TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            Func<IServiceProvider, TMailerSettings> mailerSettingsFactory,
            Func<IServiceProvider, TProxySettings>  proxyMailerSettingsFactory
        )
            where TProxy : class, IProxyMailer<TProxySettings, TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            services.Add(ServiceDescriptor.Singleton(mailerSettingsFactory));
            services.Add(ServiceDescriptor.Singleton(proxyMailerSettingsFactory));
            return services.TrySetupProxyMailer<TProxy, TProxySettings, TMailer>();
        }

        private static IServiceCollection TrySetupProxyMailer<TProxy, TProxySettings, TMailer>(this IServiceCollection services)
            where TProxy : class, IProxyMailer<TProxySettings,TMailer>, IMailer
            where TProxySettings : class, IProxyMailerSettings
            where TMailer : class, IMailer
        {
            services.Add(ServiceDescriptor.Transient<TProxy, TProxy>());
            services.Add(ServiceDescriptor.Transient<TMailer, TMailer>());
            services.Add(ServiceDescriptor.Transient(typeof(IMailer), typeof(TProxy)));
            return services;
        }

        private static IServiceCollection TrySetupMailer<TMailer>(this IServiceCollection services)
            where TMailer : class, IMailer
        {
            services.Add(ServiceDescriptor.Transient<TMailer, TMailer>());
            services.Add(ServiceDescriptor.Transient(typeof(IMailer), typeof(TMailer)));
            return services;
        }

        /// <summary>
        ///     Adds the NullMailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddNullMailer
        (
            this IServiceCollection services,
            NullMailerSettings mailerSettings
        )
        {
            return services.AddMailer<NullMailer, NullMailerSettings>(mailerSettings);
        }

        /// <summary>
        ///     Adds the NullMailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettingsFactory">A func that returns the mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddNullMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, NullMailerSettings> mailerSettingsFactory
        )
        {
            return services.AddMailer<NullMailer, NullMailerSettings>(mailerSettingsFactory);
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

        /// <summary>
        /// Adds the safety mailer proxy for an IMailer to dependency injection.
        /// </summary>
        /// <typeparam name="TMailer">The type of the t mailer.</typeparam>
        /// <typeparam name="TMailerSettings">The type of the t mailer settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer<TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            SafetyMailerSettings safetyMailerSettings,
            TMailerSettings mailerSettings
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            return AddMailer<SafetyMailer<TMailer>, SafetyMailerSettings, TMailer, TMailerSettings>(
                services,
                mailerSettings,
                safetyMailerSettings);
        }


        /// <summary>
        /// Adds the safety mailer proxy for an IMailer to dependency injection.
        /// </summary>
        /// <typeparam name="TMailer">The type of the t mailer.</typeparam>
        /// <typeparam name="TMailerSettings">The type of the t mailer settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettingsFactory">The safety mailer settings function.</param>
        /// <param name="mailerSettingsFactory">The mailer settings function.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer<TMailer, TMailerSettings>
        (
            this IServiceCollection services,
            Func<IServiceProvider, SafetyMailerSettings> safetyMailerSettingsFactory,
            Func<IServiceProvider, TMailerSettings> mailerSettingsFactory
        )
            where TMailer : class, IMailer<TMailerSettings>
            where TMailerSettings : class, IMailerSettings
        {
            return AddMailer<SafetyMailer<TMailer>, SafetyMailerSettings, TMailer, TMailerSettings>(
                services,
                mailerSettingsFactory,
                safetyMailerSettingsFactory);
        }

        /// <summary>
        /// Adds the safety mailer proxy for a NullMailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer
        (
            this IServiceCollection services,
            SafetyMailerSettings safetyMailerSettings,
            NullMailerSettings mailerSettings
        )
        {
            return AddMailer<SafetyMailer<NullMailer>, SafetyMailerSettings, NullMailer, NullMailerSettings>(
                services,
                mailerSettings,
                safetyMailerSettings);
        }

        /// <summary>
        /// Adds the safety mailer proxy for a NullMailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettingsFactory">The safety mailer settings function.</param>
        /// <param name="mailerSettingsFactory">The mailer settings function.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, SafetyMailerSettings> safetyMailerSettingsFactory,
            Func<IServiceProvider, NullMailerSettings> mailerSettingsFactory
        )
        {
            return AddMailer<SafetyMailer<NullMailer>, SafetyMailerSettings, NullMailer, NullMailerSettings>(
                services,
                mailerSettingsFactory,
                safetyMailerSettingsFactory);
        }
    }
}