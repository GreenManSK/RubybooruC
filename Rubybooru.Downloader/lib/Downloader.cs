using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IqdbApi.api;
using IqdbApi.parsers;
using IqdbApi.parsers.impl;
using NLog;
using Rubybooru.Data;
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
                var last = _files.Count > 0 ? _files.Last() : null;
                foreach (var file in _files)
                {
                    Logger.Info($"Fetching '{file}'");
                    var processingFile = new ProcessingFileInfo(file);
                    ProcessingFiles.GetOrAdd(processingFile.File, processingFile);
                    await DownloadFile(processingFile, iqdbApi);
                    if (!file.Equals(last))
                    {
                        await Task.Delay(_settings.RequestDelayMs);
                    }
                }
            }
        }

        private async Task DownloadFile(ProcessingFileInfo file, IIqdbApi iqdbApi)
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
                FileError(file, e, $"Error while downloading file '{file.File}'");
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

        private void ProcessMatches(ProcessingFileInfo file, IReadOnlyCollection<Match> matches)
        {
            var bests = matches != null ? _matchRanker.OrderBest(matches) : null;
            foreach (var best in bests)
            {
                Logger.Info($"Parsing '{file.File}'");
                file.State = ProcessingState.Parsing;

                ParseResult result = null;
                lock (ServiceType.GetTypeByUrl(best.Url))
                {
                    try
                    {
                        var parseTask = _parser.Parse(best.Url);
                        parseTask.Wait(_cancelToken);
                        result = parseTask.Result;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Error while parsing file '{file.File}'");
                    }
                }

                if (result == null) continue;
                var newFilePath = MoveFile(file, _settings.DownloadedDirPath);
                try
                {
                    JsonHelper.SaveJson(newFilePath, best, result);
                }
                catch (Exception e)
                {
                    FileError(file, e, $"Error while saving data for file '{file.File}'");
                }

                FinishFile(file);
                return;
            }

            Logger.Info($"No match for '{file.File}'");
            file.State = ProcessingState.Saving;
            MoveFile(file, _settings.NoMatchDirPath);
            FinishFile(file);
        }

        private string MoveFile(ProcessingFileInfo file, string dest)
        {
            if (dest == null)
                return file.File;
            try
            {
                var fileName = Path.GetFileName(file.File);
                var destFile = Path.Combine(dest, fileName);
                File.Move(file.File, destFile);
                return destFile;
            }
            catch (Exception e)
            {
                FileError(file, e, $"Error while moving file '{file.File}' to '{dest}'");
            }

            return file.File;
        }

        private void FileError(ProcessingFileInfo file, Exception e, string msg)
        {
            file.State = ProcessingState.Error;
            Logger.Error(e, msg);
            Errors.Add(new DownloadError(msg, e));
            FinishFile(file);
        }

        private void FinishFile(ProcessingFileInfo file)
        {
            if (ProcessingFiles.TryRemove(file.File, out _))
            {
                Interlocked.Increment(ref FinishedFiles);
            }
        }
    }
}