using Rubybooru.Downloader.lib.helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib
{
    public class Downloader
    {
        public readonly int TotalFiles;
        public int FinishedFiles { get; private set; }
        public readonly ObservableCollection<ProcessingFileInfo> ProcessingFiles;
        public readonly ObservableCollection<DownloadError> Errors;

        private readonly IEnumerable<string> files;
        private readonly Settings settings;
        private readonly MatchRanker matchRanker;

        public Downloader(IEnumerable<string> files, Settings settings)
        {
            this.files = files;
            this.settings = settings;
            TotalFiles = files.Count();
            ProcessingFiles = new ObservableCollection<ProcessingFileInfo>();
            Errors = new ObservableCollection<DownloadError>();
            matchRanker = new MatchRanker(settings.MinSimilarity, settings.Services);
        }

        public void Start()
        {
            
        }

        public static string GetJsonFileName(string file)
        {
            return $"{file}.json";
        }
    }
}
