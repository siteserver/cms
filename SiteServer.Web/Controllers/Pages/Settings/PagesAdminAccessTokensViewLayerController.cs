using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminAccessTokens))
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminAccessTokens))
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
