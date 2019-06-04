using SS.CMS.Plugin;

namespace SS.CMS.Core.Common
{
    public static class QueryManager
    {
        public const string UserToken = "userToken";
        public const string AdminToken = "adminToken";
        public const string ApiKey = "apiKey";

        public static bool TryGet(string key, IRequest request, out string accessToken)
        {
            accessToken = null;
            if (!request.TryGetQuery(key, out var value)) return false;
            accessToken = value;
            return true;
        }
    }
}
