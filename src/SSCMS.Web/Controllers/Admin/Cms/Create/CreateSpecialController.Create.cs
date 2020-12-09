using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreateSpecialController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateSpecials))
            {
                return Unauthorized();
            }

            foreach (var specialId in request.SpecialIds)
            {
                await _createManager.CreateSpecialAsync(request.SiteId, specialId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
