using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IqdbApi.api;
using NLog;
using Rubybooru.Downloader.lib.helper;

namespace Rubybooru.Downloader.lib
{
    public class Downloader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public readonly int TotalFiles;
        public int FinishedFiles;
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
                    Logger.Info($"Fetching '{file}'");
                    var processingFile = new ProcessingFileInfo(file);
                    ProcessingFiles.Add(processingFile);
                    DownloadFile(processingFile, iqdbApi);
                    if (!file.Equals(last))
                    {
                        await Task.Delay(settings.RequestDelayMs);
                    }
                }
            }
        }

        private async void DownloadFile(ProcessingFileInfo file, IIqdbApi iqdbApi)
        {
            try
            {
                List<Match> matches = null;
                try
                {
                    file.State = ProcessingState.Fetching;
                    matches = await iqdbApi.SearchFile(file.File, Options.Default);
                }
                catch (FileSizeLimitException e)
                {
                    // TODO: Resize image
                    // TODO: Delete resized temp
                }

                Task.Run(() => ProcessMatches(file, matches));
            }
            catch (Exception e)
            {
                var msg = $"Error while downloading file '{file.File}'";
                Logger.Error(e, msg);
                Errors.Add(new DownloadError(msg, e));
            }
        }

        private void ProcessMatches(ProcessingFileInfo file, List<Match> matches)
        {
            var best = matches != null ? matchRanker.PickBest(matches) : null;
            if (best != null)
            {
                Logger.Info($"Parsing '{file}'");
                file.State = ProcessingState.Parsing;
                // TODO: parse match
                // TODO: move match
                FinishFile(file);
            }
            else
            {
                Logger.Info($"No match for '{file}'");
                file.State = ProcessingState.Saving;
                MoveFile(file, settings.NoMatchDirPath);
                FinishFile(file);
            }
        }

        private void MoveFile(ProcessingFileInfo file, string dest)
        {
            if (dest == null)
                return;
            try
            {
                var fileName = Path.GetFileName(file.File);
                var destFile = Path.Combine(dest, fileName);
                File.Move(file.File, destFile);
            }
            catch (Exception e)
            {
                var msg = $"Error while moving file '{file.File}' to '{dest}'";
                Logger.Error(e, msg);
                Errors.Add(new DownloadError(msg, e));
            }
        }

        private void FinishFile(ProcessingFileInfo file)
        {
            ProcessingFiles.Remove(file);
            Interlocked.Increment(ref FinishedFiles);
        }

        public static string GetJsonFileName(string file)
        {
            return $"{file}.json";
        }
    }
}