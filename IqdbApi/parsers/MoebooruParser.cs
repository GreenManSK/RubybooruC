using System;
using System.Linq;
using HtmlAgilityPack;

namespace IqdbApi.parsers
{
    public abstract class MoebooruParser : AbstractParser
    {
        protected const string TagContainerId = "tag-sidebar";
        private const string StatsContainerId = "stats";
        private const string ImageLinkOriginalClass = "original-file-unchanged";
        private const string ImageLinkChangedClass = "original-file-changed";

        private const string SourceText = "Source";
        
        protected Uri GetImage(Uri baseUrl, HtmlDocument doc)
        {
            var link = doc.DocumentNode.SelectNodes($"//a[@class='{ImageLinkOriginalClass}']").First() ??
                       doc.DocumentNode.SelectNodes($"//a[@class='{ImageLinkChangedClass}']").First();
            return new Uri(baseUrl, link.GetAttributeValue("href", ""));
        }

        protected Uri GetSource(Uri baseUrl, HtmlDocument doc)
        {
            var infoRows = doc.GetElementbyId(StatsContainerId).Descendants("li");
            foreach (var row in infoRows)
            {
                if (row.InnerText.Contains(SourceText) && row.Descendants("a").Any())
                {
                    return new Uri(baseUrl, row.Descendants("a").First().GetAttributeValue("href", ""));
                }
            }
            return null;
        }
    }
}