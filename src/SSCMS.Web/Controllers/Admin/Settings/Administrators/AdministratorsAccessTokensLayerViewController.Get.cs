using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsAccessTokensLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int id)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsAccessTokens))
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
    }
}
