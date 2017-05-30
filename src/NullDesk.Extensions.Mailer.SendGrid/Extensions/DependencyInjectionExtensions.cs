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
        /// <param name="settings">The settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddSendGridMailer
        (
            this IServiceCollection services,
            SendGridMailerSettings settings
        )
        {
            return services.AddMailer<SendGridMailer, SendGridMailerSettings>(settings);
        }

        /// <summary>
        ///     Adds the SendGrid mailer system to dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settingsFactory">A func that returns the mailer settings.</param>
        /// <returns>IServiceCollection.</returns>
        /// <remarks>Will setup the mailer as the default</remarks>
        public static IServiceCollection AddSendGridMailer
        (
            this IServiceCollection services,
            Func<IServiceProvider, SendGridMailerSettings> settingsFactory
        )
        {
            return services.AddMailer<SendGridMailer, SendGridMailerSettings>(settingsFactory);
        }
    }
}