using System;
// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Interface for safety mailer proxy types
    /// </summary>
    public interface IProxyMailer<TProxySettings, out TMailer> : IDisposable
        where TProxySettings : class, IProxyMailerSettings
        where TMailer : class, IMailer
    {
        /// <summary>
        ///     Gets the underlying mailer instance wrapped by the proxy.
        /// </summary>
        /// <value>The mailer.</value>
        TMailer Mailer { get; }

        /// <summary>
        /// The settings for the proxy mailer.
        /// </summary>
        /// <value>The settings.</value>
        TProxySettings Settings { get; set; }
    }


}