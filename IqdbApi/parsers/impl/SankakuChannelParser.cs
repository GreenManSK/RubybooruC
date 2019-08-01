using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace IqdbApi.parsers.impl
{
    public class SankakuChannelParser : AbstractParser
    {
        private const string ImageLinkSelector = "//*[@id=\"stats\"]//li[contains(text(),\"Original:\")]//a";
        private const string TagSelector = "//*[@id=\"tag-sidebar\"]//li";

        public override async Task<ParseResult> Parse(Uri url)
        {
            var doc = await CreateDoc(url);
            var image = GetImage(url, doc);
            var tags = GetTags(doc);

            return new ParseResult(tags, image, null);
        }

        private List<Tag> GetTags(HtmlDocument doc)
        {
            var result = new List<Tag>();
            var tags = doc.DocumentNode.SelectNodes(TagSelector);
            foreach (var tag in tags)
            {
                result.Add(new Tag(
                    GetTagType(tag.GetAttributeValue("class", "")),
                    tag.Descendants("a").First().InnerText
                ));
            }

            return result;
        }

        private TagType GetTagType(string tagClass)
        {
            switch (tagClass)
            {
                case "tag-type-artist":
                    return TagType.Artist;
                case "tag-type-character":
                    return TagType.Character;
                case "tag-type-copyright":
                    return TagType.Copyright;
                case "tag-type-medium":
                    return TagType.Medium;
                case "tag-type-meta":
                    return TagType.Meta;
                case "tag-type-studio":
                    return TagType.Studio;
                default:
                    return TagType.General;
            }
        }

        private Uri GetImage(Uri baseUrl, HtmlDocument doc)
        {
            var link = doc.DocumentNode.SelectSingleNode(ImageLinkSelector);
            return link != null ? new Uri(baseUrl, link.GetAttributeValue("href", "")) : null;
        }
    }
}