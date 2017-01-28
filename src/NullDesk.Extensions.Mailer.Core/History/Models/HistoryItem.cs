using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.History
{
    /// <summary>
    /// Message Delivery History Record.
    /// </summary>
    public class HistoryItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the delivery provider.
        /// </summary>
        /// <value>The delivery provider.</value>
        [StringLength(50)]
        public string DeliveryProvider { get; set; }

        /// <summary>
        /// Gets or sets the message date.
        /// </summary>
        /// <value>The message date.</value>
        public DateTimeOffset MessageDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message was successfully delivered.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the sent to email.
        /// </summary>
        /// <value>The sent to email.</value>
        [StringLength(200)]
        public string ToEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets to display name.
        /// </summary>
        /// <value>To display name.</value>
        [StringLength(200)]
        public string ToDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [StringLength(250)]
        public string Subject { get; set; }

        /// <summary>
        /// The serialized message data in JSON.
        /// </summary>
        /// <value>The message json.</value>
        [StringLength(int.MaxValue)]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the exception message if an exception occured.
        /// </summary>
        /// <value>The exception message.</value>
        [StringLength(500)]
        public string ExceptionMessage { get; set; }
    }
}
