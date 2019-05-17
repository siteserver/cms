using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Auth;

namespace SiteServer.BackgroundPages.Core
{
    public class Request : IRequest
    {
        private readonly HttpRequest _request;
        private AdministratorInfo _adminInfo;
        private UserInfo _userInfo;

        public Request(HttpRequest request)
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
                    var tokenImpl = UserApi.Instance.ParseAccessToken(userToken);
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
                    var tokenImpl = AdminApi.Instance.ParseAccessToken(adminToken);
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

        public bool IsApiAuthenticated { get; }

        public bool IsUserLoggin { get; }

        public bool IsAdminLoggin { get; private set; }

        public string ApiToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (TryGetHeader(Constants.AuthKeyApiHeader, out var header))
                {
                    accessTokenStr = header;
                }
                else if (IsQueryExists(Constants.AuthKeyApiQuery))
                {
                    accessTokenStr = GetQueryString(Constants.AuthKeyApiQuery);
                }
                else if (TryGetCookie(Constants.AuthKeyApiCookie, out var cookie))
                {
                    accessTokenStr = cookie.Value;
                }

                if (!string.IsNullOrEmpty(accessTokenStr) && StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
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
                if (TryGetCookie(Constants.AuthKeyUserCookie, out var cookie))
                {
                    accessTokenStr = cookie.Value;
                }
                else if (TryGetHeader(Constants.AuthKeyUserHeader, out var header))
                {
                    accessTokenStr = header;
                }
                else if (IsQueryExists(Constants.AuthKeyUserQuery))
                {
                    accessTokenStr = GetQueryString(Constants.AuthKeyUserQuery);
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
                if (TryGetCookie(Constants.AuthKeyAdminCookie, out var cookie))
                {
                    accessTokenStr = cookie.Value;
                }
                else if (TryGetHeader(Constants.AuthKeyAdminHeader, out var header))
                {
                    accessTokenStr = header;
                }
                else if (IsQueryExists(Constants.AuthKeyAdminQuery))
                {
                    accessTokenStr = GetQueryString(Constants.AuthKeyAdminQuery);
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

        public string Path
        {
            get
            {
                return _request.Path;
            }
            set
            {

            }
        }

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

        public IAdministratorInfo AdminInfo => _adminInfo;

        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            _adminInfo = adminInfo;
            IsAdminLoggin = true;

            var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var accessToken = AdminApi.Instance.GetAccessToken(adminInfo.Id, adminInfo.UserName, expiresAt);

            AddAdminLog("管理员登录");

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

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public IUserInfo UserInfo => _userInfo;

        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

            _userInfo = userInfo;

            var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var accessToken = UserApi.Instance.GetAccessToken(UserId, UserName, expiresAt);

            DataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(_userInfo);
            AddUserLog("用户登录");

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
            _userInfo = null;
            CookieUtils.Erase(Constants.AuthKeyUserCookie);
        }

        #endregion

        public object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                redirect = true;
                redirectUrl = PageUtilsEx.GetAdminUrl("Installer/");
            }
            else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
                     ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = PageUtilsEx.GetAdminUrl("pageSyncDatabase.aspx");
            }
            else if (checkLogin && (!IsAdminLoggin || AdminInfo == null || AdminInfo.Locked))
            {
                redirect = true;
                redirectUrl = PageUtilsEx.GetAdminUrl("pageLogin.cshtml");
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

        public string RawUrl => _request.RawUrl;

        private string _ipAddress;
        public string IpAddress
        {
            get
            {
                if (_ipAddress == null)
                {
                    var result = string.Empty;

                    try
                    {
                        //取CDN用户真实IP的方法
                        //当用户使用代理时，取到的是代理IP
                        result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                        if (!string.IsNullOrEmpty(result))
                        {
                            //可能有代理
                            if (result.IndexOf(".", StringComparison.Ordinal) == -1)
                                result = null;
                            else
                            {
                                if (result.IndexOf(",", StringComparison.Ordinal) != -1)
                                {
                                    result = result.Replace("  ", "").Replace("'", "");
                                    var temparyip = result.Split(",;".ToCharArray());
                                    foreach (var t in temparyip)
                                    {
                                        if (PageUtils.IsIpAddress(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                                        {
                                            result = t;
                                        }
                                    }
                                    var str = result.Split(',');
                                    if (str.Length > 0)
                                        result = str[0].Trim();
                                }
                                else if (PageUtils.IsIpAddress(result))
                                    return result;
                            }
                        }

                        if (string.IsNullOrEmpty(result))
                            result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        if (string.IsNullOrEmpty(result))
                            result = HttpContext.Current.Request.UserHostAddress;
                        if (string.IsNullOrEmpty(result))
                            result = "localhost";

                        if (result == "::1" || result == "127.0.0.1")
                        {
                            result = "localhost";
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    _ipAddress = result;
                }

                return _ipAddress;
            }
        }

        public HttpFileCollection Files => _request.Files;

        public Stream Body => _request.InputStream;

        public bool IsHttps => _request.IsSecureConnection;

        public string Host => _request.UserHostName;


        public bool TryGetHeader(string name, out string value)
        {
            value = _request.Headers[name];
            return value != null;
        }

        public bool TryGetCookie(string name, out Cookie value)
        {
            value = null;
            var cookie = _request.Cookies.Get(name);
            if (cookie != null)
            {
                value = new Cookie
                {
                    Path = cookie.Path,
                    Domain = cookie.Domain,
                    Expires = cookie.Expires == DateTime.MinValue ? null : new DateTimeOffset?(cookie.Expires),
                    HttpOnly = cookie.HttpOnly,
                    Secure = cookie.Secure,
                    Value = cookie.Value
                };
            }
            return value != null;
        }

        private Dictionary<string, object> _postData;

        public Dictionary<string, object> PostData
        {
            get
            {
                if (_postData != null) return _postData;

                string json;
                using (var bodyStream = new StreamReader(_request.InputStream))
                {
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    json = bodyStream.ReadToEnd();
                }

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

        public List<string> QueryKeys => _request.QueryString.AllKeys.ToList();

        public List<string> PostKeys => PostData.Keys.ToList();

        public Dictionary<string, string> HeaderValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, string> QueryValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, object> PostValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsQueryExists(string name)
        {
            return _request.QueryString[name] != null;
        }

        public string GetQueryString(string name)
        {
            return !string.IsNullOrEmpty(_request.QueryString[name])
                ? AttackUtils.FilterSql(_request.QueryString[name])
                : null;
        }

        public int GetQueryInt(string name, int defaultValue = 0)
        {
            return !string.IsNullOrEmpty(_request.QueryString[name])
                ? TranslateUtils.ToIntWithNagetive(_request.QueryString[name])
                : defaultValue;
        }

        public decimal GetQueryDecimal(string name, decimal defaultValue = 0)
        {
            return !string.IsNullOrEmpty(_request.QueryString[name])
                ? TranslateUtils.ToDecimalWithNagetive(_request.QueryString[name])
                : defaultValue;
        }

        public bool GetQueryBool(string name, bool defaultValue = false)
        {
            var str = _request.QueryString[name];
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
                using (var bodyStream = new StreamReader(_request.InputStream))
                {
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    json = bodyStream.ReadToEnd();
                }
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
            return TranslateUtils.ToIntWithNagetive(value.ToString(), defaultValue);
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
    }
}