using System.Collections.Generic;
using HtmlAgilityPack;

namespace IqdbApi.parsers.impl
{
    public class KonachanParser : MoebooruParser
    {
        protected override List<Tag> GetTags(HtmlDocument doc)
        {
            var result = new List<Tag>();

            var tagContainer = doc.GetElementbyId(TagContainerId);
            if (tagContainer == null)
            {
                return result;
            }

            var tags = tagContainer.Descendants("li");
            foreach (var tag in tags)
            {
                result.Add(new Tag(
                    GetTagType(tag.GetAttributeValue("data-type", "")),
                    tag.GetAttributeValue("data-name", "")
                ));
            }

            return result;
        }

        private static TagType GetTagType(string type)
        {
            switch (type)
            {
                case "copyright":
                    return TagType.Copyright;
                case "circle":
                    return TagType.Circle;
                case "character":
                    return TagType.Character;
                case "artist":
                    return TagType.Artist;
                case "style":
                    return TagType.Style;
                default:
                    return TagType.General;
            }
        }
    }
}