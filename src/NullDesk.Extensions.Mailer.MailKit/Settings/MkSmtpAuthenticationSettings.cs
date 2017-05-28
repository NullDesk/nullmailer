// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Authentication settings for MailKit SMTP mailers.
    /// </summary>
    public class MkSmtpAuthenticationSettings
    {
        /// <summary>
        ///     The active authentication mode to use for SMTP connections.
        /// </summary>
        /// <value>The authentication mode.</value>
        public MkSmtpAuthenticationMode AuthenticationMode { get; set; } = MkSmtpAuthenticationMode.Auto;

        /// <summary>
        ///     Basic authentication settings.
        /// </summary>
        /// <value>The basic authentication.</value>
        public MkSmtpBasicAuthenticationSettings BasicAuthentication { get; set; }

        /// <summary>
        ///     Access token authentication.
        /// </summary>
        /// <value>The access token authentication.</value>
        public MkSmtpAccessTokenAuthenticationSettings AccessTokenAuthentication { get; set; }

        /// <summary>
        ///     System.Net.Credentials authentication.
        /// </summary>
        /// <value>The credentials authentication.</value>
        public MkSmtpCredentialsAuthenticationSettings CredentialsAuthentication { get; set; }


        /// <summary>
        ///     Gets the best settings for the current authentication mode.
        /// </summary>
        /// <returns>IAuthenticationSettings.</returns>
        public IAuthenticationSettings GetSettingsForAuthenticationMode()
        {
            IAuthenticationSettings settings = null;
            switch (AuthenticationMode)
            {
                case MkSmtpAuthenticationMode.Basic:
                    settings = BasicAuthentication;
                    break;
                case MkSmtpAuthenticationMode.Token:
                    settings = AccessTokenAuthentication;
                    break;
                case MkSmtpAuthenticationMode.Credentials:
                    settings = CredentialsAuthentication;
                    break;
                default:
                    settings = AccessTokenAuthentication ??
                               ((IAuthenticationSettings) BasicAuthentication ?? CredentialsAuthentication);
                    break;
            }
            return settings;
        }
    }
}