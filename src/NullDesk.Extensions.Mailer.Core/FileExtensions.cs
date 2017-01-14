using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Class FileExtensions.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// to message as an asynchronous operation.
        /// </summary>
        /// <param name="templateFile">The tempalte file.</param>
        /// <param name="replacementVariables">The replacement variables.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static async Task<string> ToMessageAsync(this FileInfo templateFile, IDictionary<string, string> replacementVariables, CancellationToken token)
        {
            
            var fileContents = await templateFile.OpenText().ReadToEndAsync();
            var emailBody = new StringBuilder(fileContents);
            foreach (var item in replacementVariables)
            {
                emailBody.Replace(item.Key, item.Value);
            }
            return emailBody.ToString();
        }
    }
}