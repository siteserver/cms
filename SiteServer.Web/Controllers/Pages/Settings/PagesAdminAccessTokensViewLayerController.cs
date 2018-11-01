using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/adminAccessTokensViewLayer")]
    public class PagesAdminAccessTokensViewLayerController : ApiController
    {
        private const string RouteAccessTokens = "accessTokens/{id:int}";
        private const string RouteRegenerate = "regenerate/{id:int}";

        [HttpGet, Route(RouteAccessTokens)]
        public IHttpActionResult GetAccessToken(int id)
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var tokenInfo = DataProvider.AccessTokenDao.GetAccessTokenInfo(id);
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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var accessToken = TranslateUtils.DecryptStringBySecretKey(DataProvider.AccessTokenDao.Regenerate(id));

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
