using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class BackupController
    {
        [HttpPost, Route(RouteGetRestoreProgress)]
        public async Task<ActionResult<IntResult>> GetRestoreProgress([FromBody] GetRestoreProgressRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

             var progress = _cloudManager.GetRestoreProgress(request.RestoreId);

            return new IntResult
            {
                Value = progress,
            };
        }
    }
}
