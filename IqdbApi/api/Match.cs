using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    class Match : IComparable<Match>
    {
        public int Similarity { get; }
        public int Width { get; }
        public int Height { get; }
        public ServiceType Type { get; }
        public String Url { get; }

        public Match(int similarity, int width, int height, ServiceType type, string url)
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
