using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersDepartmentController
    {
        [HttpPost, Route(RouteAppend)]
        public async Task<ActionResult<List<int>>> Append([FromBody] AppendRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersDepartment))
            {
                return Unauthorized();
            }

            var insertedNodeIdHashtable = new Hashtable { [1] = request.ParentId }; //key为部门的级别，1为第一级部门

            var departmentNames = request.Departments.Split('\n');
            var expandedNodeIds = new List<int>();
            foreach (var item in departmentNames)
            {
                if (string.IsNullOrEmpty(item)) continue;

                //count为部门的级别
                var count = StringUtils.GetStartCount('－', item) == 0 ? StringUtils.GetStartCount('-', item) : StringUtils.GetStartCount('－', item);
                var departmentName = item.Substring(count, item.Length - count);
                count++;

                if (!string.IsNullOrEmpty(departmentName) && insertedNodeIdHashtable.Contains(count))
                {
                    departmentName = departmentName.Trim();

                    var parentId = (int)insertedNodeIdHashtable[count];

                    var insertedNodeId = await _departmentRepository.InsertAsync(parentId, departmentName);
                    insertedNodeIdHashtable[count + 1] = insertedNodeId;
                    expandedNodeIds.Add(insertedNodeId);
                }
            }

            await _departmentRepository.RemoveListCacheAsync();

            return expandedNodeIds;
        }
    }
}
