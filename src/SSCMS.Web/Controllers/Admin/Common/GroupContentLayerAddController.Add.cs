using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class GroupContentLayerAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<ListResult>> Add([FromBody] AddRequest request)
        {
            if (await _contentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的内容组！");
            }

            var groupInfo = new ContentGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await _contentGroupRepository.InsertAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "新增内容组", $"内容组:{groupInfo.GroupName}");

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
