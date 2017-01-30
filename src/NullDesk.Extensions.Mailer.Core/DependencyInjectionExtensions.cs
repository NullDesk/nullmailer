using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Class MailerServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the mailer system to dependency injection.
        /// </summary>
        /// <remarks>Will setup the mailer as the default</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public static IServiceCollection AddMailer<T>(this IServiceCollection services)
            where T : class, ISimpleMailer
        {
            services.Add(ServiceDescriptor.Transient<T, T>());

            services.TrySetupAsSimpleMailer<T>();

            services.TrySetupAsStandardMailer<T>();

            return services;
        }

        private static void TrySetupAsStandardMailer<T>(this IServiceCollection services)
            where T : class, ISimpleMailer
        {
            if (typeof(T).GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(IStandardMailer)) &&
                services.All(d => d.ServiceType != typeof(IStandardMailer)))
            {
                services.Add(ServiceDescriptor.Transient(typeof(IStandardMailer), typeof(T)));
            }
        }

        private static void TrySetupAsSimpleMailer<T>(this IServiceCollection services)
            where T : class, ISimpleMailer
        {
            var simpleDescriptor = ServiceDescriptor.Transient(typeof(ISimpleMailer), typeof(T));
            if (services.All(d => d.ServiceType != simpleDescriptor.ServiceType))
            {
                services.Add(simpleDescriptor);
            }
        }
    }
}
