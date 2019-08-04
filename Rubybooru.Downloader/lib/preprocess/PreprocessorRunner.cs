using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib.preprocess
{
    class PreprocessorRunner
    {
        public static List<string> Preprocess(IEnumerable<string> files, IPreprocessor preprocessor)
        {
            var result = new List<string>();

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                if (preprocessor.IsMatch(file))
                {
                    string taskFile = file;
                    tasks.Add(Task.Run(() => { preprocessor.Preprocess(result, taskFile); }));
                }
                else
                {
                    result.Add(file);
                }
            }

            Task.WaitAll(tasks.ToArray());
            return result;
        }
    }
}