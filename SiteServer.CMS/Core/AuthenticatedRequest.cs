using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.Core
{
    public class AuthenticatedRequest
    {
        private AuthenticatedRequest()
        {
            
        }

        public static async Task<AuthenticatedRequest> GetAuthAsync()
        {
            var authRequest = new AuthenticatedRequest();
            try
            {
                authRequest.HttpRequest = HttpContext.Current.Request;

                var apiToken = authRequest.ApiToken;
                if (!string.IsNullOrEmpty(apiToken))
                {
                    var tokenInfo = await DataProvider.AccessTokenRepository.GetByTokenAsync(apiToken);
                    if (tokenInfo != null)
                    {
                        if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                        {
                            var adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                            if (adminInfo != null && !adminInfo.Locked)
                            {
                                authRequest.Administrator = adminInfo;
                                authRequest.IsAdminLoggin = true;
                            }
                        }

                        authRequest.IsApiAuthenticated = true;
                    }
                }

                var userToken = authRequest.UserToken;
                if (!string.IsNullOrEmpty(userToken))
                {
                    var tokenImpl = UserApi.Instance.ParseAccessToken(userToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var user = await DataProvider.UserRepository.GetByUserIdAsync(tokenImpl.UserId);
                        if (user != null && !user.Locked && user.Checked && user.UserName == tokenImpl.UserName)
                        {
                            authRequest.User = user;
                            authRequest.IsUserLoggin = true;
                        }
                    }
                }

                var adminToken = authRequest.AdminToken;
                if (!string.IsNullOrEmpty(adminToken))
                {
                    var tokenImpl = AdminApi.Instance.ParseAccessToken(adminToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(tokenImpl.UserId);
                        if (adminInfo != null && !adminInfo.Locked && adminInfo.UserName == tokenImpl.UserName)
                        {
                            authRequest.Administrator = adminInfo;
                            authRequest.IsAdminLoggin = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
            }

            return authRequest;
        }

        public bool IsApiAuthenticated { get; private set; }

        public bool IsUserLoggin { get; private set; }

        public bool IsAdminLoggin { get; private set; }

        public string ApiToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(Constants.AuthKeyApiHeader)))
                {
                    accessTokenStr = HttpRequest.Headers.Get(Constants.AuthKeyApiHeader);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.QueryString[Constants.AuthKeyApiQuery]))
                {
                    accessTokenStr = HttpRequest.QueryString[Constants.AuthKeyApiQuery];
                }
                else if (!string.IsNullOrEmpty(CookieUtils.GetCookie(Constants.AuthKeyApiCookie)))
                {
                    accessTokenStr = CookieUtils.GetCookie(Constants.AuthKeyApiCookie);
                }

                if (StringUtils.EndsWith(accessTokenStr, WebConfigUtils.EncryptStingIndicator))
                {
                    accessTokenStr = WebConfigUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        private string UserToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(CookieUtils.GetCookie(Constants.AuthKeyUserCookie)))
                {
                    accessTokenStr = CookieUtils.GetCookie(Constants.AuthKeyUserCookie);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(Constants.AuthKeyUserHeader)))
                {
                    accessTokenStr = HttpRequest.Headers.Get(Constants.AuthKeyUserHeader);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.QueryString[Constants.AuthKeyUserQuery]))
                {
                    accessTokenStr = HttpRequest.QueryString[Constants.AuthKeyUserQuery];
                }

                if (StringUtils.EndsWith(accessTokenStr, WebConfigUtils.EncryptStingIndicator))
                {
                    accessTokenStr = WebConfigUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        public string AdminToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(CookieUtils.GetCookie(Constants.AuthKeyAdminCookie)))
                {
                    accessTokenStr = CookieUtils.GetCookie(Constants.AuthKeyAdminCookie);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(Constants.AuthKeyAdminHeader)))
                {
                    accessTokenStr = HttpRequest.Headers.Get(Constants.AuthKeyAdminHeader);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.QueryString[Constants.AuthKeyAdminQuery]))
                {
                    accessTokenStr = HttpRequest.QueryString[Constants.AuthKeyAdminQuery];
                }

                if (StringUtils.EndsWith(accessTokenStr, WebConfigUtils.EncryptStingIndicator))
                {
                    accessTokenStr = WebConfigUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        private Dictionary<string, object> _postData;

        public Dictionary<string, object> PostData
        {
            get
            {
                if (_postData != null) return _postData;

                var bodyStream = new StreamReader(HttpRequest.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                var json = bodyStream.ReadToEnd();

                _postData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(json)) return _postData;

                var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
                foreach (var key in dict.Keys)
                {
                    _postData[key] = dict[key];
                }

                return _postData;
            }
        }

        public HttpRequest HttpRequest { get; private set; }

        public NameValueCollection QueryString => HttpRequest.QueryString;

        public int SiteId => GetQueryInt("siteId");

        public int ChannelId => GetQueryInt("channelId");

        public int ContentId => GetQueryInt("contentId");

        public bool IsQueryExists(string name)
        {
            return HttpRequest.QueryString[name] != null;
        }

        public string GetQueryString(string name)
        {
            return !string.IsNullOrEmpty(HttpRequest.QueryString[name])
                ? AttackUtils.FilterSql(HttpRequest.QueryString[name])
                : null;
        }

        public int GetQueryInt(string name, int defaultValue = 0)
        {
            return !string.IsNullOrEmpty(HttpRequest.QueryString[name])
                ? TranslateUtils.ToIntWithNegative(HttpRequest.QueryString[name])
                : defaultValue;
        }

        public decimal GetQueryDecimal(string name, decimal defaultValue = 0)
        {
            return !string.IsNullOrEmpty(HttpRequest.QueryString[name])
                ? TranslateUtils.ToDecimalWithNagetive(HttpRequest.QueryString[name])
                : defaultValue;
        }

        public bool GetQueryBool(string name, bool defaultValue = false)
        {
            var str = HttpRequest.QueryString[name];
            return !string.IsNullOrEmpty(str) ? TranslateUtils.ToBool(str) : defaultValue;
        }

        public bool IsPostExists(string name)
        {
            return PostData.ContainsKey(name);
        }

        public T GetPostObject<T>(string name = "")
        {
            string json;
            if (string.IsNullOrEmpty(name))
            {
                var bodyStream = new StreamReader(HttpRequest.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                json = bodyStream.ReadToEnd();
            }
            else
            {
                json = GetPostString(name);
            }

            return TranslateUtils.JsonDeserialize<T>(json);
        }

        private object GetPostObject(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return PostData.TryGetValue(name, out var value) ? value : null;
        }

        public string GetPostString(string name)
        {
            var value = GetPostObject(name);
            if (value == null) return null;
            if (value is string) return (string)value;
            return value.ToString();
        }

        public int GetPostInt(string name, int defaultValue = 0)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is int) return (int)value;
            return TranslateUtils.ToIntWithNegative(value.ToString(), defaultValue);
        }

        public decimal GetPostDecimal(string name, decimal defaultValue = 0)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is decimal) return (decimal)value;
            return TranslateUtils.ToDecimalWithNagetive(value.ToString(), defaultValue);
        }

        public bool GetPostBool(string name, bool defaultValue = false)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is bool) return (bool)value;
            return TranslateUtils.ToBool(value.ToString(), defaultValue);
        }

        public DateTime GetPostDateTime(string name, DateTime defaultValue)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return TranslateUtils.ToDateTime(value.ToString(), defaultValue);
        }

        #region Log

        public async Task AddSiteLogAsync(int siteId, string action)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, string.Empty);
        }

        public async Task AddSiteLogAsync(int siteId, string action, string summary)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, string action, string summary)
        {
            await LogUtils.AddSiteLogAsync(siteId, channelId, 0, Administrator, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await LogUtils.AddSiteLogAsync(siteId, channelId, contentId, Administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await LogUtils.AddAdminLogAsync(Administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await LogUtils.AddAdminLogAsync(Administrator, action);
        }

        #endregion

        #region Cookie

        public void SetCookie(string name, string value)
        {
            CookieUtils.SetCookie(name, value);
        }

        public void SetCookie(string name, string value, TimeSpan expiresAt)
        {
            CookieUtils.SetCookie(name, value, expiresAt);
        }

        public string GetCookie(string name)
        {
            return CookieUtils.GetCookie(name);
        }

        public bool IsCookieExists(string name)
        {
            return CookieUtils.IsExists(name);
        }

        #endregion

        private PermissionsImpl _userPermissionsImpl;

        public PermissionsImpl UserPermissionsImpl
        {
            get
            {
                if (_userPermissionsImpl != null) return _userPermissionsImpl;

                if (User != null)
                {
                    var groupInfo = DataProvider.UserGroupRepository.GetUserGroupAsync(User.GroupId).GetAwaiter().GetResult();
                    if (groupInfo != null)
                    {
                        Administrator = DataProvider.AdministratorRepository.GetByUserNameAsync(groupInfo.AdminName).GetAwaiter().GetResult();
                    }
                }

                _userPermissionsImpl = new PermissionsImpl(Administrator);

                return _userPermissionsImpl;
            }
        }

        public PermissionsImpl UserPermissions => UserPermissionsImpl;

        private PermissionsImpl _adminPermissionsImpl;

        public PermissionsImpl AdminPermissionsImpl
        {
            get
            {
                if (_adminPermissionsImpl != null) return _adminPermissionsImpl;

                _adminPermissionsImpl = new PermissionsImpl(Administrator);

                return _adminPermissionsImpl;
            }
        }

        public PermissionsImpl AdminPermissions => AdminPermissionsImpl;

        #region Administrator

        public int AdminId => Administrator?.Id ?? 0;

        public string AdminName
        {
            get
            {
                if (Administrator != null)
                {
                    return Administrator.UserName;
                }

                if (User != null)
                {
                    var groupInfo = DataProvider.UserGroupRepository.GetUserGroupAsync(User.GroupId).GetAwaiter().GetResult();
                    if (groupInfo != null)
                    {
                        return groupInfo.AdminName;
                    }
                }

                return string.Empty;
            }
        }

        public Administrator Administrator { get; private set; }

        public async Task<string> AdminLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            Administrator = adminInfo;
            IsAdminLoggin = true;

            var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var accessToken = AdminApi.Instance.GetAccessToken(adminInfo.Id, adminInfo.UserName, expiresAt);

            await LogUtils.AddAdminLogAsync(adminInfo, "管理员登录");

            if (isAutoLogin)
            {
                CookieUtils.SetCookie(Constants.AuthKeyAdminCookie, accessToken, expiresAt);
            }
            else
            {
                CookieUtils.SetCookie(Constants.AuthKeyAdminCookie, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            CookieUtils.Erase(Constants.AuthKeyAdminCookie);
        }

        #endregion

        #region User

        public int UserId => User?.Id ?? 0;

        public string UserName => User?.UserName ?? string.Empty;

        public User User { get; private set; }

        public async Task<string> UserLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var user = await DataProvider.UserRepository.GetByUserNameAsync(userName);
            if (user == null || user.Locked || !user.Checked) return null;

            User = user;

            var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var accessToken = UserApi.Instance.GetAccessToken(UserId, UserName, expiresAt);

            await DataProvider.UserRepository.UpdateLastActivityDateAndCountOfLoginAsync(User);
            await LogUtils.AddUserLoginLogAsync(userName);

            if (isAutoLogin)
            {
                CookieUtils.SetCookie(Constants.AuthKeyUserCookie, accessToken, expiresAt);
            }
            else
            {
                CookieUtils.SetCookie(Constants.AuthKeyUserCookie, accessToken);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            User = null;
            CookieUtils.Erase(Constants.AuthKeyUserCookie);
        }

        #endregion

        public async Task<string> AdminRedirectCheckAsync(bool checkInstall = false, bool checkDatabaseVersion = false, bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            var config = await DataProvider.ConfigRepository.GetAsync();

            if (checkInstall && string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                redirect = true;
                redirectUrl = PageUtils.GetAdminUrl("Installer/");
            }
            else if (checkDatabaseVersion && config.Initialized &&
                     config.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = PageUtils.GetAdminUrl("syncDatabase.cshtml");
            }
            else if (checkLogin && (!IsAdminLoggin || Administrator == null || Administrator.Locked))
            {
                redirect = true;
                redirectUrl = PageUtils.GetAdminUrl("login.cshtml");
            }

            return redirect ? redirectUrl : null;
        }

        public static string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = DateUtils.GetExpiresAt(expiresAt)
            };

            return JsonWebToken.Encode(userToken, WebConfigUtils.SecretKey, JwtHashAlgorithm.HS256);
        }

        public static string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = expiresAt
            };

            return JsonWebToken.Encode(userToken, WebConfigUtils.SecretKey, JwtHashAlgorithm.HS256);
        }

        public static AccessTokenImpl ParseAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return new AccessTokenImpl();

            try
            {
                var tokenObj = JsonWebToken.DecodeToObject<AccessTokenImpl>(accessToken, WebConfigUtils.SecretKey);

                if (tokenObj?.ExpiresAt.AddDays(Constants.AccessTokenExpireDays) > DateTime.Now)
                {
                    return tokenObj;
                }
            }
            catch
            {
                // ignored
            }

            return new AccessTokenImpl();
        }

        public void CheckAdminLoggin(HttpRequestMessage request)
        {
            if (!IsAdminLoggin)
            {
                request.Unauthorized();
            }
        }

        public async Task CheckSettingsPermissions(HttpRequestMessage request, params string[] permissions)
        {
            if (!IsAdminLoggin ||
                !await AdminPermissionsImpl.HasSystemPermissionsAsync(permissions))
            {
                request.Unauthorized();
            }
        }

        public async Task CheckSitePermissionsAsync(HttpRequestMessage request, int siteId, params string[] permissions)
        {
            if (!IsAdminLoggin ||
                !await AdminPermissionsImpl.HasSitePermissionsAsync(siteId, permissions))
            {
                request.Unauthorized();
            }
        }
    }
}