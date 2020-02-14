using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/groupContentLayerAdd")]
    public partial class PagesGroupContentLayerAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

            var group = await DataProvider.ContentGroupRepository.GetAsync(request.SiteId, request.GroupId);

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

            if (await DataProvider.ContentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<ListResult>("保存失败，已存在相同名称的内容组！");
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

            var groupInfo = await DataProvider.ContentGroupRepository.GetAsync(request.SiteId, request.GroupId);

            if (groupInfo.GroupName != request.GroupName && await DataProvider.ContentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return Request.BadRequest<ListResult>("保存失败，已存在相同名称的内容组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await DataProvider.ContentGroupRepository.UpdateAsync(groupInfo);

            await auth.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{groupInfo.GroupName}");

            var groups = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }
    }
}
