using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace IqdbApi.parsers.impl
{
    public class DanbooruParser : AbstractParser
    {
        private const string InfoSectionId = "post-information";
        private const string TagNameElementClass = "search-tag";

        private const string CopyrightTagsClass = "copyright-tag-list";
        private const string CharacterTagsClass = "character-tag-list";
        private const string ArtistTagsClass = "artist-tag-list";
        private const string GeneralTagsClass = "general-tag-list";
        private const string MetaTagsClass = "meta-tag-list";

        private static readonly string[] TagClasses =
        {
            CopyrightTagsClass,
            CharacterTagsClass,
            ArtistTagsClass,
            GeneralTagsClass,
            MetaTagsClass
        };

        private const string ImageLinkText = "Size";
        private const string SourceLinkText = "Source";

        public override async Task<ParseResult> Parse(Uri url)
        {
            var doc = await CreateDoc(url);

            var image = GetUrlByText(ImageLinkText, url, doc);
            var source = GetUrlByText(SourceLinkText, url, doc);
            var tags = GetTags(doc);

            return new ParseResult(tags, image, source);
        }

        private List<Tag> GetTags(HtmlDocument doc)
        {
            var tags = new List<Tag>();
            foreach (var tagClass in TagClasses)
            {
                tags.AddRange(GetTags(tagClass, TagClassToType(tagClass), doc));
            }

            return tags;
        }

        private List<Tag> GetTags(string tagClass, TagType type, HtmlDocument doc)
        {
            var tags = new List<Tag>();

            var sections = doc.DocumentNode.SelectNodes($"//ul[@class='{tagClass}']");
            if (sections == null || sections.Count == 0)
                return tags;
            var section = sections.First();
            if (section == null)
                return tags;

            var tagContainers = section.Descendants("li");
            foreach (var container in tagContainers)
            {
                var nameElements = container.SelectNodes($"a[@class='{TagNameElementClass}']");
                if (nameElements == null || !nameElements.Any())
                    continue;
                tags.Add(new Tag(type, nameElements.First().InnerText));
            }

            return tags;
        }

        private TagType TagClassToType(string tagClass)
        {
            switch (tagClass)
            {
                case CopyrightTagsClass:
                    return TagType.Copyright;
                case CharacterTagsClass:
                    return TagType.Character;
                case ArtistTagsClass:
                    return TagType.Artist;
                case MetaTagsClass:
                    return TagType.Meta;
                default:
                    return TagType.General;
            }
        }

        private Uri GetUrlByText(string text, Uri baseUrl, HtmlDocument doc)
        {
            var infoRows = doc.GetElementbyId(InfoSectionId).Descendants("li");
            foreach (var row in infoRows)
            {
                if (row.InnerText.Contains(text) && row.Descendants("a").Any())
                {
                    return new Uri(baseUrl, row.Descendants("a").First().GetAttributeValue("href", ""));
                }
            }

            return null;
        }
    }
}