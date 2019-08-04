using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib
{
    public class Downloader
    {
        public static string GetJsonFileName(string file)
        {
            return $"{file}.json";
        }
    }
}
