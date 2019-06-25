using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    //https://jonhilton.net/2017/10/11/secure-your-asp.net-core-2.0-api-part-1---issuing-a-jwt/
    public partial class UserManager : IUserManager
    {
        private readonly HttpContext _context;
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IConfigRepository _configRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;

        public UserManager(
            IHttpContextAccessor contextAccessor,
            ISettingsManager settingsManager,
            IPluginManager pluginManager,
            IPathManager pathManager,
            IAccessTokenRepository accessTokenRepository,
            IConfigRepository configRepository,
            ISiteRepository siteRepository,
            IPermissionRepository permissionRepository,
            IUserRoleRepository userRoleRepository,
            IUserGroupRepository userGroupRepository,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository
            )
        {
            _context = contextAccessor.HttpContext;
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _permissionRepository = permissionRepository;
            _accessTokenRepository = accessTokenRepository;
            _userRoleRepository = userRoleRepository;
            _userGroupRepository = userGroupRepository;
            _userLogRepository = userLogRepository;
            _userRepository = userRepository;
        }

        //public string AdminLogin(string userName, bool isAutoLogin, IAccessTokenRepository accessTokenRepository)
        //{
        //    if (string.IsNullOrEmpty(userName)) return null;
        //    var adminInfo = _userRepository.GetAdminInfoByUserName(userName);
        //    if (adminInfo == null || adminInfo.Locked) return null;

        //    var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
        //    var token = new Token
        //    {
        //        UserId = adminInfo.Id,
        //        UserName = adminInfo.UserName,
        //        ExpiresAt = expiresAt
        //    };
        //    var accessToken = GetToken(token);

        //    AddAdminLog("管理员登录");

        //    if (isAutoLogin)
        //    {
        //        _context.Response.Cookies.Delete(Constants.CookieAdminToken);
        //        _context.Response.Cookies.Append(Constants.CookieAdminToken, accessToken, new CookieOptions
        //        {
        //            Expires = expiresAt
        //        });
        //    }
        //    else
        //    {
        //        _context.Response.Cookies.Delete(Constants.CookieAdminToken);
        //        _context.Response.Cookies.Append(Constants.CookieAdminToken, accessToken);
        //    }

        //    return accessToken;
        //}

        public async Task<string> SignInAsync(UserInfo userInfo)
        {
            if (userInfo == null || userInfo.IsLockedOut) return null;

            // var identity = new ClaimsIdentity(this.GetUserClaims(userInfo), CookieAuthenticationDefaults.AuthenticationScheme);
            // var principal = new ClaimsPrincipal(identity);

            // await _context.SignInAsync(
            //     CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            //     {
            //         IsPersistent = isPersistent
            //     }
            // );

            //AddAdminLog("管理员登录");

            var claims = await this.GetUserClaimsAsync(userInfo);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settingsManager.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }

        public async Task SignOutAsync()
        {
            await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public string GetIpAddress()
        {
            return _context.Connection.RemoteIpAddress.ToString();
        }

        public string GetUserName()
        {
            return _context.User.FindFirstValue(AuthTypes.ClaimTypes.UserName);
        }

        public int GetUserId()
        {
            return TranslateUtils.ToInt(_context.User.FindFirstValue(AuthTypes.ClaimTypes.UserId));
        }

        public async Task<UserInfo> GetUserAsync()
        {
            var userName = GetUserName();
            if (userName == null) return null;

            return await _userRepository.GetUserInfoByUserNameAsync(userName);
        }
    }
}
