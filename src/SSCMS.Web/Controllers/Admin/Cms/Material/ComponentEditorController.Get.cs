using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ComponentEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.MaterialComponent))
            {
                return Unauthorized();
            }

            var component = new MaterialComponent();
            if (request.ComponentId > 0)
            {
                component = await _materialComponentRepository.GetAsync(request.ComponentId);
            }

            return new GetResult
            {
                Component = component
            };
        }
    }
}