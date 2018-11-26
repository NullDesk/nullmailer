using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.SendGrid
{
    /// <summary>
    ///     Class MailerServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds the SendGrid mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddSendGridMailer
        (
            this IServiceCollection services,
            SendGridMailerSettings mailerSettings
        )
        {
            return services.AddMailer<SendGridMailer, SendGridMailerSettings>(mailerSettings);
        }

        /// <summary>
        ///     Adds the SendGrid mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettingsFactory">A func that returns the mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddSendGridMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, SendGridMailerSettings> mailerSettingsFactory
        )
        {
            return services.AddMailer<SendGridMailer, SendGridMailerSettings>(mailerSettingsFactory);
        }

        /// <summary>
        /// Adds the safety mailer proxy for a SendGrid mailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer
        (
            this IServiceCollection services,
            SafetyMailerSettings safetyMailerSettings,
            SendGridMailerSettings mailerSettings
        )
        {
            return services
                .AddMailer<SafetyMailer<SendGridMailer>, SafetyMailerSettings, SendGridMailer, SendGridMailerSettings>(
                    mailerSettings, safetyMailerSettings);
        }

        /// <summary>
        /// Adds the safety mailer proxy for a SendGrid mailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettingsFactory">The safety mailer settings factory.</param>
        /// <param name="mailerSettingsFactory">The mailer settings factory.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, SafetyMailerSettings> safetyMailerSettingsFactory,
            Func<IServiceProvider, SendGridMailerSettings> mailerSettingsFactory
        )
        {
            return services
                .AddMailer<SafetyMailer<SendGridMailer>, SafetyMailerSettings, SendGridMailer, SendGridMailerSettings>(
                    mailerSettingsFactory, safetyMailerSettingsFactory);
        }
    }
}