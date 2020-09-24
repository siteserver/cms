using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class GroupChannelLayerAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<ListResult>> Add([FromBody] AddRequest request)
        {
            if (await _channelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的栏目组！");
            }

            var groupInfo = new ChannelGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await _channelGroupRepository.InsertAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "新增栏目组", $"栏目组:{groupInfo.GroupName}");

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }
    }
}
