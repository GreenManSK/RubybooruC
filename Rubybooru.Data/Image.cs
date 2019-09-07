using System;
using System.Collections.Generic;
using IqdbApi.parsers;

namespace Rubybooru.Data
{
    public class Image
    {
        public long Size;
        public int Width;
        public int Height;
        public DateTime Created;
        
        public string Source;
        public string InfoSource;
        public int InfoSourceSimilarity;
        public DateTime Fetched;
        public List<Tag> Tags;
        
        
    }
}