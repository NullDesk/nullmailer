using System.Collections.Generic;
using System.IO;

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    /// Class AttachmentFileExtensions.
    /// </summary>
    public static class AttachmentFileExtensions
    {
        /// <summary>
        /// Gets a dictionary with filename and streams from a list of file paths
        /// </summary>
        /// <param name="files">A list of paths for the files you wish to retrieve</param>
        /// <returns>A dictionary of file names and streams for each requested file path</returns>
        public static IDictionary<string, Stream> GetAttachmentStreamsForFile(this IEnumerable<string> files)
        {
            IDictionary<string, Stream> attachments = null;
            if (files != null)
            {
                attachments = new Dictionary<string, Stream>();
                foreach (var attachmentFile in files)
                {
                    attachments.Add(attachmentFile.GetAttachmentStreamForFile());
                }
            }
            return attachments;
        }

        /// <summary>
        /// Gets the attachment streams for file.
        /// </summary>
        /// <param name="attachmentFiles">A collection of files.</param>
        /// <returns>IDictionary&lt;System.String, Stream&gt;.</returns>
        public static IDictionary<string, Stream> GetAttachmentStreamsForFile(this IEnumerable<FileInfo> attachmentFiles)
        {
            IDictionary<string, Stream> attachments = null;
            if (attachmentFiles != null)
            {
                attachments = new Dictionary<string, Stream>();
                foreach (var attachmentFile in attachmentFiles)
                {
                    attachments.Add(attachmentFile.GetAttachmentStreamForFile());
                }
            }
            return attachments;
        }

        /// <summary>
        /// Gets the attachment stream for file.
        /// </summary>
        /// <param name="attachmentFile">The attachment filename.</param>
        /// <returns>KeyValuePair&lt;System.String, Stream&gt;.</returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static KeyValuePair<string, Stream> GetAttachmentStreamForFile(this string attachmentFile)
        {
            if (!File.Exists(attachmentFile))
            {
                throw new FileNotFoundException($"Unable to find email attachment with file name: {attachmentFile}");
            }
            var f = new FileInfo(attachmentFile);
            return f.GetAttachmentStreamForFile();
        }

        /// <summary>
        /// Gets the attachment stream for file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>KeyValuePair&lt;System.String, Stream&gt;.</returns>
        public static KeyValuePair<string, Stream> GetAttachmentStreamForFile(this FileInfo file)
        {
            return new KeyValuePair<string, Stream>(file.Name, file.OpenRead());
        }
    }
}