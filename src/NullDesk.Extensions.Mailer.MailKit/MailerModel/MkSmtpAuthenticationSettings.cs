// ReSharper disable once CheckNamespace

using NullDesk.Extensions.Mailer.MailKit.Authentication;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.MailKit
{
    /// <summary>
    ///     Authentication settings for MailKit SMTP mailers.
    /// </summary>
    public class MkSmtpAuthenticationSettings
    {
        private IMkSmtpAuthenticator _auth;

        /// <summary>
        ///     An authenticator for the mailer, or a NullAuthenticator is no authentication is configured.
        /// </summary>
        /// <value>The authenticator.</value>
        public IMkSmtpAuthenticator Authenticator
        {
            get
            {

                if (_auth == null || _auth.GetType() == typeof(NullAuthenticator))
                {
                    _auth = 
                           !string.IsNullOrEmpty(UserName) 
                        && !string.IsNullOrEmpty(Password)
                        ? (IMkSmtpAuthenticator)new MkSmtpBasicAuthenticator
                        {
                            UserName = UserName,
                            Password = Password
                        }
                        : new NullAuthenticator();
                }
                return _auth;
            }
            set => _auth = value;
        }

        /// <summary>
        ///     The name of the user for basic authentication.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        ///     The password for basic authentication.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }
}