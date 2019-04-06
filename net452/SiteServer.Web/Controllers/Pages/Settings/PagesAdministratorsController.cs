using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SqlKata;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/administrators")]
    public class PagesAdministratorsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsLock = "actions/lock";
        private const string RouteActionsUnLock = "actions/unlock";
        private const string RouteActionsDelete = "actions/delete";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList()
        {
            try
            {
                var rest = new Rest(Request);

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                const int pageSize = StringUtils.Constants.PageSize;
                var page = rest.GetQueryInt("page", 1);

                var keyword = rest.GetQueryString("keyword");
                var roleName = rest.GetQueryString("roleName");
                var order = rest.GetQueryString("order");
                var departmentId = rest.GetQueryInt("departmentId");
                var areaId = rest.GetQueryInt("areaId");

                var query = new Query();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query.Where(q => q
                        .WhereLike(nameof(AdministratorInfo.UserName), keyword)
                        .OrWhereLike(nameof(AdministratorInfo.Email), keyword)
                        .OrWhereLike(nameof(AdministratorInfo.Mobile), keyword)
                        .OrWhereLike(nameof(AdministratorInfo.DisplayName), keyword)
                    );
                }

                if (!rest.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    query.Where(nameof(AdministratorInfo.CreatorUserName), rest.AdminName);
                }

                if (departmentId != 0)
                {
                    query.Where(nameof(AdministratorInfo.DepartmentId), departmentId);
                }

                if (areaId != 0)
                {
                    query.Where(nameof(AdministratorInfo.AreaId), areaId);
                }

                if (!string.IsNullOrEmpty(roleName))
                {
                    var userNameList = DataProvider.AdministratorsInRoles.GetUserNameListByRoleName(roleName);

                    query.WhereIn(nameof(AdministratorInfo.UserName), userNameList);
                }

                var count = DataProvider.Administrator.GetCount(query);
                var pages = Convert.ToInt32(Math.Ceiling((double)count / pageSize));
                if (pages == 0) pages = 1;

                var administratorInfoList = new List<AdministratorInfo>();

                if (count > 0)
                {
                    var offset = pageSize * (page - 1);
                    var tableColumns = DataProvider.Administrator.TableColumns;

                    var columnNames = tableColumns.Where(x =>
                        x.AttributeName != nameof(AdministratorInfo.Password) &&
                        x.AttributeName != nameof(AdministratorInfo.PasswordFormat) &&
                        x.AttributeName != nameof(AdministratorInfo.PasswordSalt)).Select(x => x.AttributeName);

                    query.Select(columnNames.ToArray()).Offset(offset).Limit(pageSize);
                    if (string.IsNullOrEmpty(order))
                    {
                        query.OrderBy(nameof(AdministratorInfo.Id));
                    }
                    else
                    {
                        if (StringUtils.EndsWithIgnoreCase(order, " DESC"))
                        {
                            query.OrderByDesc(StringUtils.ReplaceEndsWithIgnoreCase(order, " DESC", string.Empty));
                        }
                        else
                        {
                            query.OrderBy(order);
                        }
                    }

                    administratorInfoList = DataProvider.Administrator.GetAll(query).ToList();
                }

                var roles = RoleManager.GetRestRoles(rest.AdminPermissionsImpl.IsConsoleAdministrator, rest.AdminName);

                var departments = DepartmentManager.GetRestDepartments();
                var areas = AreaManager.GetRestAreas();

                var adminLockLoginType =
                    EUserLockTypeUtils.GetEnumType(ConfigManager.Instance.AdminLockLoginType);
                var isAdminLockLogin = ConfigManager.Instance.IsAdminLockLogin;
                var adminLockLoginCount = ConfigManager.Instance.AdminLockLoginCount;
                var adminLockLoginHours = ConfigManager.Instance.AdminLockLoginHours;

                var pageContents = new List<IDictionary<string, object>>();

                foreach (var administratorInfo in administratorInfoList)
                {
                    dynamic dynamicObj = administratorInfo;

                    var state = string.Empty;
                    if (administratorInfo.Locked)
                    {
                        state = "[已被锁定]";
                    }
                    else if (isAdminLockLogin && adminLockLoginCount <= administratorInfo.CountOfFailedLogin)
                    {
                        if (adminLockLoginType == EUserLockType.Forever)
                        {
                            state = @"<span style=""color:red;"">[已被锁定]</span>";
                        }
                        else if (administratorInfo.LastActivityDate.HasValue)
                        {
                            var ts = new TimeSpan(DateTime.Now.Ticks - administratorInfo.LastActivityDate.Value.Ticks);
                            var hours = Convert.ToInt32(adminLockLoginHours - ts.TotalHours);
                            if (hours > 0)
                            {
                                state = $@"<span style=""color:red;"">[错误登录次数过多，已被锁定{hours}小时]</span>";
                            }
                        }
                    }

                    dynamicObj.State = state;
                    dynamicObj.DepartmentName =
                        DepartmentManager.GetDepartmentName(administratorInfo.DepartmentId);
                    dynamicObj.AreaName =
                        AreaManager.GetAreaName(administratorInfo.AreaId);
                    if (administratorInfo.LastActivityDate.HasValue)
                    {
                        dynamicObj.LastLoginDate = DateUtils.ParseThisMoment(administratorInfo.LastActivityDate.Value);
                    }

                    dynamicObj.RoleName = AdminManager.GetRoleNames(administratorInfo.UserName);

                    dynamicObj.NotMe = administratorInfo.Id != rest.AdminId;

                    pageContents.Add((IDictionary<string, object>)dynamicObj);
                }
                

                return Ok(new
                {
                    Value = pageContents,
                    Count = count,
                    Pages = pages,
                    Roles = roles,
                    Departments = departments,
                    Areas = areas
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsDelete)]
        public IHttpActionResult Delete()
        {
            try
            {
                var rest = new Rest(Request);

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var adminIdList = TranslateUtils.StringCollectionToIntList(rest.GetPostString("adminIds"));
                adminIdList.Remove(rest.AdminId);

                foreach (var adminId in adminIdList)
                {
                    var adminInfo = AdminManager.GetAdminInfoByUserId(adminId);
                    DataProvider.Administrator.Delete(adminInfo);

                    rest.AddAdminLog("删除管理员", $"管理员:{adminInfo.UserName}");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLock)]
        public IHttpActionResult Lock()
        {
            try
            {
                var rest = new Rest(Request);

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var adminIdList = TranslateUtils.StringCollectionToIntList(rest.GetPostString("adminIds"));
                adminIdList.Remove(rest.AdminId);

                DataProvider.Administrator.Lock(adminIdList);
                rest.AddAdminLog("锁定管理员");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsUnLock)]
        public IHttpActionResult UnLock()
        {
            try
            {
                var rest = new Rest(Request);

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var adminIdList = TranslateUtils.StringCollectionToIntList(rest.GetPostString("adminIds"));
                adminIdList.Remove(rest.AdminId);

                DataProvider.Administrator.UnLock(adminIdList);
                rest.AddAdminLog("解锁管理员");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
