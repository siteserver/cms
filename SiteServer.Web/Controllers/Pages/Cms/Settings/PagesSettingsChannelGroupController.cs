using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    
    [RoutePrefix("pages/cms/settings/settingsChannelGroup")]
    public partial class PagesSettingsChannelGroupController : ApiController
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

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupListAsync(request.SiteId);

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

            await DataProvider.ChannelGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupListAsync(request.SiteId);

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

            if (await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<GetResult>("保存失败，已存在相同名称的栏目组！");
            }

            var groupInfo = new ChannelGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await DataProvider.ChannelGroupRepository.InsertAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "新增栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupListAsync(request.SiteId);

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

            var groupInfo = await DataProvider.ChannelGroupRepository.GetAsync(request.SiteId, request.Id);

            if (groupInfo.GroupName != request.GroupName && await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<GetResult>("保存失败，已存在相同名称的栏目组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await DataProvider.ChannelGroupRepository.UpdateAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "修改栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupListAsync(request.SiteId);

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
                await DataProvider.ChannelGroupRepository.UpdateTaxisUpAsync(request.SiteId, request.GroupId, request.Taxis);
            }
            else
            {
                await DataProvider.ChannelGroupRepository.UpdateTaxisDownAsync(request.SiteId, request.GroupId, request.Taxis);
            }

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupListAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}
