using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/groupChannelLayerAdd")]
    public partial class PagesGroupChannelLayerAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

            var group = await DataProvider.ChannelGroupRepository.GetAsync(request.SiteId, request.GroupId);

            return new GetResult
            {
                GroupName = group.GroupName,
                Description = group.Description
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ListResult> Add([FromBody] AddRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<ListResult>();

            if (await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<ListResult>("保存失败，已存在相同名称的栏目组！");
            }

            var groupInfo = new ChannelGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await DataProvider.ChannelGroupRepository.InsertAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "新增栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ListResult> Edit([FromBody] EditRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<ListResult>();

            var groupInfo = await DataProvider.ChannelGroupRepository.GetAsync(request.SiteId, request.GroupId);

            if (groupInfo.GroupName != request.GroupName && await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<ListResult>("保存失败，已存在相同名称的栏目组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await DataProvider.ChannelGroupRepository.UpdateAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "修改栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }
    }
}
