using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ComponentController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<MaterialComponent>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialComponent))
            {
                return Unauthorized();
            }

            var lib = await _materialComponentRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _materialComponentRepository.UpdateAsync(lib);

            return lib;
        }
    }
}