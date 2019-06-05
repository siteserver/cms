using System;
using SS.CMS.Core.Api.V1;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services.V2
{
    public class AdministratorsService : ServiceBase
    {
        private const string Route = "administrators";
        private const string RouteMe = "administrators/me";
        private const string RouteActionsLogin = "administrators/actions/login";
        private const string RouteActionsLogout = "administrators/actions/logout";
        private const string RouteActionsResetPassword = "administrators/actions/resetPassword";
        private const string RouteAdministrator = "administrators/{id:int}";

        public ResponseResult<object> Create(IRequest request, IResponse response)
        {
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

        public ResponseResult<object> Update(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

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

        public ResponseResult<object> Delete(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");
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

        public ResponseResult<object> Get(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");
            var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var adminInfo = AdminManager.GetAdminInfoByUserId(id);

            if (adminInfo == null) return NotFound();

            return Ok(new
            {
                Value = adminInfo
            });
        }

        public ResponseResult<object> GetSelf(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin) return Unauthorized();

            var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

            return Ok(new
            {
                Value = adminInfo
            });
        }

        public ResponseResult<object> List(IRequest request, IResponse response)
        {
            var isApiAuthorized = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var top = request.GetQueryInt("top", 20);
            var skip = request.GetQueryInt("skip");

            var query = Q.Limit(top).Offset(skip);
            var administrators = DataProvider.AdministratorDao.GetAll(query);
            var count = DataProvider.AdministratorDao.GetCount();

            return Ok(new PageResponse(administrators, top, skip, request.RawUrl) { Count = count });
        }

        public ResponseResult<object> Login(IRequest request, IResponse response)
        {
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
            var accessToken = response.AdminLogin(adminInfo.UserName, isAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            return Ok(new
            {
                Value = adminInfo,
                AccessToken = accessToken,
                ExpiresAt = expiresAt
            });
        }

        public ResponseResult<object> Logout(IRequest request, IResponse response)
        {
            var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

            return Ok(new
            {
                Value = adminInfo
            });
        }

        public ResponseResult<object> ResetPassword(IRequest request, IResponse response)
        {
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
    }
}
