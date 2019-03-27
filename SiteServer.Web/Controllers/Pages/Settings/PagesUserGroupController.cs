using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userGroup")]
    public class PagesUserGroupController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var adminNames = DataProvider.Administrator.GetUserNameList();
                adminNames.Insert(0, string.Empty);

                return Ok(new
                {
                    Value = UserGroupManager.GetUserGroupInfoList(),
                    AdminNames = adminNames
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = rest.GetQueryInt("id");

                DataProvider.UserGroup.Delete(id);

                return Ok(new
                {
                    Value = UserGroupManager.GetUserGroupInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = rest.GetPostInt("id");
                var groupName = rest.GetPostString("groupName");
                var adminName = rest.GetPostString("adminName");

                if (id == -1)
                {
                    if (UserGroupManager.IsExists(groupName))
                    {
                        return BadRequest("保存失败，已存在相同名称的用户组！");
                    }

                    var groupInfo = new UserGroupInfo
                    {
                        GroupName = groupName,
                        AdminName = adminName
                    };

                    DataProvider.UserGroup.Insert(groupInfo);

                    rest.AddAdminLog("新增用户组", $"用户组:{groupInfo.GroupName}");
                }
                else if (id == 0)
                {
                    ConfigManager.Instance.UserDefaultGroupAdminName = adminName;

                    DataProvider.Config.Update(ConfigManager.Instance);

                    UserGroupManager.ClearCache();

                    rest.AddAdminLog("修改用户组", "用户组:默认用户组");
                }
                else if (id > 0)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(id);

                    if (groupInfo.GroupName != groupName && UserGroupManager.IsExists(groupName))
                    {
                        return BadRequest("保存失败，已存在相同名称的用户组！");
                    }

                    groupInfo.GroupName = groupName;
                    groupInfo.AdminName = adminName;

                    DataProvider.UserGroup.Update(groupInfo);

                    rest.AddAdminLog("修改用户组", $"用户组:{groupInfo.GroupName}");
                }

                return Ok(new
                {
                    Value = UserGroupManager.GetUserGroupInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
