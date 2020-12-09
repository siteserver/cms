using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(RouteItemsOrder)]
        public async Task<ActionResult<ItemsResult>> ItemsOrder([FromBody] ItemsOrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            var item = await _relatedFieldItemRepository.GetAsync(request.SiteId, request.Id);

            if (request.Up)
            {
                await _relatedFieldItemRepository.UpdateTaxisToUpAsync(request.SiteId, request.RelatedFieldId, item.Id, item.ParentId);
            }
            else
            {
                await _relatedFieldItemRepository.UpdateTaxisToDownAsync(request.SiteId, request.RelatedFieldId, item.Id, item.ParentId);
            }

            await _authManager.AddAdminLogAsync("排序联动字段项");

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }
    }
}
