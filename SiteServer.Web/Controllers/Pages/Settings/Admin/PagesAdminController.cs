using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using System.Linq;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/admin")]
    public class PagesAdminController : ApiController
    {
        private const string Route = "";
        private const string RoutePermissions = "permissions/{adminId:int}";
        private const string RouteLock = "actions/lock";
        private const string RouteUnLock = "actions/unLock";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var roles = new List<KeyValuePair<string, string>>();

                var roleNameList = await request.AdminPermissionsImpl.IsSuperAdminAsync() ? await DataProvider.RoleRepository.GetRoleNameListAsync() : await DataProvider.RoleRepository.GetRoleNameListByCreatorUserNameAsync(request.AdminName);

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

                var isSuperAdmin = await request.AdminPermissions.IsSuperAdminAsync();
                var creatorUserName = isSuperAdmin ? string.Empty : request.AdminName;
                var count = await DataProvider.AdministratorRepository.GetCountAsync(creatorUserName, role, lastActivityDate, keyword);
                var administratorInfoList = await DataProvider.AdministratorRepository.GetAdministratorsAsync(creatorUserName, role, order, lastActivityDate, keyword, offset, limit);
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
                        Roles = await DataProvider.AdministratorRepository.GetRolesAsync(administratorInfo.UserName)
                    });
                }

                return Ok(new
                {
                    Value = administrators,
                    Count = count,
                    Roles = roles,
                    IsSuperAdmin = await request.AdminPermissions.IsSuperAdminAsync(),
                    request.AdminId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RoutePermissions)]
        public async Task<IHttpActionResult> GetPermissions(int adminId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                if (!await request.AdminPermissions.IsSuperAdminAsync())
                {
                    return Unauthorized();
                }

                var roles = await DataProvider.RoleRepository.GetRoleNameListAsync();
                roles = roles.Where(x => !EPredefinedRoleUtils.IsPredefinedRole(x)).ToList();
                var allSites = await DataProvider.SiteRepository.GetSiteListAsync();

                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(adminId);
                var adminRoles = await DataProvider.AdministratorsInRolesRepository.GetRolesForUserAsync(adminInfo.UserName);
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
                    checkedSites = adminInfo.SiteIds;
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
        public async Task<IHttpActionResult> SavePermissions(int adminId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                if (!await request.AdminPermissions.IsSuperAdminAsync())
                {
                    return Unauthorized();
                }

                var adminLevel = request.GetPostString("adminLevel");
                var checkedSites = request.GetPostObject<List<int>>("checkedSites");
                var checkedRoles = request.GetPostObject<List<string>>("checkedRoles");

                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(adminId);

                await DataProvider.AdministratorsInRolesRepository.RemoveUserAsync(adminInfo.UserName);
                if (adminLevel == "SuperAdmin")
                {
                    await DataProvider.AdministratorsInRolesRepository.AddUserToRoleAsync(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
                }
                else if (adminLevel == "SiteAdmin")
                {
                    await DataProvider.AdministratorsInRolesRepository.AddUserToRoleAsync(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.SystemAdministrator));
                }
                else
                {
                    await DataProvider.AdministratorsInRolesRepository.AddUserToRoleAsync(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator));
                    await DataProvider.AdministratorsInRolesRepository.AddUserToRolesAsync(adminInfo.UserName, checkedRoles.ToArray());
                }

                await DataProvider.AdministratorRepository.UpdateSiteIdCollectionAsync(adminInfo,
                    adminLevel == "SiteAdmin"
                        ? checkedSites
                        : new List<int>());

                PermissionsImpl.ClearAllCache();

                await request.AddAdminLogAsync("设置管理员权限", $"管理员:{adminInfo.UserName}");

                return Ok(new
                {
                    Value = true,
                    Roles = await DataProvider.AdministratorRepository.GetRolesAsync(adminInfo.UserName)
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(id);
                await DataProvider.AdministratorsInRolesRepository.RemoveUserAsync(adminInfo.UserName);
                await DataProvider.AdministratorRepository.DeleteAsync(adminInfo.Id);

                await request.AddAdminLogAsync("删除管理员", $"管理员:{adminInfo.UserName}");

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
        public async Task<IHttpActionResult> Lock()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(id);

                await DataProvider.AdministratorRepository.LockAsync(new List<string>
                {
                    adminInfo.UserName
                });

                await request.AddAdminLogAsync("锁定管理员", $"管理员:{adminInfo.UserName}");

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
        public async Task<IHttpActionResult> UnLock()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(id);

                await DataProvider.AdministratorRepository.UnLockAsync(new List<string>
                {
                    adminInfo.UserName
                });

                await request.AddAdminLogAsync("解锁管理员", $"管理员:{adminInfo.UserName}");

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

        [HttpPost, Route(RouteImport)]
        public async Task<IHttpActionResult> Import()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var fileName = request.HttpRequest["fileName"];
                var fileCount = request.HttpRequest.Files.Count;
                if (fileCount == 0)
                {
                    return BadRequest("请选择有效的文件上传");
                }

                var file = request.HttpRequest.Files[0];
                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                var sExt = PathUtils.GetExtension(fileName);
                if (!StringUtils.EqualsIgnoreCase(sExt, ".xlsx"))
                {
                    return BadRequest("导入文件为Excel格式，请选择有效的文件上传");
                }

                var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                file.SaveAs(filePath);

                var errorMessage = string.Empty;
                var success = 0;
                var failure = 0;

                var sheet = ExcelUtils.GetDataTable(filePath);
                if (sheet != null)
                {
                    for (var i = 1; i < sheet.Rows.Count; i++) //行
                    {
                        if (i == 1) continue;

                        var row = sheet.Rows[i];

                        var userName = row[0].ToString().Trim();
                        var password = row[1].ToString().Trim();
                        var displayName = row[2].ToString().Trim();
                        var mobile = row[3].ToString().Trim();
                        var email = row[4].ToString().Trim();

                        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                        {
                            var (isValid, message) = await DataProvider.AdministratorRepository.InsertAsync(new Administrator
                            {
                                UserName = userName,
                                DisplayName = displayName,
                                Mobile = mobile,
                                Email = email
                            }, password);
                            if (!isValid)
                            {
                                failure++;
                                errorMessage = message;
                            }
                            else
                            {
                                success++;
                            }
                        }
                        else
                        {
                            failure++;
                        }
                    }
                }

                return Ok(new
                {
                    Value = true,
                    Success = success,
                    Failure = failure,
                    ErrorMessage = errorMessage
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteExport)]
        public async Task<IHttpActionResult> Export()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                const string fileName = "administrators.csv";
                var filePath = PathUtils.GetTemporaryFilesPath(fileName);

                await ExcelObject.CreateExcelFileForAdministratorsAsync(filePath);
                var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

                return Ok(new
                {
                    Value = downloadUrl
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
