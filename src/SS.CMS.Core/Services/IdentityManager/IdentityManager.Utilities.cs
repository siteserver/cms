using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class IdentityManager
    {
        private string GetApiToken()
        {
            var accessToken = string.Empty;

            if (_context.Request.Query.TryGetValue(Constants.QueryApiKey, out var query))
            {
                accessToken = query;
            }
            else if (_context.Request.Headers.TryGetValue(Constants.HeaderApiKey, out var header))
            {
                accessToken = header;
            }
            else if (_context.Request.Cookies.TryGetValue(Constants.CookieApiKey, out var cookie))
            {
                accessToken = cookie;
            }

            return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _settingsManager.SecretKey) : accessToken;
        }

        private string GetUserToken()
        {
            var accessToken = string.Empty;

            if (_context.Request.Query.TryGetValue(Constants.QueryUserToken, out var query))
            {
                accessToken = query;
            }
            else if (_context.Request.Headers.TryGetValue(Constants.HeaderUserToken, out var header))
            {
                accessToken = header;
            }
            else if (_context.Request.Cookies.TryGetValue(Constants.CookieUserToken, out var cookie))
            {
                accessToken = cookie;
            }

            return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _settingsManager.SecretKey) : accessToken;
        }

        private string GetAdminToken()
        {
            var accessToken = string.Empty;

            if (_context.Request.Query.TryGetValue(Constants.QueryAdminToken, out var query))
            {
                accessToken = query;
            }
            else if (_context.Request.Headers.TryGetValue(Constants.HeaderAdminToken, out var header))
            {
                accessToken = header;
            }
            else if (_context.Request.Cookies.TryGetValue(Constants.CookieAdminToken, out var cookie))
            {
                accessToken = cookie;
            }

            return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _settingsManager.SecretKey) : accessToken;
        }
    }
}
