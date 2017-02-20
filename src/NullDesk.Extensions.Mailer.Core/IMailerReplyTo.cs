namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Interface IMailerAddress
    /// </summary>
    public interface IMailerAddress
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the display name for the reply address.
        /// </summary>
        /// <value>The display name.</value>
        string EmailAddress { get; set; }
    }
}