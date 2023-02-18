using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class DashboardController
    {
        [HttpPost, Route(RouteDisconnect)]
        public async Task<ActionResult<BoolResult>> Disconnect()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            await _cloudManager.RemoveAuthenticationAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
