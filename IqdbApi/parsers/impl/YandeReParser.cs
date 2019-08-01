using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace IqdbApi.parsers.impl
{
    public class YandeReParser : MoebooruParser
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
                    GetTagType(tag.GetAttributeValue("class", "")),
                    tag.Descendants("a").ElementAt(1).InnerText
                ));
            }

            return result;
        }

        private static TagType GetTagType(string type)
        {
            switch (type)
            {
                case "tag-type-copyright":
                    return TagType.Copyright;
                case "tag-type-circle":
                    return TagType.Circle;
                case "tag-type-character":
                    return TagType.Character;
                case "tag-type-artist":
                    return TagType.Artist;
                case "tag-type-faults":
                    return TagType.Faults;
                default:
                    return TagType.General;
            }
        }
    }
}