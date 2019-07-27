using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IqdbApi.api;

namespace Rubybooru
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var iqdb = new BasicIqdbApi()) {
                var options = new Options(new List<ServiceType>() { ServiceType.Zerochan, ServiceType.Danbooru }, false);
                var r =  iqdb.SearchUrl("http://img7.anidb.net/pics/anime/235604.jpg", options);
                //var r = iqdb.SearchFile(@"C:\Users\lukas\OneDrive\Downloads\235604.jpg", Options.Default);
                r.Wait();
            }
            System.Console.ReadKey();
        }
    }
}
