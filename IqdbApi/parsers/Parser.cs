using System;
using System.Threading.Tasks;

namespace IqdbApi.parsers
{
    public interface IParser
    {
        Task<ParseResult> Parse(Uri url);
    }
}