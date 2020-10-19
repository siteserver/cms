using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpGet, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> ItemsGet([FromQuery] ItemsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }
    }
}
