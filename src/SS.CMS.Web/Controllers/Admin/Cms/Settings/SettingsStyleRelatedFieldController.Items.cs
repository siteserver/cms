using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        private const string RouteItems = "items";
        private const string RouteItemsOrder = "items/actions/order";

        [HttpGet, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> GetItems([FromQuery] ItemsRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Unauthorized();
            }

            var tree = await DataProvider.RelatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpPost, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> AddItems([FromBody] ItemsAddRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
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
                await DataProvider.RelatedFieldItemRepository.InsertAsync(itemInfo);
            }

            await auth.AddAdminLogAsync("批量添加联动字段项");

            var tree = await DataProvider.RelatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpPut, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> EditItem([FromBody] ItemsEditRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Unauthorized();
            }

            var item = await DataProvider.RelatedFieldItemRepository.GetAsync(request.SiteId, request.Id);
            item.Label = request.Label;
            item.Value = request.Value;

            await DataProvider.RelatedFieldItemRepository.UpdateAsync(item);

            await auth.AddAdminLogAsync("编辑联动字段项");

            var tree = await DataProvider.RelatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpDelete, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> DeleteItem([FromBody] ItemsDeleteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Unauthorized();
            }

            await DataProvider.RelatedFieldItemRepository.DeleteAsync(request.SiteId, request.Id);

            await auth.AddSiteLogAsync(request.SiteId, "删除联动字段项");

            var tree = await DataProvider.RelatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpPost, Route(RouteItemsOrder)]
        public async Task<ActionResult<ItemsResult>> OrderItem([FromBody] ItemsOrderRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Unauthorized();
            }

            var item = await DataProvider.RelatedFieldItemRepository.GetAsync(request.SiteId, request.Id);

            if (request.Up)
            {
                await DataProvider.RelatedFieldItemRepository.UpdateTaxisToUpAsync(request.SiteId, request.RelatedFieldId, item.Id, item.ParentId);
            }
            else
            {
                await DataProvider.RelatedFieldItemRepository.UpdateTaxisToDownAsync(request.SiteId, request.RelatedFieldId, item.Id, item.ParentId);
            }

            await auth.AddAdminLogAsync("排序联动字段项");

            var tree = await DataProvider.RelatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }
    }
}
