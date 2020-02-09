using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsStyleRelatedFieldController
    {
        private const string RouteItems = "items";
        private const string RouteItemsOrder = "items/actions/order";

        [HttpGet, Route(RouteItems)]
        public async Task<ItemsResult> GetItems([FromUri] ItemsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ItemsResult>();
            }

            var tree = await DataProvider.RelatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }

        [HttpPost, Route(RouteItems)]
        public async Task<ItemsResult> AddItems([FromBody] ItemsAddRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ItemsResult>();
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
        public async Task<ItemsResult> EditItem([FromBody] ItemsEditRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ItemsResult>();
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
        public async Task<ItemsResult> DeleteItem([FromBody] ItemsDeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ItemsResult>();
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
        public async Task<ItemsResult> OrderItem([FromBody] ItemsOrderRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ItemsResult>();
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
