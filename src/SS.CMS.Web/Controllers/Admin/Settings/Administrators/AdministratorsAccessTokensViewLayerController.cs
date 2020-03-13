using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsAccessTokensViewLayer")]
    public partial class AdministratorsAccessTokensViewLayerController : ControllerBase
    {
        private const string Route = "";
        private const string RouteRegenerate = "actions/regenerate";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public AdministratorsAccessTokensViewLayerController(ISettingsManager settingsManager, IAuthManager authManager, IAccessTokenRepository accessTokenRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _accessTokenRepository = accessTokenRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]int id)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var tokenInfo = await _accessTokenRepository.GetAsync(id);
            var accessToken = _settingsManager.Decrypt(tokenInfo.Token);

            return new GetResult
            {
                Token = tokenInfo,
                AccessToken = accessToken
            };
        }

        [HttpPost, Route(RouteRegenerate)]
        public async Task<ActionResult<RegenerateResult>> Regenerate([FromBody]IdRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var accessTokenInfo = await _accessTokenRepository.GetAsync(request.Id);

            var accessToken = _settingsManager.Decrypt(await _accessTokenRepository.RegenerateAsync(accessTokenInfo));

            return new RegenerateResult
            {
                AccessToken = accessToken
            };
        }
    }
}
