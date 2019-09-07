using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IqdbApi.api;
using IqdbApi.parsers.impl;
using NLog;
using Rubybooru.Downloader.lib.helper;

namespace Rubybooru.Downloader.lib
{
    public class Downloader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public readonly int TotalFiles;
        public int FinishedFiles;
        public readonly ConcurrentDictionary<string, ProcessingFileInfo> ProcessingFiles;
        public readonly ConcurrentBag<DownloadError> Errors;

        private readonly List<string> _files;
        private readonly Settings _settings;
        private readonly CancellationToken _cancelToken;
        private readonly MatchRanker _matchRanker;
        private readonly DynamicParser _parser;

        public Downloader(List<string> files, Settings settings, CancellationToken cancelToken)
        {
            _files = files;
            _settings = settings;
            _cancelToken = cancelToken;
            TotalFiles = files.Count;
            ProcessingFiles = new ConcurrentDictionary<string, ProcessingFileInfo>();
            Errors = new ConcurrentBag<DownloadError>();
            _matchRanker = new MatchRanker(settings.MinSimilarity, settings.Services);
            _parser = new DynamicParser();
        }

        public async Task Start()
        {
            using (var iqdbApi = new IqdbApi.api.IqdbApi())
            {
                var last = _files.Last();
                foreach (var file in _files)
                {
                    Logger.Info($"Fetching '{file}'");
                    var processingFile = new ProcessingFileInfo(file);
                    ProcessingFiles.GetOrAdd(processingFile.File, processingFile);
                    DownloadFile(processingFile, iqdbApi);
                    if (!file.Equals(last))
                    {
                        await Task.Delay(_settings.RequestDelayMs);
                    }
                }
            }
        }

        private async void DownloadFile(ProcessingFileInfo file, IIqdbApi iqdbApi)
        {
            try
            {
                List<Match> matches;
                try
                {
                    file.State = ProcessingState.Fetching;
                    matches = await iqdbApi.SearchFile(file.File, Options.Default);
                }
                catch (FileSizeLimitException e)
                {
                    matches = await DownloadResizedFile(file, iqdbApi);
                }

                Task.Run(() => ProcessMatches(file, matches), _cancelToken);
            }
            catch (Exception e)
            {
                var msg = $"Error while downloading file '{file.File}'";
                Logger.Error(e, msg);
                Errors.Add(new DownloadError(msg, e));
            }
        }

        private async Task<List<Match>> DownloadResizedFile(ProcessingFileInfo file, IIqdbApi iqdbApi)
        {
            var resizedFile = ImageResizer.Resize(
                file.File,
                IqdbApi.api.IqdbApi.MaxImageWidth,
                IqdbApi.api.IqdbApi.MaxImageHeight
            );
            var matches = await iqdbApi.SearchFile(resizedFile, Options.Default);
            File.Delete(resizedFile);

            return matches;
        }

        private async void ProcessMatches(ProcessingFileInfo file, IReadOnlyCollection<Match> matches)
        {
            var best = matches != null ? _matchRanker.PickBest(matches) : null;
            if (best != null)
            {
                Logger.Info($"Parsing '{file}'");
                file.State = ProcessingState.Parsing;
                
                // TODO: parse match - parser locks, error catching
                var result = await _parser.Parse(best.Url);
                
                MoveFile(file, _settings.DownloadedDirPath);
                // TODO: save data
                
                FinishFile(file);
            }
            else
            {
                Logger.Info($"No match for '{file}'");
                file.State = ProcessingState.Saving;
                MoveFile(file, _settings.NoMatchDirPath);
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
            ProcessingFiles.TryRemove(file.File, out _);
            Interlocked.Increment(ref FinishedFiles);
        }

        public static string GetJsonFileName(string file)
        {
            return $"{file}.json";
        }
    }
}