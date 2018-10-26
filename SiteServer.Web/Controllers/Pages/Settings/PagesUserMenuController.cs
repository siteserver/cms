using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;

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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                DataProvider.UserMenuDao.Delete(id);

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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                if (menuInfo.Id == 0)
                {
                    DataProvider.UserMenuDao.Insert(menuInfo);

                    request.AddAdminLog("新增用户菜单", $"用户菜单:{menuInfo.Text}");
                }
                else if (menuInfo.Id > 0)
                {
                    DataProvider.UserMenuDao.Update(menuInfo);

                    request.AddAdminLog("修改用户菜单", $"用户菜单:{menuInfo.Text}");
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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                foreach (var userMenuInfo in UserMenuManager.GetAllUserMenuInfoList())
                {
                    DataProvider.UserMenuDao.Delete(userMenuInfo.Id);
                }

                request.AddAdminLog("重置用户菜单");

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
