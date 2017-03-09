using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Flattened mailer message for a single recipient with delivery meta-data.
    /// </summary>
    public class DeliveryItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryItem"/> class.
        /// </summary>
        public DeliveryItem() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeliveryItem" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipient">The recipient.</param>
        public DeliveryItem(MailerMessage message, MessageRecipient recipient)
        {
            FromEmailAddress = message?.From?.EmailAddress;
            FromDisplayName = message?.From?.DisplayName;
            ToEmailAddress = recipient?.EmailAddress;
            ToDisplayName = recipient?.DisplayName;
            Subject = message?.Subject;
            Body = message?.Body;
            Attachments = message?.Attachments;

            //merge personalized and general subs
            Substitutions = new Dictionary<string, string>(recipient?.PersonalizedSubstitutions);
            foreach (var gSub in message?.Substitutions?.Where(s => !Substitutions.ContainsKey(s.Key)) ?? new Dictionary<string, string>())
            {
                Substitutions.Add(gSub.Key, gSub.Value);
            }
        }

        /// <summary>
        ///     The unique message identifier
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///     Gets or sets the delivery provider.
        /// </summary>
        /// <value>The delivery provider.</value>
        [StringLength(50)]
        public string DeliveryProvider { get; set; }

        /// <summary>
        /// Gets or sets the provider's identifier for the message.
        /// </summary>
        /// <remarks>
        /// Used to reference the message in the underlying mail system after delivery has been attempted.
        /// </remarks>
        /// <value>The provider message identifier.</value>
        [StringLength(100)]
        public string ProviderMessageId { get; set; }

        /// <summary>
        ///     Gets or sets the message created date.
        /// </summary>
        /// <value>The message date.</value>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        ///     Gets or sets a value indicating whether the message was successfully sent.
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        public string FromEmailAddress { get; set; }

        /// <summary>
        ///     Gets or sets the display name for the reply address.
        /// </summary>
        /// <value>The display name.</value>
        public string FromDisplayName { get; set; }

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
        public string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the message body.
        /// </summary>
        /// <value>The body.</value>
        [NotMapped]
        public IMessageBody Body { get; set; }




        /// <summary>
        ///     A collection of attachments to include with the message.
        /// </summary>
        /// <value>The attachments.</value>
        [NotMapped]
        public IDictionary<string, Stream> Attachments { get; set; }


        /// <summary>
        ///     A collection of tokens and replacement values to use with body contents, templates, and message subject.
        /// </summary>
        /// <remarks>
        ///     This collection contains the merged dictionary of personalized substitutions and general message substitutions.
        /// </remarks>
        /// <value>The substitutions.</value>
        [NotMapped]
        public IDictionary<string, string> Substitutions { get; set; }



        /// <summary>
        ///     Gets or sets the exception message if an exception occurred.
        /// </summary>
        /// <value>The exception message.</value>
        [StringLength(500)]
        public string ExceptionMessage { get; set; }

        #region history storage fields

        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        /// Gets the body as json .
        /// </summary>
        /// <value>The content.</value>
        public string BodyJson
        {
            get
            {
                return JsonConvert.SerializeObject(Body, Formatting.Indented, jsonSettings);
            }
            set
            {
                var b = JsonConvert.DeserializeObject(value, jsonSettings);
                Body = (IMessageBody)b;
            }
        }
        /// <summary>
        /// Gets or sets the substitutions json.
        /// </summary>
        /// <value>The substitutions json.</value>
        public string SubstitutionsJson
        {
            get { return JsonConvert.SerializeObject(Substitutions, Formatting.Indented); }
            set { Substitutions = JsonConvert.DeserializeObject<IDictionary<string, string>>(value); }
        }

        /// <summary>
        /// Gets or sets the attachments json.
        /// </summary>
        /// <value>The attachments json.</value>
        public string AttachmentsJson
        {
            get
            {
                var att = new Dictionary<string, string>(Attachments.Select(a =>
                 {
                     var ms = new MemoryStream();
                     a.Value.CopyTo(ms);
                     return new KeyValuePair<string, string>(a.Key, ms.ToString());
                 }).ToDictionary(k => k.Key, k => k.Value));


                return JsonConvert.SerializeObject(att, Formatting.Indented);
            }
            set
            {

                var att = JsonConvert.DeserializeObject<IDictionary<string, string>>(value);
                Attachments = att.Select(a =>
                {
                    var ms = new MemoryStream();
                    StreamWriter writer = new StreamWriter(ms);
                    writer.Write(a.Value);
                    writer.Flush();
                    ms.Position = 0;
                    return new KeyValuePair<string, Stream>(a.Key, ms);
                }).ToDictionary(k => k.Key, k => k.Value);
            }
        }

        #endregion

        /// <summary>
        ///     Get the specified content after processing this instance's substitutions.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.String.</returns>
        public string ProcessSubstitutions(string content)
        {
            return content.PerformContentSubstitution(Substitutions);
        }
    }
}