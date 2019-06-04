using SiteServer.Plugin;
using SiteServer.Utils;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public bool IsApiAuthenticated { get; }

        public string ApiToken
        {
            get
            {
                var accessToken = string.Empty;
                if (HeaderManager.TryGet(HeaderManager.ApiKey, this, out var header))
                {
                    accessToken = header;
                }
                else if (QueryManager.TryGet(QueryManager.ApiKey, this, out var query))
                {
                    accessToken = query;
                }
                else if (CookieManager.TryGet(CookieManager.ApiKey, this, out var cookie))
                {
                    accessToken = cookie;
                }

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken) : accessToken;
            }
        }
    }
}
