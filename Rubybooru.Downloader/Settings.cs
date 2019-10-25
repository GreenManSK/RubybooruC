using System.Collections.Generic;
using IqdbApi.api;

namespace Rubybooru.Downloader
{
    public class Settings
    {
        public string SourceImagesDirPath { get; set; }
        public string DownloadedDirPath { get; set; }
        public string NoMatchDirPath { get; set; }

        public int RequestDelayMs { get; set; } = 750;

        public List<ServiceType> Services { get; set; } = new List<ServiceType>()
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

        public HashSet<string> AllowedExtensions { get; set; } = new HashSet<string>()
        {
            ".jfif",
            ".jpg",
            ".jpeg",
            ".png"
        };
    }
}