using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces content in a string for each of the values in a replacement variables dictionary 
        /// </summary>
        /// <param name="content">The template content</param>
        /// <param name="replacementVariables">The replacement variables</param>
        /// <returns></returns>
        public static string TemplateReplace(this string content, IDictionary<string, string> replacementVariables){
            var result = new StringBuilder(content);
            foreach (var item in replacementVariables)
            {
                result.Replace(item.Key, item.Value);
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets a dictionary with filename and streams from a list of file paths 
        /// </summary>
        /// <param name="files">A list of paths for the files you wish to retrieve</param>
        /// <param name="logger">An optinal logger</param>
        /// <returns>A dictionary of file names and streams for each requested file path</returns>
        public static IDictionary<string, Stream> GetStreamsForFileNames(this IEnumerable<string> files, ILogger logger = null){
            IDictionary<string, Stream> attachments = null;
            if (files != null)
            {
                attachments = new Dictionary<string, Stream>();
                foreach (var attachmentFile in files)
                {
                    if (!File.Exists(attachmentFile))
                    {
                        var ex = new ArgumentException($"Unable to find email attachment with file name: {attachmentFile}");
                        logger.LogError(1, ex, ex.Message);
                        throw ex;
                    }
                    var f = new FileInfo(attachmentFile);
                    attachments.Add(f.Name, f.OpenRead());
                }
            }
            return attachments;
        }
    }


}