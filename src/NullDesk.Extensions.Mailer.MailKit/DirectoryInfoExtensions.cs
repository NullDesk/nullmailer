using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.MailKit
{
    internal static class DirectoryInfoExtensions
    {
        internal static FileInfo GetFirstFileForExtensions(this DirectoryInfo dir, string fileName, params string[] extensions)
        {
            //ensure extensions list all start with dot
            var fExtensions = extensions.Select(e => e.StartsWith(".") ? e : $".{e}");

            var files = dir.EnumerateFiles($"{fileName}.*");

            return files.FirstOrDefault(f => fExtensions.Contains(f.Extension));
        }
    }
}
