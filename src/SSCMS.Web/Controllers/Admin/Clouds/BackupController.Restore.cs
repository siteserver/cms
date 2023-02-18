using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class BackupController
    {
        [HttpPost, Route(RouteRestore)]
        public async Task<ActionResult<StringResult>> Restore([FromBody] RestoreRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var restoreId = StringUtils.Guid();
            _taskManager.Queue(async cancel =>
            {
                await _cloudManager.RestoreAsync(restoreId, request.BackupId);
            });

            return new StringResult
            {
                Value = restoreId
            };
        }
    }
}
