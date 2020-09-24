using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class GroupChannelLayerAddController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<ListResult>> Edit([FromBody] EditRequest request)
        {
            var groupInfo = await _channelGroupRepository.GetAsync(request.SiteId, request.GroupId);

            if (groupInfo.GroupName != request.GroupName && await _channelGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的栏目组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await _channelGroupRepository.UpdateAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改栏目组", $"栏目组:{groupInfo.GroupName}");

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
