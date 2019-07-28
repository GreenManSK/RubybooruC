using HtmlAgilityPack;

namespace IqdbApi.parsers
{
    public abstract class AbstractParser: Parser
    {
        public abstract ParseResult Parse(string url);

        protected HtmlDocument CreateDoc(string url)
        {
            var doc = new HtmlDocument();
            doc.Load(url);
            return doc;
        }
    }
}