using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsAccessTokensViewLayer")]
    public partial class AdministratorsAccessTokensViewLayerController : ControllerBase
    {
        private const string Route = "";
        private const string RouteRegenerate = "actions/regenerate";

        private readonly IAuthManager _authManager;

        public AdministratorsAccessTokensViewLayerController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]int id)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var tokenInfo = await DataProvider.AccessTokenRepository.GetAsync(id);
            var accessToken = TranslateUtils.DecryptStringBySecretKey(tokenInfo.Token, WebConfigUtils.SecretKey);

            return new GetResult
            {
                Token = tokenInfo,
                AccessToken = accessToken
            };
        }

        [HttpPost, Route(RouteRegenerate)]
        public async Task<ActionResult<RegenerateResult>> Regenerate([FromBody]IdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var accessTokenInfo = await DataProvider.AccessTokenRepository.GetAsync(request.Id);

            var accessToken = TranslateUtils.DecryptStringBySecretKey(await DataProvider.AccessTokenRepository.RegenerateAsync(accessTokenInfo), WebConfigUtils.SecretKey);

            return new RegenerateResult
            {
                AccessToken = accessToken
            };
        }
    }
}
