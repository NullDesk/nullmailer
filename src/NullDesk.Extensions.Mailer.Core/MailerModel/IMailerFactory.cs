using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Factory class for obtaining mailer instances.
    ///     Factory class for obtaining mailer instances.
    /// </summary>
    public interface IMailerFactory
    {
        /// <summary>
        ///     Gets a collection of registered mailer functions.
        /// </summary>
        /// <value>The mailers.</value>
        List<Func<IMailer>> MailerRegistrations { get; }

        /// <summary>
        /// Gets an instance of the first registered standard mailer.
        /// </summary>
        /// <returns>IMailer.</returns>
        IMailer GetMailer();


        /// <summary>
        ///     Registers a function that can be use to create a configured mailer instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mailerFunc">The mailer function.</param>
        void Register<T>(Func<T> mailerFunc) where T : class, IMailer;

        /// <summary>
        /// Gets an instance of a registered mailer for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of mailer instance you wish to create</typeparam>
        /// <returns>IMailer.</returns>
        /// <remarks>If more than one function for the mailer type exists, will use the first matching function.</remarks>
        IMailer GetMailer<T>() where T : class, IMailer;
    }
}