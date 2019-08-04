using IqdbApi.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubybooru.Downloader.lib.helper
{
    class MatchRanker
    {
        private readonly int minSimilarity;
        private readonly Dictionary<int, int> serviceValues;

        public MatchRanker(int minSimilarity, IEnumerable<ServiceType> services)
        {
            this.minSimilarity = minSimilarity;
            serviceValues = new Dictionary<int, int>();
            BuildServiceValues(services);
        }

        public Match PickBest(IEnumerable<Match> matches)
        {
            return matches.Where(m => m.Similarity >= minSimilarity)
                   .OrderBy(m => serviceValues[ServiceType.GetTypeByUrl(m.Url).Id])
                   .First();
        }

        private void BuildServiceValues(IEnumerable<ServiceType> services)
        {
            int value = 0;
            foreach (var service in services)
            {
                serviceValues.Add(service.Id, value++);
            }
        }
    }
}
