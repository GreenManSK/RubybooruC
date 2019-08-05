using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rubybooru.Downloader.lib
{
    class Filter
    {
        public static List<string> FilterFiles(IEnumerable<string> files, HashSet<string> allowedExt) {
            return files
                .Where(f => IsAllowedExt(f, allowedExt))
                .Where(f => !IsAlreadyDownloaded(f))
                .ToList();
        }

        private static bool IsAlreadyDownloaded(string file)
        {
            return File.Exists(Downloader.GetJsonFileName(file));
        }

        private static bool IsAllowedExt(string fileName, ICollection<string> allowedExt)
        {
            var ext = Path.GetExtension(fileName);
            return ext != null && allowedExt.Contains(ext.ToLower());
        }
    }
}
