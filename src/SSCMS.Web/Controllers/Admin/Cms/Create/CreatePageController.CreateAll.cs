using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreatePageController
    {
        [HttpPost, Route(RouteAll)]
        public async Task<ActionResult<BoolResult>> CreateAll([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.CreateAll))
            {
                return Unauthorized();
            }

            await _createManager.CreateByAllAsync(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
