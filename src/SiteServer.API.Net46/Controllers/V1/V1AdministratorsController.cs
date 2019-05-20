using System;
using System.Web.Http;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.BackgroundPages.Common;
using SiteServer.API.Common;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/administrators")]
    public class V1AdministratorsController : ControllerBase
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
                var request = GetRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = new AdministratorInfo
                {
                    UserName = request.GetPostString(nameof(AdministratorInfo.UserName)),
                    Password = request.GetPostString(nameof(AdministratorInfo.Password)),
                    CreationDate = DateTime.Now,
                    CreatorUserName = request.UserName,
                    Locked = request.GetPostBool(nameof(AdministratorInfo.Locked)),
                    SiteIdCollection = request.GetPostString(nameof(AdministratorInfo.SiteIdCollection)),
                    SiteId = request.GetPostInt(nameof(AdministratorInfo.SiteId)),
                    DepartmentId = request.GetPostInt(nameof(AdministratorInfo.DepartmentId)),
                    AreaId = request.GetPostInt(nameof(AdministratorInfo.AreaId)),
                    DisplayName = request.GetPostString(nameof(AdministratorInfo.DisplayName)),
                    Mobile = request.GetPostString(nameof(AdministratorInfo.Mobile)),
                    Email = request.GetPostString(nameof(AdministratorInfo.Email)),
                    AvatarUrl = request.GetPostString(nameof(AdministratorInfo.AvatarUrl))
                };

                var id = DataProvider.AdministratorDao.Insert(adminInfo, out var errorMessage);
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
                var request = GetRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);

                if (adminInfo == null) return NotFound();

                if (request.IsPostExists(nameof(AdministratorInfo.UserName)))
                {
                    adminInfo.UserName = request.GetPostString(nameof(AdministratorInfo.UserName));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.Locked)))
                {
                    adminInfo.Locked = request.GetPostBool(nameof(AdministratorInfo.Locked));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.SiteIdCollection)))
                {
                    adminInfo.SiteIdCollection = request.GetPostString(nameof(AdministratorInfo.SiteIdCollection));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.SiteId)))
                {
                    adminInfo.SiteId = request.GetPostInt(nameof(AdministratorInfo.SiteId));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.DepartmentId)))
                {
                    adminInfo.DepartmentId = request.GetPostInt(nameof(AdministratorInfo.DepartmentId));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.AreaId)))
                {
                    adminInfo.AreaId = request.GetPostInt(nameof(AdministratorInfo.AreaId));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.DisplayName)))
                {
                    adminInfo.DisplayName = request.GetPostString(nameof(AdministratorInfo.DisplayName));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.Mobile)))
                {
                    adminInfo.Mobile = request.GetPostString(nameof(AdministratorInfo.Mobile));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.Email)))
                {
                    adminInfo.Email = request.GetPostString(nameof(AdministratorInfo.Email));
                }
                if (request.IsPostExists(nameof(AdministratorInfo.AvatarUrl)))
                {
                    adminInfo.AvatarUrl = request.GetPostString(nameof(AdministratorInfo.AvatarUrl));
                }

                var updated = DataProvider.AdministratorDao.Update(adminInfo, out var errorMessage);
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
                var request = GetRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(id);
                if (adminInfo == null) return NotFound();

                DataProvider.AdministratorDao.Delete(adminInfo);

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
                var request = GetRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
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
                var request = GetRequest();

                if (!request.IsAdminLoggin) return Unauthorized();

                var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

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
                var request = GetRequest();

                var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var query = Q.Limit(top).Offset(skip);
                var administrators = DataProvider.AdministratorDao.GetAll(query);
                var count = DataProvider.AdministratorDao.GetCount();

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
                var request = GetRequest();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var isAutoLogin = request.GetPostBool("isAutoLogin");

                AdministratorInfo adminInfo;

                if (!DataProvider.AdministratorDao.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                    if (adminInfo != null)
                    {
                        DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo); // 记录最后登录时间、失败次数+1
                    }
                    return BadRequest(errorMessage);
                }

                adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(adminInfo); // 记录最后登录时间、失败次数清零
                var accessToken = request.AdminLogin(adminInfo.UserName, isAutoLogin);
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
                var request = GetRequest();

                var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

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
                var request = GetRequest();
                var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");

                if (!DataProvider.AdministratorDao.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var adminInfo = AdminManager.GetAdminInfoByUserName(userName);

                if (!DataProvider.AdministratorDao.ChangePassword(adminInfo, newPassword, out errorMessage))
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
