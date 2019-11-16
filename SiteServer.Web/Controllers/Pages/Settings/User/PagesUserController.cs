using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/user")]
    public class PagesUserController : ApiController
    {
        private const string Route = "";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";
        private const string RouteCheck = "actions/check";
        private const string RouteLock = "actions/lock";
        private const string RouteUnLock = "actions/unLock";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var groupInfoList = await UserGroupManager.GetUserGroupListAsync();

                var state = ETriStateUtils.GetEnumType(request.GetQueryString("state"));
                var groupId = request.GetQueryInt("groupId");
                var order = request.GetQueryString("order");
                var lastActivityDate = request.GetQueryInt("lastActivityDate");
                var keyword = request.GetQueryString("keyword");
                var offset = request.GetQueryInt("offset");
                var limit = request.GetQueryInt("limit");

                var count = await DataProvider.UserDao.GetCountAsync(state, groupId, lastActivityDate, keyword);
                var users = await DataProvider.UserDao.GetUsersAsync(state,groupId, lastActivityDate, keyword, order, offset, limit);

                return Ok(new
                {
                    Value = users,
                    Count = count,
                    Groups = groupInfoList
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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var user = await UserManager.GetUserByUserIdAsync(id);
                await DataProvider.UserDao.DeleteAsync(user);

                await request.AddAdminLogAsync("删除用户", $"用户:{user.UserName}");

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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
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
                            var (userId, message) = await DataProvider.UserDao.InsertAsync(new CMS.Model.User
                            {
                                UserName = userName,
                                DisplayName = displayName,
                                Mobile = mobile,
                                Email = email
                            }, password, string.Empty);
                            if (userId == 0)
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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                const string fileName = "users.csv";
                var filePath = PathUtils.GetTemporaryFilesPath(fileName);

                await ExcelObject.CreateExcelFileForUsersAsync(filePath, ETriState.All);
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

        [HttpPost, Route(RouteCheck)]
        public async Task<IHttpActionResult> Check()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                await DataProvider.UserDao.CheckAsync(new List<int>
                {
                    id
                });

                await request.AddAdminLogAsync("审核用户", $"用户Id:{id}");

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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var user = await UserManager.GetUserByUserIdAsync(id);

                await DataProvider.UserDao.LockAsync(new List<int>
                {
                    id
                });

                await request.AddAdminLogAsync("锁定用户", $"用户:{user.UserName}");

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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var user = await UserManager.GetUserByUserIdAsync(id);

                await DataProvider.UserDao.UnLockAsync(new List<int>
                {
                    id
                });

                await request.AddAdminLogAsync("解锁用户", $"用户:{user.UserName}");

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
