using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsAccessTokensController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<TokensResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            await _accessTokenRepository.DeleteAsync(request.Id);
            var list = await _accessTokenRepository.GetAccessTokensAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
