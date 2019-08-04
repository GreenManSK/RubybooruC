using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib
{
    public class DownloadError
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public DownloadError(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}
