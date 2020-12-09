using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerGroupAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<StringResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage,
                MenuUtils.SitePermissions.MaterialImage,
                MenuUtils.SitePermissions.MaterialVideo,
                MenuUtils.SitePermissions.MaterialAudio,
                MenuUtils.SitePermissions.MaterialFile))
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
