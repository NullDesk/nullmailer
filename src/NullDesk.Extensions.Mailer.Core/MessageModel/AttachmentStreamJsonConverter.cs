using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NullDesk.Extensions.Mailer.Core.Extensions;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Json converter for attachment stream dictionaries
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class AttachmentStreamJsonConverter : JsonConverter
    {
        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            var v = typeof(IDictionary<string, Stream>).IsAssignableFrom(objectType);
            return v;
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
            var dict = serializer.Deserialize<IDictionary<string, string>>(reader);
            return dict.Select(i =>
                    new KeyValuePair<string, Stream>(
                        i.Key,
                        i.Value == null ? null : new MemoryStream(Convert.FromBase64String(i.Value))))
                .ToDictionary(k => k.Key, k => k.Value);
        }


        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = (IDictionary<string, Stream>) value;

            var converted = dict.Select(i => new KeyValuePair<string, string>(i.Key, i.Value.ToBase64String()))
                .ToDictionary(k => k.Key, k => k.Value);

            serializer.Serialize(writer, converted);
        }
    }
}