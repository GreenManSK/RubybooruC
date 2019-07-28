using System;

namespace IqdbApi.api
{
    public class FileSizeLimitException : Exception
    {
        public FileSizeLimitException(string message) : base(message)
        {
        }
    }
}