using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Summary of Delivery details.
    /// </summary>
    public class DeliverySummary
    {
        /// <summary>
        ///     The unique message identifier
        /// </summary>
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///     Gets or sets the delivery provider.
        /// </summary>
        /// <value>The delivery provider.</value>
        public virtual string DeliveryProvider { get; set; }

        /// <summary>
        ///     Gets or sets the provider's identifier for the message.
        /// </summary>
        /// <remarks>
        ///     Used to reference the message in the underlying mail system after delivery has been attempted.
        /// </remarks>
        /// <value>The provider message identifier.</value>
        [StringLength(100)]
        public virtual string ProviderMessageId { get; set; }

        /// <summary>
        ///     Gets or sets the message created date.
        /// </summary>
        /// <value>The message date.</value>
        public virtual DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        ///     Gets or sets a value indicating whether the message was successfully sent.
        /// </summary>
        public virtual bool IsSuccess { get; set; } = false;

        /// <summary>
        ///     Gets or sets the sender's email address.
        /// </summary>
        /// <value>The senders email address.</value>
        public virtual string FromEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets the sender's display name for the sender.
        /// </summary>
        /// <value>The display name.</value>
        public virtual string FromDisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the reply to email address.
        /// </summary>
        /// <value>The email address.</value>
        public virtual string ReplyToEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets the reply to display name.
        /// </summary>
        /// <value>The display name.</value>
        public virtual string ReplyToDisplayName { get; set; }


        /// <summary>
        ///     Gets or sets the sent to email.
        /// </summary>
        /// <value>The sent to email.</value>
        public virtual string ToEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets to display name.
        /// </summary>
        /// <value>To display name.</value>
        public virtual string ToDisplayName { get; set; }

        /// <summary>
        ///     The message subject.
        /// </summary>
        /// <remarks>
        ///     If substitutions are provided, they will be used here. Some services may ignore this value when using templates,
        ///     others will use this value in place of any subject defined in the template.
        /// </remarks>
        /// <value>The subject.</value>
        public virtual string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the exception message if an exception occurred.
        /// </summary>
        /// <value>The exception message.</value>
        public virtual string ExceptionMessage { get; set; }

        /// <summary>
        ///     The name of the application to be include in history.
        /// </summary>
        /// <remarks>Use to give a name to the system recording history.</remarks>
        /// <value>The name of the delivery provider.</value>
        public virtual string SourceApplicationName { get; set; }
    }
}