using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/user")]
    public class PagesUserController : ApiController
    {
        private const string Route = "";
        private const string RouteExport = "actions/export";
        private const string RouteCheck = "actions/check";
        private const string RouteLock = "actions/lock";
        private const string RouteUnLock = "actions/unLock";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var groupInfoList = UserGroupManager.GetUserGroupInfoList();

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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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

        [HttpPost, Route(RouteExport)]
        public async Task<IHttpActionResult> Export()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                const string fileName = "users.csv";
                var filePath = PathUtils.GetTemporaryFilesPath(fileName);

                await ExcelObject.CreateExcelFileForUsersAsync(filePath, ETriState.All);
                var downloadUrl = ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath);

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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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
