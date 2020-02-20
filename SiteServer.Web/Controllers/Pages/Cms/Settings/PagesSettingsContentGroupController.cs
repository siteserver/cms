using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    
    [RoutePrefix("pages/cms/settings/settingsContentGroup")]
    public partial class PagesSettingsContentGroupController : ApiController
    {
        private const string Route = "";
        private const string RouteOrder = "actions/order";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            var groups = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<GetResult> Delete([FromBody]DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            await DataProvider.ContentGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPost, Route(Route)]
        public async Task<GetResult> Add([FromBody] ChannelGroup request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            if (await DataProvider.ContentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<GetResult>("保存失败，已存在相同名称的内容组！");
            }

            var groupInfo = new ContentGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await DataProvider.ContentGroupRepository.InsertAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "新增内容组", $"内容组:{groupInfo.GroupName}");

            var groups = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPut, Route(Route)]
        public async Task<GetResult> Edit([FromBody] ChannelGroup request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            var groupInfo = await DataProvider.ContentGroupRepository.GetAsync(request.SiteId, request.Id);

            if (groupInfo.GroupName != request.GroupName && await DataProvider.ContentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<GetResult>("保存失败，已存在相同名称的内容组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await DataProvider.ContentGroupRepository.UpdateAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{groupInfo.GroupName}");

            var groups = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPost, Route(RouteOrder)]
        public async Task<GetResult> Order([FromBody] OrderRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            if (request.IsUp)
            {
                await DataProvider.ContentGroupRepository.UpdateTaxisUpAsync(request.SiteId, request.GroupId, request.Taxis);
            }
            else
            {
                await DataProvider.ContentGroupRepository.UpdateTaxisDownAsync(request.SiteId, request.GroupId, request.Taxis);
            }

            var groups = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}
