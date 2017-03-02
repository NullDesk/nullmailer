using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class FileExtensions.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        ///     Converts content of a template file into string message content.
        /// </summary>
        /// <param name="templateFile">The tempalte file.</param>
        /// <param name="replacementVariables">The replacement variables.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static async Task<string> ToMessageAsync(this FileInfo templateFile,
            IDictionary<string, string> replacementVariables, CancellationToken token = default(CancellationToken))
        {
            if (!token.IsCancellationRequested)
            {
                var fileContents = await templateFile.OpenText().ReadToEndAsync();
                return fileContents.TemplateReplace(replacementVariables);
            }
            return null;
        }
    }
}