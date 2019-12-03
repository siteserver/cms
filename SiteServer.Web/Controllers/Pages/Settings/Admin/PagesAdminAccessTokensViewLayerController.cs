using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminAccessTokensViewLayer")]
    public class PagesAdminAccessTokensViewLayerController : ApiController
    {
        private const string RouteAccessTokens = "accessTokens/{id:int}";
        private const string RouteRegenerate = "regenerate/{id:int}";

        [HttpGet, Route(RouteAccessTokens)]
        public async Task<IHttpActionResult> GetAccessToken(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var tokenInfo = await DataProvider.AccessTokenRepository.GetAsync(id);
                var accessToken = WebConfigUtils.DecryptStringBySecretKey(tokenInfo.Token);

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
        public async Task<IHttpActionResult> Regenerate(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var accessTokenInfo = await DataProvider.AccessTokenRepository.GetAsync(id);

                var accessToken = WebConfigUtils.DecryptStringBySecretKey(await DataProvider.AccessTokenRepository.RegenerateAsync(accessTokenInfo));

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
