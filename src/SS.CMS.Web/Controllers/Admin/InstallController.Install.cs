using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Dto.Result;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteInstall)]
        public async Task<ActionResult<BoolResult>> Install([FromBody]InstallRequest request)
        {
            if (request.SecurityKey != _settingsManager.SecurityKey) return Unauthorized();

            var (success, errorMessage) = await _databaseManager.InstallAsync(_pluginManager, request.UserName, request.AdminPassword, request.Email, request.Mobile);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            await FileUtils.WriteTextAsync(_pathManager.GetWebRootPath("index.html"), Constants.Html5Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
