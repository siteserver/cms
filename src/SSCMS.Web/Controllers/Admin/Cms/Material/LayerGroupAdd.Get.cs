using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerGroupAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<StringResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialMessage,
                AuthTypes.SitePermissions.MaterialImage,
                AuthTypes.SitePermissions.MaterialVideo,
                AuthTypes.SitePermissions.MaterialAudio,
                AuthTypes.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            var group = await _materialGroupRepository.GetAsync(request.GroupId);

            return new StringResult
            {
                Value = group.GroupName
            };
        }
    }
}
