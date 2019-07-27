using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    public class BasicIqdbApi : IqdbApi, IDisposable
    {

        public static readonly int MaxFileSize = 8388608;
        public static readonly int MaxImageWitdh = 7500;
        public static readonly int MaxImageHeight = 7500;
        public static readonly string URL = "https://iqdb.org/";

        private static readonly string FormUrl = "/";
        private static readonly string FieldService = "service[]";
        private static readonly string FieldFile = "file";
        private static readonly string FieldUrl = "url";
        private static readonly string FieldIgnoreColors = "forcegray";

        private readonly HttpClient httpClient;

        public BasicIqdbApi()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(URL);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<List<Match>> SearchFile(string path, Options options)
        {
            var fileSize = new System.IO.FileInfo(path).Length;
            if (fileSize > MaxFileSize) {
                throw new FileSizeLimitException($"File size {fileSize}bytes of {path} is too large for iqdb");
            }

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.IO.File.ReadAllBytes(path))), FieldFile, Path.GetFileName(path));

            return await Search(content, options);
        }

        public async Task<List<Match>> SearchUrl(string url, Options options)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(url), FieldUrl);
            return await Search(content, options);
        }

        private async Task<List<Match>> Search(MultipartFormDataContent content, Options options) {
            content.Add(new StringContent(options.IgnoreColors ? "1" : "0"), FieldIgnoreColors);
            foreach (var service in options.Services) {
                content.Add(new StringContent(service.Id.ToString()), FieldService);
            }

            var result = await httpClient.PostAsync(FormUrl, content);
            string resultContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine(resultContent);
            throw new NotImplementedException();
        }
    }
}
