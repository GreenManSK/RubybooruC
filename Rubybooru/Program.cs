using System;
using IqdbApi.api;

namespace Rubybooru
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var iqdb = new BasicIqdbApi()) {
                var r =  iqdb.SearchUrl("http://img7.anidb.net/pics/anime/235604.jpg", Options.Default);
                //var r = iqdb.SearchFile(@"C:\Users\lukas\OneDrive\Downloads\235604.jpg", Options.Default);
                r.Wait();
                Console.WriteLine(r.Result.ToString());
            }
            Console.ReadKey();
        }
    }
}
