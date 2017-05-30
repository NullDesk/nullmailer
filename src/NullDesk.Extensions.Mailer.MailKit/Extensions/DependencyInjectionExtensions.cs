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
        ///     Adds the MailKit SMTP mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMkSmtpMailer
        (
            this IServiceCollection services,
            MkSmtpMailerSettings settings
        )
        {
            return services.AddMailer<MkSmtpMailer, MkSmtpMailerSettings>(settings);
        }

        /// <summary>
        ///     Adds the MailKit SMTP mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddMkSmtpMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, MkSmtpMailerSettings> settingsFactory
        )
        {
            return services.AddMailer<MkSmtpMailer, MkSmtpMailerSettings>(settingsFactory);
        }
    }
}