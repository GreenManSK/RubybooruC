namespace IqdbApi.parsers
{
    public interface Parser
    {
        ParseResult Parse(string url);
    }
}