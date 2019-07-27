using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    public class Match : IComparable<Match>
    {
        public int Similarity { get; }
        public int Width { get; }
        public int Height { get; }
        public ServiceType Type { get; }
        public Uri Url { get; }

        public Match(int similarity, int width, int height, ServiceType type, Uri url)
        {
            Similarity = similarity;
            Width = width;
            Height = height;
            Type = type;
            Url = url;
        }

        public int CompareTo(Match other)
        {
            return Similarity - other.Similarity;
        }
    }
}
