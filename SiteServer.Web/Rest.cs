using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Auth;

namespace SiteServer.API
{
    public class Rest: IRequest
    {
        private const string AuthKeyUserHeader = "X-SS-USER-TOKEN";
        private const string AuthKeyUserCookie = "SS-USER-TOKEN";
        private const string AuthKeyUserQuery = "userToken";
        public const string AuthKeyAdminHeader = "X-SS-ADMIN-TOKEN";
        private const string AuthKeyAdminCookie = "SS-ADMIN-TOKEN";
        private const string AuthKeyAdminQuery = "adminToken";
        private const string AuthKeyApiHeader = "X-SS-API-KEY";
        private const string AuthKeyApiCookie = "SS-API-KEY";
        private const string AuthKeyApiQuery = "apiKey";

        public const int AccessTokenExpireDays = 7;

        private readonly HttpRequestMessage _request;

        public Rest(HttpRequestMessage request)
        {
            _request = request;

            try
            {
                var apiToken = ApiToken;
                if (!string.IsNullOrEmpty(apiToken))
                {
                    var tokenInfo = AccessTokenManager.GetAccessTokenInfo(apiToken);
                    if (tokenInfo != null)
                    {
                        if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                        {
                            var adminInfo = AdminManager.GetAdminInfoByUserName(tokenInfo.AdminName);
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
                    var tokenImpl = ParseAccessToken(userToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var userInfo = UserManager.GetUserInfoByUserId(tokenImpl.UserId);
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
                    var tokenImpl = ParseAccessToken(adminToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserId(tokenImpl.UserId);
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

        #region Request

        /// <summary>
        /// Returns an individual HTTP Header value
        /// </summary>
        public string GetHeader(string key)
        {
            return !_request.Headers.TryGetValues(key, out var keys) ? null : keys.First();
        }

        /// <summary>
        /// Retrieves an individual cookie from the cookies collection
        /// </summary>
        public string GetCookie(string cookieName)
        {
            var cookie = _request.Headers.GetCookies(cookieName).FirstOrDefault();
            var value = cookie?[cookieName].Value;
            return TranslateUtils.DecryptStringBySecretKey(value);
        }

        private Dictionary<string, string> _queryDict;
        public Dictionary<string, string> QueryDict
        {
            get
            {
                if (_queryDict != null) return _queryDict;

                _queryDict = _request.GetQueryNameValuePairs()
                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

                return _queryDict;
            }
        }

        public bool IsQueryExists(string name)
        {
            return QueryDict.ContainsKey(name);
        }

        public string GetQueryString(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return QueryDict.TryGetValue(name, out var value) ? value : null;
        }

        public int GetQueryInt(string name, int defaultValue = 0)
        {
            return TranslateUtils.ToIntWithNegative(GetQueryString(name), defaultValue);
        }

        public decimal GetQueryDecimal(string name, decimal defaultValue = 0)
        {
            return TranslateUtils.ToDecimalWithNegative(GetQueryString(name), defaultValue);
        }

        public bool GetQueryBool(string name, bool defaultValue = false)
        {
            return TranslateUtils.ToBool(GetQueryString(name), false);
        }

        private Dictionary<string, object> _postDict;
        public Dictionary<string, object> PostDict
        {
            get
            {
                if (_postDict != null) return _postDict;

                _postDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                var json = _request.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(json)) return _postDict;

                var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
                foreach (var key in dict.Keys)
                {
                    _postDict[key] = dict[key];
                }

                return _postDict;
            }
        }

        public T GetPostObject<T>()
        {
            var json = _request.Content.ReadAsStringAsync().Result;
            return TranslateUtils.JsonDeserialize<T>(json);
        }

        public T GetPostObject<T>(string name)
        {
            var json = GetPostString(name);
            return TranslateUtils.JsonDeserialize<T>(json);
        }

        public bool IsPostExists(string name)
        {
            return PostDict.ContainsKey(name);
        }

        private object GetPostObject(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return PostDict.TryGetValue(name, out var value) ? value : null;
        }

        public string GetPostString(string name)
        {
            var value = GetPostObject(name);
            switch (value)
            {
                case null:
                    return null;
                case string s:
                    return s;
                default:
                    return value.ToString();
            }
        }

        public int GetPostInt(string name, int defaultValue = 0)
        {
            var value = GetPostObject(name);
            switch (value)
            {
                case null:
                    return 0;
                case int _:
                    return (int)value;
                default:
                    return TranslateUtils.ToIntWithNegative(value.ToString(), defaultValue);
            }
        }

        public decimal GetPostDecimal(string name, decimal defaultValue = 0)
        {
            var value = GetPostObject(name);
            switch (value)
            {
                case null:
                    return 0;
                case decimal _:
                    return (decimal)value;
                default:
                    return TranslateUtils.ToDecimalWithNegative(value.ToString(), defaultValue);
            }
        }

        public bool GetPostBool(string name, bool defaultValue = false)
        {
            var value = GetPostObject(name);
            switch (value)
            {
                case null:
                    return false;
                case bool _:
                    return (bool)value;
                default:
                    return TranslateUtils.ToBool(value.ToString(), defaultValue);
            }
        }

        public DateTime GetPostDateTime(string name, DateTime defaultValue)
        {
            var value = GetPostObject(name);
            switch (value)
            {
                case null:
                    return DateTime.Now;
                case DateTime _:
                    return (DateTime)value;
                default:
                    return TranslateUtils.ToDateTime(value.ToString(), defaultValue);
            }
        }

        #endregion

        public bool IsApiAuthenticated { get; }

        public bool IsUserLoggin { get; }

        public bool IsAdminLoggin { get; private set; }

        public string ApiToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(GetHeader(AuthKeyApiHeader)))
                {
                    accessTokenStr = GetHeader(AuthKeyApiHeader);
                }
                else if (IsQueryExists(AuthKeyApiQuery))
                {
                    accessTokenStr = GetQueryString(AuthKeyApiQuery);
                }
                else if (!string.IsNullOrEmpty(GetCookie(AuthKeyApiCookie)))
                {
                    accessTokenStr = GetCookie(AuthKeyApiCookie);
                }

                if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
                {
                    accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        private string UserToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(GetCookie(AuthKeyUserCookie)))
                {
                    accessTokenStr = GetCookie(AuthKeyUserCookie);
                }
                else if (!string.IsNullOrEmpty(GetHeader(AuthKeyUserHeader)))
                {
                    accessTokenStr = GetHeader(AuthKeyUserHeader);
                }
                else if (IsQueryExists(AuthKeyUserQuery))
                {
                    accessTokenStr = GetQueryString(AuthKeyUserQuery);
                }

                if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
                {
                    accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        public string AdminToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(GetCookie(AuthKeyAdminCookie)))
                {
                    accessTokenStr = GetCookie(AuthKeyAdminCookie);
                }
                else if (!string.IsNullOrEmpty(GetHeader(AuthKeyAdminHeader)))
                {
                    accessTokenStr = GetHeader(AuthKeyAdminHeader);
                }
                else if (IsQueryExists(AuthKeyAdminQuery))
                {
                    accessTokenStr = GetQueryString(AuthKeyAdminQuery);
                }

                if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
                {
                    accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        public int SiteId => GetQueryInt("siteId");

        public int ChannelId => GetQueryInt("channelId");

        public int ContentId => GetQueryInt("contentId");

        #region Log

        public void AddSiteLog(int siteId, string action)
        {
            AddSiteLog(siteId, 0, 0, action, string.Empty);
        }

        public void AddSiteLog(int siteId, string action, string summary)
        {
            AddSiteLog(siteId, 0, 0, action, summary);
        }

        public void AddSiteLog(int siteId, int channelId, string action, string summary)
        {
            LogUtils.AddSiteLog(siteId, channelId, 0, AdminName, action, summary);
        }

        public void AddSiteLog(int siteId, int channelId, int contentId, string action, string summary)
        {
            LogUtils.AddSiteLog(siteId, channelId, contentId, AdminName, action, summary);
        }

        public void AddAdminLog(string action, string summary)
        {
            LogUtils.AddAdminLog(AdminName, action, summary);
        }

        public void AddAdminLog(string action)
        {
            LogUtils.AddAdminLog(AdminName, action);
        }

        #endregion

        #region Cookie

        public void SetCookie(string name, string value)
        {
            CookieUtils.SetCookie(name, value);
        }

        public void SetCookie(string name, string value, TimeSpan expires)
        {
            CookieUtils.SetCookie(name, value, expires);
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

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        AdminInfo = AdminManager.GetAdminInfoByUserName(groupInfo.AdminName);
                    }
                }

                _userPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _userPermissionsImpl;
            }
        }

        public IPermissions UserPermissions => UserPermissionsImpl;

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

        #region Administrator

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

        public AdministratorInfo AdminInfo { get; private set; }

        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            AdminInfo = adminInfo;
            IsAdminLoggin = true;

            var expiresAt = TimeSpan.FromDays(AccessTokenExpireDays);
            var accessToken = GetAccessToken(adminInfo.Id, adminInfo.UserName, expiresAt);

            LogUtils.AddAdminLog(adminInfo.UserName, "管理员登录");

            if (isAutoLogin)
            {
                CookieUtils.SetCookie(AuthKeyAdminCookie, accessToken, expiresAt);
            }
            else
            {
                CookieUtils.SetCookie(AuthKeyAdminCookie, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            if (IsAdminLoggin)
            {
                PermissionsImpl.ClearAllCache();
                AdminManager.RemoveCache(AdminInfo);
            }
            
            CookieUtils.Erase(AuthKeyAdminCookie);
        }

        #endregion

        #region User

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public UserInfo UserInfo { get; private set; }

        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

            UserInfo = userInfo;

            var expiresAt = TimeSpan.FromDays(AccessTokenExpireDays);
            var accessToken = GetAccessToken(UserId, UserName, expiresAt);

            DataProvider.User.UpdateLastActivityDateAndCountOfLogin(UserInfo);
            LogUtils.AddUserLoginLog(userName);

            if (isAutoLogin)
            {
                CookieUtils.SetCookie(AuthKeyUserCookie, accessToken, expiresAt);
            }
            else
            {
                CookieUtils.SetCookie(AuthKeyUserCookie, accessToken);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            UserInfo = null;
            CookieUtils.Erase(AuthKeyUserCookie);
        }

        #endregion

        #region Utils

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

        private static AccessTokenImpl ParseAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return new AccessTokenImpl();

            try
            {
                var tokenObj = JsonWebToken.DecodeToObject<AccessTokenImpl>(accessToken, WebConfigUtils.SecretKey);

                if (tokenObj?.ExpiresAt.AddDays(AccessTokenExpireDays) > DateTime.Now)
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

        public object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(WebConfigUtils.ConnectionString))
            {
                redirect = true;
                redirectUrl = PageUtils.GetAdminUrl("installer/default.aspx");
            }
            else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
                     ConfigManager.Instance.DatabaseVersion != SystemManager.Version)
            {
                redirect = true;
                redirectUrl = AdminPagesUtils.UpdateUrl;
            }
            else if (checkLogin && (!IsAdminLoggin || AdminInfo == null || AdminInfo.Locked))
            {
                redirect = true;
                redirectUrl = AdminPagesUtils.LoginUrl;
            }

            if (redirect)
            {
                return new
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            return null;
        }

        #endregion
    }
}