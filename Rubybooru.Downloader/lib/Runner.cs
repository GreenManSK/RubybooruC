using Rubybooru.Downloader.lib.preprocess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib
{
    public class Runner
    {
        private readonly Settings settings;

        public Runner(Settings settings)
        {
            this.settings = settings;
            // TODO: Check settings validity
        }

        public void Start()
        {
            // TODO: Make task?
            var files = GetFiles(settings.SourceImagesDirPath, settings.IncludeSubdirs);
            files = Preprocess(files);
            files = Filter.FilterFiles(files, settings.AllowedExtensions);
            // Filter.Filter
            // Downloader.Start

            Console.WriteLine(files);
        }

        private List<string> GetFiles(string path, bool includeSubdirs)
        {
            return Directory.GetFiles(path, "*",
                includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
        }

        private List<string> Preprocess(List<string> files)
        {
            if (settings.RenameLargeExt)
            {
                files = PreprocessorRunner.Preprocess(files, new LargeExtPreprocessor());
            }

            if (settings.DeleteWindowsDuplicates)
            {
                files = PreprocessorRunner.Preprocess(files, new ChromeDuplicatePreprocessor());
            }

            return files;
        }
    }
}