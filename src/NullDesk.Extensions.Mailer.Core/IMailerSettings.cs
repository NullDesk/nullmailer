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
    }
}