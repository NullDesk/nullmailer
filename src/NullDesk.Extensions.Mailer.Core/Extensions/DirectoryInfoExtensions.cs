using System.IO;
using System.Linq;

// ReSharper disable once CheckNamespace

namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class DirectoryInfoExtensions.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        ///     Gets the first file by name with one of the specified extensions.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>FileInfo.</returns>
        public static FileInfo GetFirstFileForExtensions(this DirectoryInfo dir, string fileName,
            params string[] extensions)
        {
            //ensure extensions list all start with dot
            var fExtensions = extensions.Select(e => e.StartsWith(".") ? e : $".{e}");

            var files = dir.EnumerateFiles($"{fileName}.*");

            return files.FirstOrDefault(f => fExtensions.Contains(f.Extension));
        }
    }
}