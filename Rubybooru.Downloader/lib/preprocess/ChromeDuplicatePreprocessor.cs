using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NLog;

namespace Rubybooru.Downloader.lib.preprocess
{
    class ChromeDuplicatePreprocessor : IPreprocessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex NamePattern = new Regex(@"^(.*)\s\(\d+\)(\.[^.]*)$");

        public bool IsMatch(string file)
        {
            return NamePattern.Match(file).Success;
        }

        public void Preprocess(List<string> result, string file)
        {
            var match = NamePattern.Match(file);
            if (!match.Success) return;
            var newFile = $"{match.Groups[1].Value}{match.Groups[2].Value}";
            if (!IsSame(file, newFile)) return;
            try
            {
                Logger.Info($"Deleting file '{file}'");
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Couldn't delete file '{file}'");
            }
        }

        private bool IsSame(string file, string newFile)
        {
            return File.Exists(newFile) &&
                   new FileInfo(file).Length == new FileInfo(newFile).Length;
        }
    }
}