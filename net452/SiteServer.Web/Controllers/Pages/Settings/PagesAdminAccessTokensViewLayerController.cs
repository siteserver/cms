using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/adminAccessTokensViewLayer")]
    public class PagesAdminAccessTokensViewLayerController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = Request.GetQueryInt("id");

                var tokenInfo = DataProvider.AccessToken.Get(id);
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

        [HttpPost, Route(Route)]
        public IHttpActionResult Regenerate()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = Request.GetPostInt("id");
                var accessTokenInfo = DataProvider.AccessToken.Get(id);

                var accessToken = TranslateUtils.DecryptStringBySecretKey(DataProvider.AccessToken.Regenerate(accessTokenInfo));

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
