using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userMenu")]
    public class PagesUserMenuController : ApiController
    {
        private const string Route = "";
        private const string RouteReset = "actions/reset";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList(),
                    Groups = UserGroupManager.GetUserGroupInfoList()
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
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = Request.GetQueryInt("id");

                DataProvider.UserMenu.Delete(id);

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit([FromBody] UserMenuInfo menuInfo)
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                if (menuInfo.Id == 0)
                {
                    DataProvider.UserMenu.Insert(menuInfo);

                    LogUtils.AddAdminLog(rest.AdminName, "新增用户菜单", $"用户菜单:{menuInfo.Text}");
                }
                else if (menuInfo.Id > 0)
                {
                    DataProvider.UserMenu.Update(menuInfo);

                    LogUtils.AddAdminLog(rest.AdminName, "修改用户菜单", $"用户菜单:{menuInfo.Text}");
                }

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteReset)]
        public IHttpActionResult Reset()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                foreach (var userMenuInfo in UserMenuManager.GetAllUserMenuInfoList())
                {
                    DataProvider.UserMenu.Delete(userMenuInfo.Id);
                }

                LogUtils.AddAdminLog(rest.AdminName, "重置用户菜单");

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
