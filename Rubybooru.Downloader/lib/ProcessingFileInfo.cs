using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib
{
    public enum ProcessingState
    {
        Init, Fetching, Parsing, Saving, Error
    }

    public class ProcessingFileInfo
    {
        public string File { get; set; }
        public ProcessingState State { get; set; }

        public ProcessingFileInfo(string file, ProcessingState state = ProcessingState.Init)
        {
            File = file;
            State = state;
        }
    }
}
