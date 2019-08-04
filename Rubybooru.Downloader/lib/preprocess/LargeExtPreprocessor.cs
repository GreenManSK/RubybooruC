using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace Rubybooru.Downloader.lib.preprocess
{
    class LargeExtPreprocessor : IPreprocessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string ExtEng = "_large";

        public bool IsMatch(string file)
        {
            var ext = Path.GetExtension(file);
            return ext != null && ext.EndsWith(ExtEng);
        }

        public void Preprocess(List<string> result, string file)
        {
            try
            {
                var newFile = file.Substring(0, file.Length - ExtEng.Length);
                Logger.Info($"Renaming '{file}' to '{newFile}'");
                File.Move(file, newFile);
                result.Add(newFile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Couldn't rename file extension for '{file}'");
            }
        }
    }
}