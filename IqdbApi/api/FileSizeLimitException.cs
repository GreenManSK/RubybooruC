using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    public class FileSizeLimitException: Exception
    {
        public FileSizeLimitException(string message): base(message)
        {

        }
    }
}
