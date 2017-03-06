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
        /// <remarks>Will setup the mailer as the default</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public static IServiceCollection AddMailer<T>(this IServiceCollection services)
            where T : class, IMailer
        {
            services.Add(ServiceDescriptor.Transient<T, T>());


            services.TrySetupMailer<T>();

            return services;
        }

        private static void TrySetupMailer<T>(this IServiceCollection services)
            where T : class, IMailer
        {
            if (typeof(T).GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(IMailer)) &&
                services.All(d => d.ServiceType != typeof(IMailer)))
            {
                services.Add(ServiceDescriptor.Transient(typeof(IMailer), typeof(T)));
            }
        }
    }
}