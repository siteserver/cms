using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Cloud
{
    public partial class DashboardController
    {
        [HttpPost, Route(RouteActionsDisconnect)]
        public async Task<ActionResult<BoolResult>> Disconnect()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            await _cloudManager.RemoveAuthentication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
