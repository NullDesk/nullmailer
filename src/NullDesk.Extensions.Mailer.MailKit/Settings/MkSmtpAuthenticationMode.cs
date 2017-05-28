// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     MailKit Authentication Modes
    /// </summary>
    public enum MkSmtpAuthenticationMode
    {
        /// <summary>
        ///     Automatically detemine the authenticaition mode based on supplied authentication settings
        /// </summary>
        Auto,

        /// <summary>
        ///     Use basic authentication
        /// </summary>
        Basic,

        /// <summary>
        ///     Use token authentication
        /// </summary>
        Token,

        /// <summary>
        ///     Use System.Net.Credential authentication
        /// </summary>
        Credentials
    }
}