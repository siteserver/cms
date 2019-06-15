using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;
using SS.CMS.Core.Security;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class IdentityManager : IIdentityManager
    {
        private readonly HttpContext _context;
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IAdministratorsInRolesRepository _administratorsInRolesRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;

        public IdentityManager(IHttpContextAccessor contextAccessor, ISettingsManager settingsManager, IPluginManager pluginManager, IPathManager pathManager, ISiteRepository siteRepository, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IAdministratorsInRolesRepository administratorsInRolesRepository, IUserGroupRepository userGroupRepository, IUserLogRepository userLogRepository, IUserRepository userRepository, IPermissionsInRolesRepository permissionsInRolesRepository, ISitePermissionsRepository sitePermissionsRepository)
        {
            _context = contextAccessor.HttpContext;
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _administratorsInRolesRepository = administratorsInRolesRepository;
            _userGroupRepository = userGroupRepository;
            _userLogRepository = userLogRepository;
            _userRepository = userRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
        }

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
                            var adminInfo = _administratorRepository.GetAdminInfoByUserName(tokenInfo.AdminName);
                            if (adminInfo != null && !adminInfo.Locked)
                            {
                                AdminInfo = adminInfo;
                                IsAdminLoggin = true;
                            }
                        }

                        IsApiAuthenticated = true;
                    }
                }

                var userToken = UserToken;
                if (!string.IsNullOrEmpty(userToken))
                {
                    var tokenImpl = ParseToken(userToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var userInfo = _userRepository.GetUserInfoByUserId(tokenImpl.UserId);
                        if (userInfo != null && !userInfo.Locked && userInfo.Checked && userInfo.UserName == tokenImpl.UserName)
                        {
                            UserInfo = userInfo;
                            IsUserLoggin = true;
                        }
                    }
                }

                var adminToken = AdminToken;
                if (!string.IsNullOrEmpty(adminToken))
                {
                    var tokenImpl = ParseToken(adminToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var adminInfo = _administratorRepository.GetAdminInfoByUserId(tokenImpl.UserId);
                        if (adminInfo != null && !adminInfo.Locked && adminInfo.UserName == tokenImpl.UserName)
                        {
                            AdminInfo = adminInfo;
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

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _settingsManager.SecretKey) : accessToken;
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

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _settingsManager.SecretKey) : accessToken;
            }
        }

        public bool IsAdminLoggin { get; private set; }

        private IPermissions _adminPermissions;

        public IPermissions AdminPermissions
        {
            get
            {
                if (_adminPermissions != null) return _adminPermissions;

                _adminPermissions = new Permissions(AdminInfo, _settingsManager, _pathManager, _pluginManager, _siteRepository, _administratorsInRolesRepository, _permissionsInRolesRepository, _sitePermissionsRepository);

                return _adminPermissions;
            }
        }

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
                    var groupInfo = _userGroupRepository.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        return groupInfo.AdminName;
                    }
                }

                return string.Empty;
            }
        }

        public AdministratorInfo AdminInfo { get; private set; }

        public string AdminLogin(string userName, bool isAutoLogin, IAccessTokenRepository accessTokenRepository)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = _administratorRepository.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var token = new Token
            {
                UserId = adminInfo.Id,
                UserName = adminInfo.UserName,
                ExpiresAt = expiresAt
            };
            var accessToken = GetToken(token);

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

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken, _settingsManager.SecretKey) : accessToken;
            }
        }

        private IPermissions _userPermissions;

        public IPermissions UserPermissions
        {
            get
            {
                if (_userPermissions != null) return _userPermissions;

                if (UserInfo != null)
                {
                    var groupInfo = _userGroupRepository.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        AdminInfo = _administratorRepository.GetAdminInfoByUserName(groupInfo.AdminName);
                    }
                }

                _userPermissions = new Permissions(AdminInfo, _settingsManager, _pathManager, _pluginManager, _siteRepository, _administratorsInRolesRepository, _permissionsInRolesRepository, _sitePermissionsRepository);

                return _userPermissions;
            }
        }

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public UserInfo UserInfo { get; private set; }

        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = _userRepository.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var token = new Token
            {
                UserId = userInfo.Id,
                UserName = userInfo.UserName,
                ExpiresAt = expiresAt
            };
            var accessToken = GetToken(token);

            _userRepository.UpdateLastActivityDateAndCountOfLogin(userInfo);
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
            _userLogRepository.AddUserLog(IpAddress, UserName, action, summary);
        }

        // log end

        // token start

        public string GetToken(Token token)
        {
            if (token == null || token.UserId <= 0 || string.IsNullOrEmpty(token.UserName)) return null;

            return JsonWebToken.Encode(token, _settingsManager.SecretKey, JwtHashAlgorithm.HS256);
        }

        public Token ParseToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return new Token();

            try
            {
                var tokenObj = JsonWebToken.DecodeToObject<Token>(accessToken, _settingsManager.SecretKey);

                if (tokenObj?.ExpiresAt.AddDays(Constants.AccessTokenExpireDays) > DateTime.Now)
                {
                    return tokenObj;
                }
            }
            catch
            {
                // ignored
            }

            return new Token();
        }

        // token end
    }
}
