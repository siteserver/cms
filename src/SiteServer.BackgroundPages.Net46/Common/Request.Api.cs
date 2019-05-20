using SiteServer.Plugin;
using SiteServer.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public bool IsApiAuthenticated { get; }

        public string ApiToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (TryGetHeader(Constants.AuthKeyApiHeader, out var headers))
                {
                    var header = headers.SingleOrDefault();
                    accessTokenStr = StringUtils.IsEncrypted(header) ? TranslateUtils.DecryptStringBySecretKey(header) : header;
                }
                else if (IsQueryExists(Constants.AuthKeyApiQuery))
                {
                    var query = GetQueryString(Constants.AuthKeyApiQuery);
                    accessTokenStr = StringUtils.IsEncrypted(query) ? TranslateUtils.DecryptStringBySecretKey(query) : query;
                }
                else if (TryGetCookie(Constants.AuthKeyApiCookie, out var cookie))
                {
                    accessTokenStr = cookie;
                }

                return accessTokenStr;
            }
        }
    }
}
