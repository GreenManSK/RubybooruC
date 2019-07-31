using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.parsers.impl
{
    class GelbooruParser // : AbstractParser
    {
        private const string SourceSelector = "//li[contains(text(),\"Source:\")]";
        private const string ImageLinkSelector = "//li[contains(text(),\"Original image\")]";
        private const string TagSelector = "li[contains(@class, 'tag-type-')]";
    }
}
