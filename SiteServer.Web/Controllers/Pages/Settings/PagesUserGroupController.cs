using System;
using System.Web.Http;
using SiteServer.CMS.Database.Caches;
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
        public IHttpActionResult Submit([FromBody] UserGroupInfo itemObj)
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                if (itemObj.Id == -1)
                {
                    if (UserGroupManager.IsExists(itemObj.GroupName))
                    {
                        return BadRequest("保存失败，已存在相同名称的用户组！");
                    }

                    var groupInfo = new UserGroupInfo
                    {
                        GroupName = itemObj.GroupName,
                        AdminName = itemObj.AdminName
                    };

                    DataProvider.UserGroup.Insert(groupInfo);

                    rest.AddAdminLog("新增用户组", $"用户组:{groupInfo.GroupName}");
                }
                else if (itemObj.Id == 0)
                {
                    ConfigManager.Instance.SystemExtend.UserDefaultGroupAdminName = itemObj.AdminName;

                    DataProvider.Config.Update(ConfigManager.Instance);

                    UserGroupManager.ClearCache();

                    rest.AddAdminLog("修改用户组", "用户组:默认用户组");
                }
                else if (itemObj.Id > 0)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(itemObj.Id);

                    if (groupInfo.GroupName != itemObj.GroupName && UserGroupManager.IsExists(itemObj.GroupName))
                    {
                        return BadRequest("保存失败，已存在相同名称的用户组！");
                    }

                    groupInfo.GroupName = itemObj.GroupName;
                    groupInfo.AdminName = itemObj.AdminName;

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
