using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class TasksController
    {
        [HttpPost, Route(RouteEnable)]
        public async Task<ActionResult<BoolResult>> Enable([FromBody] IdRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var task = await _scheduledTaskRepository.GetAsync(request.Id);
            task.IsDisabled = !task.IsDisabled;
            await _scheduledTaskRepository.UpdateAsync(task);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
