using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.V1;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/administrators")]
    public class V1AdministratorsController : ApiController
    {
        private const string Route = "";
        private const string RouteMe = "me";
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsLogout = "actions/logout";
        private const string RouteActionsResetPassword = "actions/resetPassword";
        private const string RouteAdministrator = "{id:int}";

        [HttpPost, Route(Route)]
        public IHttpActionResult Create()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(Request.GetApiToken(), AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = new AdministratorInfo
                {
                    UserName = Request.GetPostString(nameof(AdministratorInfo.UserName)),
                    Password = Request.GetPostString(nameof(AdministratorInfo.Password)),
                    CreationDate = DateTime.Now,
                    CreatorUserName = rest.UserName,
                    Locked = Request.GetPostBool(nameof(AdministratorInfo.Locked)),
                    SiteIdCollection = Request.GetPostString(nameof(AdministratorInfo.SiteIdCollection)),
                    SiteId = Request.GetPostInt(nameof(AdministratorInfo.SiteId)),
                    DepartmentId = Request.GetPostInt(nameof(AdministratorInfo.DepartmentId)),
                    AreaId = Request.GetPostInt(nameof(AdministratorInfo.AreaId)),
                    DisplayName = Request.GetPostString(nameof(AdministratorInfo.DisplayName)),
                    Mobile = Request.GetPostString(nameof(AdministratorInfo.Mobile)),
                    Email = Request.GetPostString(nameof(AdministratorInfo.Email)),
                    AvatarUrl = Request.GetPostString(nameof(AdministratorInfo.AvatarUrl))
                };

                var id = DataProvider.Administrator.Insert(adminInfo, out var errorMessage);
                if (id == 0)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteAdministrator)]
        public IHttpActionResult Update(int id)
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(Request.GetApiToken(), AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);

                if (adminInfo == null) return NotFound();

                if (Request.IsPostExists(nameof(AdministratorInfo.UserName)))
                {
                    adminInfo.UserName = Request.GetPostString(nameof(AdministratorInfo.UserName));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.Locked)))
                {
                    adminInfo.Locked = Request.GetPostBool(nameof(AdministratorInfo.Locked));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.SiteIdCollection)))
                {
                    adminInfo.SiteIdCollection = Request.GetPostString(nameof(AdministratorInfo.SiteIdCollection));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.SiteId)))
                {
                    adminInfo.SiteId = Request.GetPostInt(nameof(AdministratorInfo.SiteId));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.DepartmentId)))
                {
                    adminInfo.DepartmentId = Request.GetPostInt(nameof(AdministratorInfo.DepartmentId));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.AreaId)))
                {
                    adminInfo.AreaId = Request.GetPostInt(nameof(AdministratorInfo.AreaId));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.DisplayName)))
                {
                    adminInfo.DisplayName = Request.GetPostString(nameof(AdministratorInfo.DisplayName));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.Mobile)))
                {
                    adminInfo.Mobile = Request.GetPostString(nameof(AdministratorInfo.Mobile));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.Email)))
                {
                    adminInfo.Email = Request.GetPostString(nameof(AdministratorInfo.Email));
                }
                if (Request.IsPostExists(nameof(AdministratorInfo.AvatarUrl)))
                {
                    adminInfo.AvatarUrl = Request.GetPostString(nameof(AdministratorInfo.AvatarUrl));
                }

                var updated = DataProvider.Administrator.Update(adminInfo, out var errorMessage);
                if (!updated)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteAdministrator)]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(Request.GetApiToken(), AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);
                if (adminInfo == null) return NotFound();

                DataProvider.Administrator.Delete(adminInfo);

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteAdministrator)]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(Request.GetApiToken(), AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);

                if (adminInfo == null) return NotFound();

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteMe)]
        public IHttpActionResult GetSelf()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                if (!rest.IsAdminLoggin) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(rest.AdminId);

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var isApiAuthorized = AccessTokenManager.IsScope(Request.GetApiToken(), AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var top = Request.GetQueryInt("top", 20);
                var skip = Request.GetQueryInt("skip");

                var query = new Query().Limit(top).Offset(skip);
                var administrators = DataProvider.Administrator.GetAll(query);
                var count = DataProvider.Administrator.GetCount();

                return Ok(new PageResponse(administrators, top, skip, Request.RequestUri.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult Login()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var account = Request.GetPostString("account");
                var password = Request.GetPostString("password");
                var isAutoLogin = Request.GetPostBool("isAutoLogin");

                AdministratorInfo adminInfo;

                if (!DataProvider.Administrator.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                    if (adminInfo != null)
                    {
                        DataProvider.Administrator.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo); // 记录最后登录时间、失败次数+1
                    }
                    return BadRequest(errorMessage);
                }

                adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                DataProvider.Administrator.UpdateLastActivityDateAndCountOfLogin(adminInfo); // 记录最后登录时间、失败次数清零
                var accessToken = rest.AdminLogin(adminInfo.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

                return Ok(new
                {
                    Value = adminInfo,
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogout)]
        public IHttpActionResult Logout()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var adminInfo = AdminManager.GetAdminInfoByUserId(rest.AdminId);

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsResetPassword)]
        public IHttpActionResult ResetPassword()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(Request.GetApiToken(), AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var account = Request.GetPostString("account");
                var password = Request.GetPostString("password");
                var newPassword = Request.GetPostString("newPassword");

                if (!DataProvider.Administrator.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var adminInfo = AdminManager.GetAdminInfoByUserName(userName);

                if (!DataProvider.Administrator.ChangePassword(adminInfo, newPassword, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = adminInfo
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
