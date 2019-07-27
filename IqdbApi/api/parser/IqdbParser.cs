using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IqdbApi.api.parser
{
    class IqdbParser
    {
        private static readonly string ContainerId = "pages";

        // Number of tr rows in valid match HTML div
        private static readonly int ValidRowCount = 5;

        private static readonly int ImageRow = 1;
        private static readonly int ServiceRow = 2;
        private static readonly int SizeRow = 3;
        private static readonly int SimilarityRow = 4;

        private static readonly string NoMatchText = "No relevant matches";
        
        private static readonly string SimilarityRegex = @"(\d+)";
        private static readonly string WidthRegex = @"(\d+)×";
        private static readonly string HeightRegex = @"×(\d+)";

        public static List<Match> ParseHtml(Uri baseUrl, string html) {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = new List<Match>();

            var pageContainer = doc.GetElementbyId(ContainerId);
            if (pageContainer == null || html.Contains(NoMatchText)) {
                return result;
            }

            var matches = pageContainer.Elements("div");
            foreach (var match in matches)
            {
                var rows = match.Descendants("tr");
                if (rows.Count() != ValidRowCount || match.Descendants("img").Count() == 0)
                {
                    continue;
                }

                int similarity = int.Parse(Extract(rows.ElementAt(SimilarityRow).InnerText, SimilarityRegex));
                int width = 0, height = 0;
                int.TryParse(Extract(rows.ElementAt(SizeRow).InnerText, WidthRegex), out width);
                int.TryParse(Extract(rows.ElementAt(SizeRow).InnerText, HeightRegex), out height);

                Uri url = new Uri(baseUrl, rows.ElementAt(ImageRow).Descendants("a").First().GetAttributeValue("href", ""));
                ServiceType type = ServiceType.GetTypeByUrl(url);

                result.Add(new Match(similarity, width, height, type, url));
            }

            result.Sort();
            result.Reverse();
            return result;
        }

        private static string Extract(string text, string pattern)
        {
            var matches = Regex.Match(text, pattern);
            if (!matches.Success)
            {
                return "";
            }
            return matches.Groups[1].Value;
        }
    }
}
