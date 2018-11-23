using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Json converter for IMessageBody.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class MessageBodyJsonConverter : JsonConverter
    {
        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            IMessageBody body = null;
            var jbody = JObject.Load(reader);
            var hasTemplateName = jbody.Properties()
                .Any(p => p.Name.Equals("templatename", StringComparison.OrdinalIgnoreCase));
            if (hasTemplateName)
            {
                body = jbody.ToObject<TemplateBody>();
            }
            else
            {
                var hasContentProperties = jbody.Properties()
                    .Any(p => p.Name.Equals("PlainTextContent", StringComparison.OrdinalIgnoreCase) ||
                              p.Name.Equals("HtmlContent", StringComparison.OrdinalIgnoreCase));
                if (hasContentProperties)
                {
                    body = jbody.ToObject<ContentBody>();
                }
            }

            return body;
        }

        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMessageBody);
        }
    }
}