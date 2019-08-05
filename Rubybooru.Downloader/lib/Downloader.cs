using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Rubybooru.Downloader.lib.helper;

namespace Rubybooru.Downloader.lib
{
    public class Downloader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
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

        public async Task Start()
        {
            using (var iqdbApi = new IqdbApi.api.IqdbApi())
            {
                var last = files.Last();
                foreach (var file in files)
                {
                    DownloadFile(file);
                    if (!file.Equals(last))
                    {
                        await Task.Delay(settings.RequestDelayMs);
                    }
                }
            }
        }

        private void DownloadFile(string file)
        {
            try
            {
                // TODO: Download info
            }
            catch (Exception e)
            {
                var msg = $"Error while downloading file '{file}'";
                Logger.Error(e, msg);
                Errors.Add(new DownloadError(msg, e));
            }
        }

        public static string GetJsonFileName(string file)
        {
            return $"{file}.json";
        }
    }
}