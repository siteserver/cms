using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;
using System.Linq;
using Datory;
using Namotion.Reflection;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

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
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
            {
                return Unauthorized();
            }

            var roles = new List<KeyValuePair<string, string>>();

            var roleNameList = await request.AdminPermissionsImpl.IsSuperAdminAsync() ? await DataProvider.RoleRepository.GetRoleNameListAsync() : await DataProvider.RoleRepository.GetRoleNameListByCreatorUserNameAsync(request.AdminName);

            var predefinedRoles = TranslateUtils.GetEnums<PredefinedRole>();
            foreach (var predefinedRole in predefinedRoles)
            {
                roles.Add(new KeyValuePair<string, string>(predefinedRole.GetValue(), predefinedRole.GetDisplayName()));
            }
            foreach (var roleName in roleNameList)
            {
                if (!roles.Any(x => StringUtils.EqualsIgnoreCase(x.Key, roleName)))
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

        [HttpGet, Route(RoutePermissions)]
        public async Task<IHttpActionResult> GetPermissions(int adminId)
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
            roles = roles.Where(x => !DataProvider.RoleRepository.IsPredefinedRole(x)).ToList();
            var allSites = await DataProvider.SiteRepository.GetSiteListAsync();

            var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(adminId);
            var adminRoles = await DataProvider.AdministratorsInRolesRepository.GetRolesForUserAsync(adminInfo.UserName);
            string adminLevel;
            var checkedSites = new List<int>();
            var checkedRoles = new List<string>();
            if (DataProvider.RoleRepository.IsConsoleAdministrator(adminRoles))
            {
                adminLevel = "SuperAdmin";
            }
            else if (DataProvider.RoleRepository.IsSystemAdministrator(adminRoles))
            {
                adminLevel = "SiteAdmin";
                checkedSites = adminInfo.SiteIds;
            }
            else
            {
                adminLevel = "Admin";
                foreach (var role in roles)
                {
                    if (!checkedRoles.Contains(role) && !DataProvider.RoleRepository.IsPredefinedRole(role) && adminRoles.Contains(role))
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

        [HttpPost, Route(RoutePermissions)]
        public async Task<IHttpActionResult> SavePermissions(int adminId)
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
                await DataProvider.AdministratorRepository.AddUserToRoleAsync(adminInfo.UserName, PredefinedRole.ConsoleAdministrator.GetValue());
            }
            else if (adminLevel == "SiteAdmin")
            {
                await DataProvider.AdministratorRepository.AddUserToRoleAsync(adminInfo.UserName, PredefinedRole.SystemAdministrator.GetValue());
            }
            else
            {
                await DataProvider.AdministratorRepository.AddUserToRoleAsync(adminInfo.UserName, PredefinedRole.Administrator.GetValue());
                await DataProvider.AdministratorRepository.AddUserToRolesAsync(adminInfo.UserName, checkedRoles.ToArray());
            }

            await DataProvider.AdministratorRepository.UpdateSiteIdsAsync(adminInfo,
                adminLevel == "SiteAdmin"
                    ? checkedSites
                    : new List<int>());

            CacheUtils.ClearAll();

            await request.AddAdminLogAsync("设置管理员权限", $"管理员:{adminInfo.UserName}");

            return Ok(new
            {
                Value = true,
                Roles = await DataProvider.AdministratorRepository.GetRolesAsync(adminInfo.UserName)
            });
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
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

        [HttpPost, Route(RouteLock)]
        public async Task<IHttpActionResult> Lock()
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

        [HttpPost, Route(RouteUnLock)]
        public async Task<IHttpActionResult> UnLock()
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

        [HttpPost, Route(RouteImport)]
        public async Task<IHttpActionResult> Import()
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

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
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

        [HttpPost, Route(RouteExport)]
        public async Task<IHttpActionResult> Export()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
            {
                return Unauthorized();
            }

            const string fileName = "administrators.csv";
            var filePath = PathUtility.GetTemporaryFilesPath(fileName);

            await ExcelObject.CreateExcelFileForAdministratorsAsync(filePath);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return Ok(new
            {
                Value = downloadUrl
            });
        }
    }
}
