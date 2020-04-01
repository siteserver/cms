using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/admin")]
    public class PagesAdminController : ApiController
    {
        private const string Route = "";
        private const string RoutePermissions = "permissions/{adminId:int}";
        private const string RouteLock = "actions/lock";
        private const string RouteUnLock = "actions/unLock";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var roles = new List<KeyValuePair<string, string>>();

                var roleNameList = request.AdminPermissionsImpl.IsConsoleAdministrator ? DataProvider.RoleDao.GetRoleNameList() : DataProvider.RoleDao.GetRoleNameListByCreatorUserName(request.AdminName);

                var predefinedRoles = EPredefinedRoleUtils.GetAllPredefinedRoleName();
                foreach (var predefinedRole in predefinedRoles)
                {
                    roles.Add(new KeyValuePair<string, string>(predefinedRole, EPredefinedRoleUtils.GetText(EPredefinedRoleUtils.GetEnumType(predefinedRole))));
                }
                foreach (var roleName in roleNameList)
                {
                    if (!predefinedRoles.Contains(roleName))
                    {
                        roles.Add(new KeyValuePair<string, string>(roleName, roleName));
                    }
                }

                var role = request.GetQueryString("role");
                var order = request.GetQueryString("order");
                var lastActivityDate = request.GetQueryInt("lastActivityDate");
                var keyword = request.GetQueryString("keyword");
                var offset = request.GetQueryInt("offset");
                var limit = request.GetQueryInt("limit");

                var isSuperAdmin = request.AdminPermissions.IsSuperAdmin();
                var creatorUserName = isSuperAdmin ? string.Empty : request.AdminName;
                var count = DataProvider.AdministratorDao.GetCount(creatorUserName, role, order, lastActivityDate,
                    keyword);
                var administratorInfoList = DataProvider.AdministratorDao.GetAdministrators(creatorUserName, role, order, lastActivityDate, keyword, offset, limit);
                var administrators = new List<object>();
                foreach (var administratorInfo in administratorInfoList)
                {
                    administrators.Add(new
                    {
                        administratorInfo.Id,
                        administratorInfo.AvatarUrl,
                        administratorInfo.UserName,
                        DisplayName = string.IsNullOrEmpty(administratorInfo.DisplayName)
                            ? administratorInfo.UserName
                            : administratorInfo.DisplayName,
                        administratorInfo.Mobile,
                        administratorInfo.LastActivityDate,
                        administratorInfo.CountOfLogin,
                        administratorInfo.Locked,
                        Roles = AdminManager.GetRoles(administratorInfo.UserName)
                    });
                }

                return Ok(new
                {
                    Value = administrators,
                    Count = count,
                    Roles = roles,
                    IsSuperAdmin = request.AdminPermissions.IsSuperAdmin(),
                    request.AdminId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RoutePermissions)]
        public IHttpActionResult GetPermissions(int adminId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                if (!request.AdminPermissions.IsSuperAdmin())
                {
                    return Unauthorized();
                }

                var roles = DataProvider.RoleDao.GetRoleNameList().Where(x => !EPredefinedRoleUtils.IsPredefinedRole(x)).ToList();
                var allSites = SiteManager.GetSiteInfoList();

                var adminInfo = AdminManager.GetAdminInfoByUserId(adminId);
                var adminRoles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(adminInfo.UserName);
                string adminLevel;
                var checkedSites = new List<int>();
                var checkedRoles = new List<string>();
                if (EPredefinedRoleUtils.IsConsoleAdministrator(adminRoles))
                {
                    adminLevel = "SuperAdmin";
                }
                else if (EPredefinedRoleUtils.IsSystemAdministrator(adminRoles))
                {
                    adminLevel = "SiteAdmin";
                    checkedSites = TranslateUtils.StringCollectionToIntList(adminInfo.SiteIdCollection);
                }
                else
                {
                    adminLevel = "Admin";
                    foreach (var role in roles)
                    {
                        if (!checkedRoles.Contains(role) && !EPredefinedRoleUtils.IsPredefinedRole(role) && adminRoles.Contains(role))
                        {
                            checkedRoles.Add(role);
                        }
                    }
                }

                return Ok(new
                {
                    Value = true,
                    Roles = roles,
                    AllSites = allSites,
                    AdminLevel = adminLevel,
                    CheckedSites = checkedSites,
                    CheckedRoles = checkedRoles
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RoutePermissions)]
        public IHttpActionResult SavePermissions(int adminId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                if (!request.AdminPermissions.IsSuperAdmin())
                {
                    return Unauthorized();
                }

                var adminLevel = request.GetPostString("adminLevel");
                var checkedSites = request.GetPostObject<List<int>>("checkedSites");
                var checkedRoles = request.GetPostObject<List<string>>("checkedRoles");

                var adminInfo = AdminManager.GetAdminInfoByUserId(adminId);

                DataProvider.AdministratorsInRolesDao.RemoveUser(adminInfo.UserName);
                if (adminLevel == "SuperAdmin")
                {
                    DataProvider.AdministratorsInRolesDao.AddUserToRole(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
                }
                else if (adminLevel == "SiteAdmin")
                {
                    DataProvider.AdministratorsInRolesDao.AddUserToRole(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.SystemAdministrator));
                }
                else
                {
                    DataProvider.AdministratorsInRolesDao.AddUserToRole(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator));
                    DataProvider.AdministratorsInRolesDao.AddUserToRoles(adminInfo.UserName, checkedRoles.ToArray());
                }

                DataProvider.AdministratorDao.UpdateSiteIdCollection(adminInfo,
                    adminLevel == "SiteAdmin"
                        ? TranslateUtils.ObjectCollectionToString(checkedSites)
                        : string.Empty);

                PermissionsImpl.ClearAllCache();

                request.AddAdminLog("设置管理员权限", $"管理员:{adminInfo.UserName}");

                return Ok(new
                {
                    Value = true,
                    Roles = AdminManager.GetRoles(adminInfo.UserName)
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);
                DataProvider.AdministratorsInRolesDao.RemoveUser(adminInfo.UserName);
                DataProvider.AdministratorDao.Delete(adminInfo);

                request.AddAdminLog("删除管理员", $"管理员:{adminInfo.UserName}");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteLock)]
        public IHttpActionResult Lock()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);

                DataProvider.AdministratorDao.Lock(new List<string>
                {
                    adminInfo.UserName
                });

                request.AddAdminLog("锁定管理员", $"管理员:{adminInfo.UserName}");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUnLock)]
        public IHttpActionResult UnLock()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);

                DataProvider.AdministratorDao.UnLock(new List<string>
                {
                    adminInfo.UserName
                });

                request.AddAdminLog("解锁管理员", $"管理员:{adminInfo.UserName}");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
