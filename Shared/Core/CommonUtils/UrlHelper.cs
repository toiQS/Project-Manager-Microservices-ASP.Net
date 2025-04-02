namespace Shared.Core.CommonUtils
{
    using System.Web;

    public static class UrlHelper
    {
        public static string Encode(string url) => HttpUtility.UrlEncode(url);
        public static string Decode(string url) => HttpUtility.UrlDecode(url);

        public static string AppendQueryString(string url, Dictionary<string, string> queryParams)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var param in queryParams)
                query[param.Key] = param.Value;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }

}
