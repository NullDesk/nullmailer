using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Factory class for obtaining mailer instances.
    /// </summary>
    public class MailerFactory : IMailerFactory
    {
        /// <summary>
        ///     Gets a collection of registered mailer functions.
        /// </summary>
        /// <value>The mailers.</value>
        public List<Func<IMailer>> MailerRegistrations { get; } = new List<Func<IMailer>>();

        /// <summary>
        ///     Gets an instance of the first registered standard mailer.
        /// </summary>
        /// <value>The mailer.</value>
        public virtual IMailer GetMailer()
        {
            var func = MailerRegistrations.FirstOrDefault(m => m
                                                                   .GetType().GetTypeInfo()
                                                                   .GenericTypeArguments
                                                                   .FirstOrDefault()?
                                                                   .GetTypeInfo()
                                                                   .ImplementedInterfaces.Any(
                                                                       i => i == typeof(IMailer)) ?? false);
            return func?.Invoke();
        }



        /// <summary>
        ///     Registers a function that can be use to create a configured mailer instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mailerFunc">The mailer function.</param>
        public virtual void Register<T>(Func<T> mailerFunc) where T : class, IMailer
        {
            MailerRegistrations.Add(mailerFunc);
        }

        /// <summary>
        ///     Gets an instance of a registered mailer for the specified type.
        /// </summary>
        /// <remarks>
        ///     If more than one function for the mailer type exists, will use the first matching function.
        /// </remarks>
        /// <typeparam name="T">The type of mailer instance you wish to create</typeparam>
        /// <returns>T.</returns>
        public virtual T GetMailer<T>() where T : class, IMailer
        {
            var mailer =
                MailerRegistrations.FirstOrDefault(
                    m =>
                        m.GetType().GenericTypeArguments.FirstOrDefault()?.AssemblyQualifiedName ==
                        typeof(T).AssemblyQualifiedName);
            return mailer?.Invoke() as T;
        }
    }
}