using System.Collections.Generic;

namespace IqdbApi.parsers
{
    public class ParseResult
    {
        public List<Tag> Tags { get; }
        public string Image { get; }
        public string Source { get; }

        public ParseResult(List<Tag> tags, string image, string source)
        {
            Tags = tags;
            Image = image;
            Source = source;
        }
    }
}