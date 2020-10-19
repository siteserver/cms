using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPut, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> ItemsEdit([FromBody] ItemsEditRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            var item = await _relatedFieldItemRepository.GetAsync(request.SiteId, request.Id);
            item.Label = request.Label;
            item.Value = request.Value;

            await _relatedFieldItemRepository.UpdateAsync(item);

            await _authManager.AddAdminLogAsync("编辑联动字段项");

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }
    }
}
