using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Factory class for obtaining mailer instances.
    /// </summary>
    public class MailerFactory
    {
        private List<Func<IMailer>> Mailers { get; } = new List<Func<IMailer>>();

        /// <summary>
        /// Gets an instance of the first registered standard mailer.
        /// </summary>
        /// <value>The mailer.</value>
        public ITemplateMailer Mailer
        {
            get
            {
                var func = Mailers.FirstOrDefault(m => m
                    .GetType().GetTypeInfo()
                    .GenericTypeArguments
                    .FirstOrDefault()?
                    .GetTypeInfo()
                    .ImplementedInterfaces.Any(i => i == typeof(ITemplateMailer)) ?? false);
                return func?.Invoke() as ITemplateMailer;
            }
        }

        /// <summary>
        /// Gets an instance of the first registered simple mailer.
        /// </summary>
        /// <value>The simple mailer.</value>
        public IMailer SimpleMailer
        {
            get
            {
                var func = Mailers.FirstOrDefault(m => m
                            .GetType().GetTypeInfo()
                            .GenericTypeArguments
                            .FirstOrDefault()?
                            .GetTypeInfo()
                            .ImplementedInterfaces.All(i => i != typeof(ITemplateMailer)) ??
                        false);
                // template mailers also implement IMailer, and can act as a surrogate if a simple mailer wasn't explicitly registered.
                //if simple mailer isn't registered, return a template mailer if one is registered.
                var simple = func?.Invoke() ?? Mailers.FirstOrDefault()?.Invoke();
                return simple;
            }
        }

        /// <summary>
        /// Registers a function that can be use to create a configured mailer instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mailerFunc">The mailer function.</param>
        public void RegisterMailer<T>(Func<T> mailerFunc) where T : class, IMailer
        {
            Mailers.Add(mailerFunc);
        }

        /// <summary>
        /// Gets an instance of a registered mailer for the specified type.
        /// </summary>
        /// <remarks>
        /// If more than one function for the mailer type exists, will use the first matching function.
        /// </remarks>
        /// <typeparam name="T">The type of mailer instance you wish to create</typeparam>
        /// <returns>T.</returns>
        public T GetMailer<T>() where T : class, IMailer
        {
            var mailer = Mailers.FirstOrDefault(m => m.GetType().GenericTypeArguments.FirstOrDefault()?.AssemblyQualifiedName == typeof(T).AssemblyQualifiedName);
            return mailer?.Invoke() as T;
        }

    }
}
