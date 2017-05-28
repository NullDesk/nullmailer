// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Mailer settings marker interface
    /// </summary>
    public interface IMailerSettings
    {
        /// <summary>
        ///     From Email Address
        /// </summary>
        /// <returns></returns>
        string FromEmailAddress { get; set; }

        /// <summary>
        ///     From display name.
        /// </summary>
        /// <value>From display name.</value>
        string FromDisplayName { get; set; }


        /// <summary>
        ///     Reply to email address.
        /// </summary>
        /// <value>The reply to email address.</value>
        string ReplyToEmailAddress { get; set; }


        /// <summary>
        ///     Reply to display name.
        /// </summary>
        /// <value>The display name of the reply to address.</value>
        string ReplyToDisplayName { get; set; }
    }
}