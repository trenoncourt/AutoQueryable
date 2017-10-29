namespace AutoQueryable.Extensions
{
    public static class QueryStringExtensions
    {
        public static string[] GetParts(this string queryString)
        {
            return queryString.Replace("?", "").Split('&');
        }
    }
}