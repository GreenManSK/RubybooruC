using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    interface IqdbApi
    {
        List<Match> SearchFile(string path, Options options);
        List<Match> SearchUrl(string url, Options options);
    }
}
