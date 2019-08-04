using System.Collections.Generic;

namespace Rubybooru.Downloader.lib.preprocess
{
    interface IPreprocessor
    {
        bool IsMatch(string file);
        void Preprocess(List<string> result, string file);
    }
}