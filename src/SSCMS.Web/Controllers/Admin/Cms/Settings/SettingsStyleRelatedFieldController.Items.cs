using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        private const string RouteItems = "items";
        private const string RouteItemsOrder = "items/actions/order";

        [HttpGet, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> GetItems([FromQuery] ItemsRequest request)
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

        [HttpPost, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> AddItems([FromBody] ItemsAddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            foreach (var item in request.Items)
            {
                var itemInfo = new RelatedFieldItem
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    RelatedFieldId = request.RelatedFieldId,
                    Label = item.Key,
                    Value = item.Value,
                    ParentId = request.ParentId
                };
                await _relatedFieldItemRepository.InsertAsync(itemInfo);
            }

            await _authManager.AddAdminLogAsync("批量添加联动字段项");

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpPut, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> EditItem([FromBody] ItemsEditRequest request)
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

        [HttpDelete, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> DeleteItem([FromBody] ItemsDeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldItemRepository.DeleteAsync(request.SiteId, request.Id);

            await _authManager.AddSiteLogAsync(request.SiteId, "删除联动字段项");

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpPost, Route(RouteItemsOrder)]
        public async Task<ActionResult<ItemsResult>> OrderItem([FromBody] ItemsOrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.SettingsStyleRelatedField))
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
