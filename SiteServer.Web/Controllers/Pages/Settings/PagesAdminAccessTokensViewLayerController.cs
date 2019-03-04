using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = rest.GetQueryInt("id");

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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = rest.GetPostInt("id");
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
