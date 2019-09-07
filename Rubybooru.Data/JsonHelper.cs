using System;
using System.IO;
using IqdbApi.api;
using IqdbApi.parsers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rubybooru.Data
{
    public class JsonHelper
    {
        private const string FileExt = ".json";
        private static JsonSerializer _serializer = null;

        private JsonHelper()
        {
        }

        public static void SaveJson(string file, Match match, ParseResult result)
        {
            var image = new Image();

            image.Source = result.Source.ToString();
            image.InfoSourceSimilarity = match.Similarity;
            image.InfoSource = match.Url.ToString();
            image.Fetched = DateTime.Now;
            image.Tags = result.Tags;

            var fileInfo = new FileInfo(file);
            image.Size = fileInfo.Length;
            image.Created = fileInfo.CreationTime;

            var imageObj = System.Drawing.Image.FromFile(file);
            image.Width = imageObj.Width;
            image.Height = imageObj.Height;

            var jsonFile = $"{file}{FileExt}";

            using (StreamWriter sw = new StreamWriter(jsonFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                GetSerializer().Serialize(writer, image);
            }
        }

        private static JsonSerializer GetSerializer()
        {
            if (_serializer == null)
            {
                _serializer = new JsonSerializer();
                _serializer.Converters.Add(new JavaScriptDateTimeConverter());
            }

            return _serializer;
        }
    }
}