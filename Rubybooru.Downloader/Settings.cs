using IqdbApi.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader
{
    public class Settings
    {
        public string SourceImagesDirPath { get; set; }
        public string DownloadedDirPath { get; set; }
        public string NoMatchDirPath { get; set; }

        public int RequestDelayMs { get; set; } = 750;

        public List<ServiceType> services { get; set; } = new List<ServiceType>()
        {
            ServiceType.Konachan,
            ServiceType.YandeRe,
            ServiceType.Danbooru,
            ServiceType.Gelbooru,
            ServiceType.SankakuChannel,
            ServiceType.EShuushuu,
            ServiceType.Zerochan,
            ServiceType.AnimePictures
        };

        public int MinSimilarity { get; set; } = 80;

        public bool IncludeSubdirs { get; set; } = false;
        public bool RenameLargeExt { get; set; } = true;
        public bool DeleteWindowsDuplicates { get; set; } = true;

        public List<string> AllowedExtensions { get; set; } = new List<string>()
        {
            "jpg",
            "jpeg",
            "png"
        };
    }
}