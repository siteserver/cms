using System.Threading.Tasks;
using Datory.Caching;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ActionsController
    {
        [OpenApiOperation("重启系统 API", "重启系统，使用POST发起请求，请求地址为/api/v1/actions/restart。")]
        [HttpPost, Route(RouteRestart)]
        public async Task<ActionResult<BoolResult>> Restart()
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeOthers))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }
            
            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
