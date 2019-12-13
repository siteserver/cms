using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userGroup")]
    public class PagesUserGroupController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserGroup))
                {
                    return Unauthorized();
                }

                var adminNames = (await DataProvider.AdministratorRepository.GetUserNameListAsync()).ToList();
                adminNames.Insert(0, string.Empty);

                return Ok(new
                {
                    Value = await UserGroupManager.GetUserGroupListAsync(),
                    AdminNames = adminNames
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserGroup))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                await DataProvider.UserGroupRepository.DeleteAsync(id);

                return Ok(new
                {
                    Value = await UserGroupManager.GetUserGroupListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit([FromBody] UserGroup itemObj)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserGroup))
                {
                    return Unauthorized();
                }

                if (itemObj.Id == -1)
                {
                    if (await UserGroupManager.IsExistsAsync(itemObj.GroupName))
                    {
                        return BadRequest("保存失败，已存在相同名称的用户组！");
                    }

                    var groupInfo = new UserGroup
                    {
                        GroupName = itemObj.GroupName,
                        AdminName = itemObj.AdminName
                    };

                    await DataProvider.UserGroupRepository.InsertAsync(groupInfo);

                    await request.AddAdminLogAsync("新增用户组", $"用户组:{groupInfo.GroupName}");
                }
                else if (itemObj.Id == 0)
                {
                    var config = await DataProvider.ConfigRepository.GetAsync();

                    config.UserDefaultGroupAdminName = itemObj.AdminName;

                    await DataProvider.ConfigRepository.UpdateAsync(config);

                    UserGroupManager.ClearCache();

                    await request.AddAdminLogAsync("修改用户组", "用户组:默认用户组");
                }
                else if (itemObj.Id > 0)
                {
                    var groupInfo = await UserGroupManager.GetUserGroupAsync(itemObj.Id);

                    if (groupInfo.GroupName != itemObj.GroupName && await UserGroupManager.IsExistsAsync(itemObj.GroupName))
                    {
                        return BadRequest("保存失败，已存在相同名称的用户组！");
                    }

                    groupInfo.GroupName = itemObj.GroupName;
                    groupInfo.AdminName = itemObj.AdminName;

                    await DataProvider.UserGroupRepository.UpdateAsync(groupInfo);

                    await request.AddAdminLogAsync("修改用户组", $"用户组:{groupInfo.GroupName}");
                }

                return Ok(new
                {
                    Value = await UserGroupManager.GetUserGroupListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
