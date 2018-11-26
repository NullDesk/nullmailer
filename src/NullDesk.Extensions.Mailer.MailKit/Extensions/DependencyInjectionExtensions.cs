using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Class MailerServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the MailKit SMTP mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMkSmtpMailer
        (
            this IServiceCollection services,
            MkSmtpMailerSettings mailerSettings
        )
        {
            return services.AddMailer<MkSmtpMailer, MkSmtpMailerSettings>(mailerSettings);
        }

        /// <summary>
        /// Adds the MailKit SMTP mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="mailerSettingsFactory">The mailer settings factory.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMkSmtpMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, MkSmtpMailerSettings> mailerSettingsFactory
        )
        {
            return services.AddMailer<MkSmtpMailer, MkSmtpMailerSettings>(mailerSettingsFactory);
        }

        /// <summary>
        /// Adds the safety mailer proxy for a MailKit SMTP mailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettings">The safety mailer settings.</param>
        /// <param name="mailerSettings">The mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer
        (
            this IServiceCollection services,
            SafetyMailerSettings safetyMailerSettings,
            MkSmtpMailerSettings mailerSettings
        )
        {
            return services
                .AddMailer<SafetyMailer<MkSmtpMailer>, SafetyMailerSettings, MkSmtpMailer, MkSmtpMailerSettings>(
                    mailerSettings, safetyMailerSettings);
        }

        /// <summary>
        /// Adds the safety mailer proxy for a MailKit SMTP mailer to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="safetyMailerSettingsFactory">The safety mailer settings function.</param>
        /// <param name="mailerSettingsFactory">The mailer settings function.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSafetyMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, SafetyMailerSettings> safetyMailerSettingsFactory,
            Func<IServiceProvider, MkSmtpMailerSettings> mailerSettingsFactory
        )
        {
            return services
                .AddMailer<SafetyMailer<MkSmtpMailer>, SafetyMailerSettings, MkSmtpMailer, MkSmtpMailerSettings>(
                    mailerSettingsFactory, safetyMailerSettingsFactory);
        }
    }
}