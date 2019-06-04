using System;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/adminAccessTokensViewLayer")]
    public class PagesAdminAccessTokensViewLayerController : ControllerBase
    {
        private const string RouteAccessTokens = "accessTokens/{id:int}";
        private const string RouteRegenerate = "regenerate/{id:int}";

        [HttpGet, Route(RouteAccessTokens)]
        public IHttpActionResult GetAccessToken(int id)
        {
            try
            {
                var request = GetRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var tokenInfo = DataProvider.AccessTokenDao.Get(id);
                var accessToken = TranslateUtils.DecryptStringBySecretKey(tokenInfo.Token);

                return Ok(new
                {
                    tokenInfo,
                    accessToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteRegenerate)]
        public IHttpActionResult Regenerate(int id)
        {
            try
            {
                var request = GetRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var accessTokenInfo = DataProvider.AccessTokenDao.Get(id);
                var accessToken = TranslateUtils.DecryptStringBySecretKey(DataProvider.AccessTokenDao.Regenerate(accessTokenInfo));

                return Ok(new
                {
                    Value = accessToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
