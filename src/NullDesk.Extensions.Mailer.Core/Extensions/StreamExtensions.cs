using System;
using System.IO;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core.Extensions
{
    /// <summary>
    /// Class StreamExtensions.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Gets a base64 encoded string from the stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static async Task<string> ToBase64String(this Stream input)
        {
            if (input == null)
            {
                return null;
            }
            using (var ms = new MemoryStream())
            {
                input.Position = 0; //put stream back to start
                await input.CopyToAsync(ms);
                return Convert.ToBase64String(ms.ToArray());
            }

        }
    }
}
