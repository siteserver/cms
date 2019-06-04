using System;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Common
{
    public static class HeaderManager
    {
        public const string UserToken = "X-SS-USER-TOKEN";
        public const string AdminToken = "X-SS-ADMIN-TOKEN";
        public const string ApiKey = "X-SS-API-KEY";

        public static void Set(string key, IResponse response, string accessToken, DateTimeOffset? expires)
        {
            response.Headers.Remove(key);
            response.Headers.Add(key, accessToken);
        }

        public static void Set(string key, IResponse response, string accessToken)
        {
            response.Headers.Remove(key);
            response.Headers.Add(key, accessToken);
        }

        public static void Delete(string key, IResponse response)
        {
            response.Headers.Remove(key);
        }

        public static bool TryGet(string key, IRequest request, out string accessToken)
        {
            accessToken = null;
            if (!request.Headers.TryGetValue(key, out var value)) return false;
            accessToken = value;
            return true;
        }
    }
}
