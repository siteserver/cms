using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class GroupContentLayerAddController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<ListResult>> Edit([FromBody] EditRequest request)
        {
            var groupInfo = await _contentGroupRepository.GetAsync(request.SiteId, request.GroupId);

            if (groupInfo.GroupName != request.GroupName && await _contentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的内容组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await _contentGroupRepository.UpdateAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{groupInfo.GroupName}");

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);
            var groupNames = groups.Select(x => x.GroupName);

            return new ListResult
            {
                GroupNames = groupNames,
                Groups = groups
            };
        }
    }
}
