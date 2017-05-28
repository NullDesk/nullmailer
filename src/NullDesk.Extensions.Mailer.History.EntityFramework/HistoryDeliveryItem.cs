using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    ///     Class HistoryDeliveryItem.
    /// </summary>
    public class EntityHistoryDeliveryItem
    {
        /// <summary>
        ///     The unique message identifier
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///     Gets or sets the delivery provider.
        /// </summary>
        /// <value>The delivery provider.</value>
        [StringLength(100)]
        public string DeliveryProvider { get; set; }

        /// <summary>
        ///     Gets or sets the provider's identifier for the message.
        /// </summary>
        /// <remarks>
        ///     Used to reference the message in the underlying mail system after delivery has been attempted.
        /// </remarks>
        /// <value>The provider message identifier.</value>
        [StringLength(200)]
        public string ProviderMessageId { get; set; }

        /// <summary>
        ///     Gets or sets the message created date.
        /// </summary>
        /// <value>The message date.</value>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        ///     Gets or sets a value indicating whether the message was successfully sent.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        [StringLength(200)]
        public string FromEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets the display name for the reply address.
        /// </summary>
        /// <value>The display name.</value>
        [StringLength(200)]
        public string FromDisplayName { get; set; }


        /// <summary>
        ///     Gets or sets the reply to email address.
        /// </summary>
        /// <value>The email address.</value>
        [StringLength(200)]
        public string ReplyToEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets the reply to display name.
        /// </summary>
        /// <value>The display name.</value>
        [StringLength(200)]
        public string ReplyToDisplayName { get; set; }


        /// <summary>
        ///     Gets or sets the sent to email.
        /// </summary>
        /// <value>The sent to email.</value>
        [StringLength(200)]
        public string ToEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets to display name.
        /// </summary>
        /// <value>To display name.</value>
        [StringLength(200)]
        public string ToDisplayName { get; set; }

        /// <summary>
        ///     The message subject.
        /// </summary>
        /// <remarks>
        ///     If substitutions are provided, they will be used here. Some services may ignore this value when using templates,
        ///     others will use this value in place of any subject defined in the template.
        /// </remarks>
        /// <value>The subject.</value>
        [StringLength(200)]
        public string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the html body.
        /// </summary>
        /// <value>The body.</value>
        public string HtmlContent { get; set; }

        /// <summary>
        ///     Gets or sets the text body.
        /// </summary>
        /// <value>The text body.</value>
        public string TextContent { get; set; }

        /// <summary>
        ///     Gets or sets the template name.
        /// </summary>
        /// <value>The template body.</value>
        [StringLength(255)]
        public string TemplateName { get; set; }


        /// <summary>
        ///     A collection of attachments to include with the message.
        /// </summary>
        /// <value>The attachments.</value>
        public string AttachmentsJson { get; set; }


        /// <summary>
        ///     Gets or sets the substitutions json.
        /// </summary>
        /// <value>The substitutions json.</value>
        public string SubstitutionsJson { get; set; }

        /// <summary>
        ///     Gets or sets the exception message if an exception occurred.
        /// </summary>
        /// <value>The exception message.</value>
        [StringLength(500)]
        public string ExceptionMessage { get; set; }
    }

    internal static class DeliveryItemsExtensions
    {
        public static DeliveryItem FromDeliveryItem(this EntityHistoryDeliveryItem item)
        {
            return new DeliveryItem
            {
                Body =
                    string.IsNullOrEmpty(item.TemplateName)
                        ? (IMessageBody) new TemplateBody
                        {
                            TemplateName = item.TemplateName
                        }
                        : new ContentBody
                        {
                            HtmlContent = item.HtmlContent,
                            PlainTextContent = item.TextContent
                        },
                Substitutions = JsonConvert.DeserializeObject<IDictionary<string, string>>(item.SubstitutionsJson),
                CreatedDate = item.CreatedDate,
                DeliveryProvider = item.DeliveryProvider,
                ExceptionMessage = item.ExceptionMessage,
                FromDisplayName = item.FromDisplayName,
                FromEmailAddress = item.FromEmailAddress,
                Id = item.Id,
                IsSuccess = item.IsSuccess,
                ProviderMessageId = item.ProviderMessageId,
                Subject = item.Subject,
                ToDisplayName = item.ToDisplayName,
                ToEmailAddress = item.ToEmailAddress,
                Attachments = GetDeliveryAttachments(item.AttachmentsJson)
            };
        }

        private static IDictionary<string, Stream> GetDeliveryAttachments(string itemAttachmentsJson)
        {
            var att = JsonConvert.DeserializeObject<IDictionary<string, string>>(itemAttachmentsJson);
            return att.Select(a =>
                {
                    var ms = new MemoryStream();
                    var writer = new StreamWriter(ms);
                    writer.Write(a.Value);
                    writer.Flush();
                    ms.Position = 0;
                    return new KeyValuePair<string, Stream>(a.Key, ms);
                })
                .ToDictionary(k => k.Key, k => k.Value);
        }
    }

    internal static class HistoryDeliveryItemExtensions
    {
        public static EntityHistoryDeliveryItem FromDeliveryItem(this DeliveryItem item, bool serializeAttachments)
        {
            return new EntityHistoryDeliveryItem
            {
                HtmlContent = (item.Body as ContentBody)?.HtmlContent,
                TextContent = (item.Body as ContentBody)?.PlainTextContent,
                TemplateName = (item.Body as TemplateBody)?.TemplateName,
                SubstitutionsJson = JsonConvert.SerializeObject(item.Substitutions, Formatting.Indented),
                CreatedDate = item.CreatedDate,
                DeliveryProvider = item.DeliveryProvider,
                ExceptionMessage = item.ExceptionMessage,
                FromDisplayName = item.FromDisplayName,
                FromEmailAddress = item.FromEmailAddress,
                Id = item.Id,
                IsSuccess = item.IsSuccess,
                ProviderMessageId = item.ProviderMessageId,
                Subject = item.Subject,
                ToDisplayName = item.ToDisplayName,
                ToEmailAddress = item.ToEmailAddress,
                AttachmentsJson = GetHistoryAttachments(item.Attachments, serializeAttachments)
            };
        }

        private static string GetHistoryAttachments(IDictionary<string, Stream> itemAttachments,
            bool serializeAttachments)
        {
            var att = new Dictionary<string, string>(itemAttachments.Select(a =>
                {
                    KeyValuePair<string, string> kvp;
                    if (serializeAttachments)
                    {
                        var ms = new MemoryStream();
                        a.Value.CopyTo(ms);
                        a.Value.Position = 0; //put stream back
                        var content = Convert.ToBase64String(ms.ToArray());

                        kvp = new KeyValuePair<string, string>(a.Key, content);
                    }
                    else
                    {
                        kvp = new KeyValuePair<string, string>(a.Key, null);
                    }
                    return kvp;
                })
                .ToDictionary(k => k.Key, k => k.Value));


            return JsonConvert.SerializeObject(att, Formatting.Indented);
        }
    }
}