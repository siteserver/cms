using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.V1;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
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
                var rest = new Rest(Request);
                var isApiAuthorized = rest.IsApiAuthenticated && AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = new AdministratorInfo
                {
                    UserName = rest.GetPostString(nameof(AdministratorInfo.UserName)),
                    Password = rest.GetPostString(nameof(AdministratorInfo.Password)),
                    CreationDate = DateTime.Now,
                    CreatorUserName = rest.UserName,
                    Locked = rest.GetPostBool(nameof(AdministratorInfo.Locked)),
                    SiteIdCollection = rest.GetPostString(nameof(AdministratorInfo.SiteIdCollection)),
                    SiteId = rest.GetPostInt(nameof(AdministratorInfo.SiteId)),
                    DepartmentId = rest.GetPostInt(nameof(AdministratorInfo.DepartmentId)),
                    AreaId = rest.GetPostInt(nameof(AdministratorInfo.AreaId)),
                    DisplayName = rest.GetPostString(nameof(AdministratorInfo.DisplayName)),
                    Mobile = rest.GetPostString(nameof(AdministratorInfo.Mobile)),
                    Email = rest.GetPostString(nameof(AdministratorInfo.Email)),
                    AvatarUrl = rest.GetPostString(nameof(AdministratorInfo.AvatarUrl))
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
                var rest = new Rest(Request);
                var isApiAuthorized = rest.IsApiAuthenticated && AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);

                if (adminInfo == null) return NotFound();

                if (rest.IsPostExists(nameof(AdministratorInfo.UserName)))
                {
                    adminInfo.UserName = rest.GetPostString(nameof(AdministratorInfo.UserName));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.Locked)))
                {
                    adminInfo.Locked = rest.GetPostBool(nameof(AdministratorInfo.Locked));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.SiteIdCollection)))
                {
                    adminInfo.SiteIdCollection = rest.GetPostString(nameof(AdministratorInfo.SiteIdCollection));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.SiteId)))
                {
                    adminInfo.SiteId = rest.GetPostInt(nameof(AdministratorInfo.SiteId));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.DepartmentId)))
                {
                    adminInfo.DepartmentId = rest.GetPostInt(nameof(AdministratorInfo.DepartmentId));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.AreaId)))
                {
                    adminInfo.AreaId = rest.GetPostInt(nameof(AdministratorInfo.AreaId));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.DisplayName)))
                {
                    adminInfo.DisplayName = rest.GetPostString(nameof(AdministratorInfo.DisplayName));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.Mobile)))
                {
                    adminInfo.Mobile = rest.GetPostString(nameof(AdministratorInfo.Mobile));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.Email)))
                {
                    adminInfo.Email = rest.GetPostString(nameof(AdministratorInfo.Email));
                }
                if (rest.IsPostExists(nameof(AdministratorInfo.AvatarUrl)))
                {
                    adminInfo.AvatarUrl = rest.GetPostString(nameof(AdministratorInfo.AvatarUrl));
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
                var rest = new Rest(Request);
                var isApiAuthorized = rest.IsApiAuthenticated && AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeAdministrators);
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
                var rest = new Rest(Request);
                var isApiAuthorized = rest.IsApiAuthenticated && AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeAdministrators);
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
                var rest = new Rest(Request);

                if (!rest.IsAdminLoggin) return Unauthorized();

                return Ok(new
                {
                    Value = rest.AdminInfo
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
                var rest = new Rest(Request);

                var isApiAuthorized = rest.IsApiAuthenticated && AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var top = rest.GetQueryInt("top", 20);
                var skip = rest.GetQueryInt("skip");

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
                var rest = new Rest(Request);

                var account = rest.GetPostString("account");
                var password = rest.GetPostString("password");
                var isAutoLogin = rest.GetPostBool("isAutoLogin");

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
                var expiresAt = DateTime.Now.AddDays(Rest.AccessTokenExpireDays);

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
                var rest = new Rest(Request);
                var adminInfo = rest.IsAdminLoggin ? rest.AdminInfo : null;
                rest.AdminLogout();

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
                var rest = new Rest(Request);
                var isApiAuthorized = rest.IsApiAuthenticated && AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var account = rest.GetPostString("account");
                var password = rest.GetPostString("password");
                var newPassword = rest.GetPostString("newPassword");

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
