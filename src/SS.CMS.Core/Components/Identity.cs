using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Settings;
using SS.CMS.Utils;

namespace SS.CMS.Core.Components
{
    public class Identity : IIdentity
    {
        private readonly HttpContext _context;
        private readonly AppSettings _appSettings;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public Identity(HttpContext context, AppSettings appSettings, IAccessTokenRepository accessTokenRepository)
        {
            _context = context;
            _appSettings = appSettings;
            _accessTokenRepository = accessTokenRepository;
        }

        private AdministratorInfo _adminInfo;
        private UserInfo _userInfo;

        public async Task Sync()
        {
            try
            {
                var apiToken = ApiToken;
                if (!string.IsNullOrEmpty(apiToken))
                {
                    var tokenInfo = await _accessTokenRepository.GetAsync(apiToken);
                    if (tokenInfo != null)
                    {
                        if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                        {
                            var adminInfo = AdminManager.GetAdminInfoByUserName(tokenInfo.AdminName);
                            if (adminInfo != null && !adminInfo.Locked)
                            {
                                _adminInfo = adminInfo;
                                IsAdminLoggin = true;
                            }
                        }

                        IsApiAuthenticated = true;
                    }
                }

                var userToken = UserToken;
                if (!string.IsNullOrEmpty(userToken))
                {
                    var tokenImpl = _accessTokenRepository.ParseAccessToken(userToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var userInfo = UserManager.GetUserInfoByUserId(tokenImpl.UserId);
                        if (userInfo != null && !userInfo.Locked && userInfo.Checked && userInfo.UserName == tokenImpl.UserName)
                        {
                            _userInfo = userInfo;
                            IsUserLoggin = true;
                        }
                    }
                }

                var adminToken = AdminToken;
                if (!string.IsNullOrEmpty(adminToken))
                {
                    var tokenImpl = _accessTokenRepository.ParseAccessToken(adminToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserId(tokenImpl.UserId);
                        if (adminInfo != null && !adminInfo.Locked && adminInfo.UserName == tokenImpl.UserName)
                        {
                            _adminInfo = adminInfo;
                            IsAdminLoggin = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
            }
        }

        // api start

        public bool IsApiAuthenticated { get; set; }

        public string ApiToken
        {
            get
            {
                var accessToken = string.Empty;

                if (_context.Request.Query.TryGetValue(Constants.QueryApiKey, out var query))
                {
                    accessToken = query;
                }
                else if (_context.Request.Headers.TryGetValue(Constants.HeaderApiKey, out var header))
                {
                    accessToken = header;
                }
                else if (_context.Request.Cookies.TryGetValue(Constants.CookieApiKey, out var cookie))
                {
                    accessToken = cookie;
                }

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _appSettings.SecretKey) : accessToken;
            }
        }

        // api end

        // admin start

        public string AdminToken
        {
            get
            {
                var accessToken = string.Empty;

                if (_context.Request.Query.TryGetValue(Constants.QueryAdminToken, out var query))
                {
                    accessToken = query;
                }
                else if (_context.Request.Headers.TryGetValue(Constants.HeaderAdminToken, out var header))
                {
                    accessToken = header;
                }
                else if (_context.Request.Cookies.TryGetValue(Constants.CookieAdminToken, out var cookie))
                {
                    accessToken = cookie;
                }

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _appSettings.SecretKey) : accessToken;
            }
        }

        public bool IsAdminLoggin { get; private set; }

        private PermissionsImpl _adminPermissionsImpl;

        public PermissionsImpl AdminPermissionsImpl
        {
            get
            {
                if (_adminPermissionsImpl != null) return _adminPermissionsImpl;

                _adminPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _adminPermissionsImpl;
            }
        }

        public IPermissions AdminPermissions => AdminPermissionsImpl;

        public int AdminId => AdminInfo?.Id ?? 0;

        public string AdminName
        {
            get
            {
                if (AdminInfo != null)
                {
                    return AdminInfo.UserName;
                }

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        return groupInfo.AdminName;
                    }
                }

                return string.Empty;
            }
        }

        public IAdministratorInfo AdminInfo => _adminInfo;

        public string AdminLogin(string userName, bool isAutoLogin, IAccessTokenRepository accessTokenRepository)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = accessTokenRepository.GetAccessToken(adminInfo.Id, adminInfo.UserName, TimeSpan.FromDays(Constants.AccessTokenExpireDays));

            AddAdminLog("管理员登录");

            if (isAutoLogin)
            {
                _context.Response.Cookies.Delete(Constants.CookieAdminToken);
                _context.Response.Cookies.Append(Constants.CookieAdminToken, accessToken, new CookieOptions
                {
                    Expires = expiresAt
                });
            }
            else
            {
                _context.Response.Cookies.Delete(Constants.CookieAdminToken);
                _context.Response.Cookies.Append(Constants.CookieAdminToken, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            _context.Response.Cookies.Delete(Constants.CookieAdminToken);
        }

        // admin end

        // user start

        public bool IsUserLoggin { get; set; }


        public string UserToken
        {
            get
            {
                var accessToken = string.Empty;

                if (_context.Request.Query.TryGetValue(Constants.QueryUserToken, out var query))
                {
                    accessToken = query;
                }
                else if (_context.Request.Headers.TryGetValue(Constants.HeaderUserToken, out var header))
                {
                    accessToken = header;
                }
                else if (_context.Request.Cookies.TryGetValue(Constants.CookieUserToken, out var cookie))
                {
                    accessToken = cookie;
                }

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _appSettings.SecretKey) : accessToken;
            }
        }

        private PermissionsImpl _userPermissionsImpl;

        public PermissionsImpl UserPermissionsImpl
        {
            get
            {
                if (_userPermissionsImpl != null) return _userPermissionsImpl;

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        _adminInfo = AdminManager.GetAdminInfoByUserName(groupInfo.AdminName);
                    }
                }

                _userPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _userPermissionsImpl;
            }
        }

        public IPermissions UserPermissions => UserPermissionsImpl;

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public IUserInfo UserInfo => _userInfo;

        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = _accessTokenRepository.GetAccessToken(userInfo.Id, userInfo.UserName, TimeSpan.FromDays(Constants.AccessTokenExpireDays));

            DataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userInfo);
            AddUserLog("用户登录");

            if (isAutoLogin)
            {
                _context.Response.Cookies.Delete(Constants.CookieUserToken);
                _context.Response.Cookies.Append(Constants.CookieUserToken, accessToken, new CookieOptions
                {
                    Expires = expiresAt
                });
            }
            else
            {
                _context.Response.Cookies.Delete(Constants.CookieUserToken);
                _context.Response.Cookies.Append(Constants.CookieUserToken, accessToken);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            _context.Response.Cookies.Delete(Constants.CookieUserToken);
        }

        // user end

        // log start

        public string IpAddress => _context.Connection.RemoteIpAddress.ToString();

        public void AddSiteLog(int siteId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, 0, 0, IpAddress, AdminName, action, summary);
        }

        public void AddChannelLog(int siteId, int channelId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, channelId, 0, IpAddress, AdminName, action, summary);
        }

        public void AddContentLog(int siteId, int channelId, int contentId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, channelId, contentId, IpAddress, AdminName, action, summary);
        }

        public void AddAdminLog(string action, string summary = "")
        {
            LogUtils.AddAdminLog(IpAddress, AdminName, action, summary);
        }

        public void AddUserLog(string action, string summary = "")
        {
            LogUtils.AddUserLog(IpAddress, UserName, action, summary);
        }

        // log end
    }
}
