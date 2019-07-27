using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    public interface IqdbApi
    {
        Task<List<Match>> SearchFile(string path, Options options);
        Task<List<Match>> SearchUrl(string url, Options options);
    }
}
