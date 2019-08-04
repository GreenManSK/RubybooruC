using IqdbApi.api;
using IqdbApi.parsers.impl;
using Rubybooru.Downloader;
using Rubybooru.Downloader.lib;
using System;

namespace Rubybooru
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();

            // settings.IncludeSubdirs = true;
            settings.SourceImagesDirPath = @"C:\Users\lukas\OneDrive\Ayanoneebook\Desktop\sort test";

            var runner = new Runner(settings);
            runner.Start();
        }
    }
}
