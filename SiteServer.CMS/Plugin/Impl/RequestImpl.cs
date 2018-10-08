using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Auth;

namespace SiteServer.CMS.Plugin.Impl
{
    public class RequestImpl : IRequest
    {
        private const string AuthKeyUserHeader = "X-SS-USER-TOKEN";
        private const string AuthKeyUserCookie = "SS-USER-TOKEN";
        private const string AuthKeyUserQuery = "userToken";
        private const string AuthKeyAdminHeader = "X-SS-ADMIN-TOKEN";
        private const string AuthKeyAdminCookie = "SS-ADMIN-TOKEN";
        private const string AuthKeyAdminQuery = "adminToken";
        private const string AuthKeyApiHeader = "X-SS-API-KEY";
        private const string AuthKeyApiCookie = "SS-API-KEY";
        private const string AuthKeyApiQuery = "apiKey";

        private const string CookieUser = "SS-USER";
        private const string CookieUserExpiresAt = "SS-USER-EXPIRES-AT";

        public const int AccessTokenExpireDays = 7;

        private readonly string _scope;

        public RequestImpl(string scope) : this(HttpContext.Current.Request)
        {
            _scope = scope;
        }

        public RequestImpl() : this(HttpContext.Current.Request)
        {
        }

        public RequestImpl(HttpRequest request)
        {
            HttpRequest = request;

            AuthUser();
            AuthAdministrator();
            AuthApi();
        }

        private JObject _postData;

        private JObject PostData
        {
            get
            {
                if (_postData != null) return _postData;
                var bodyStream = new StreamReader(HttpRequest.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                var raw = bodyStream.ReadToEnd();
                _postData = !string.IsNullOrEmpty(raw) ? JObject.Parse(raw) : new JObject();
                return _postData;
            }
        }

        public HttpRequest HttpRequest { get; }

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
                ? TranslateUtils.ToIntWithNagetive(HttpRequest.QueryString[name])
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
            var retval = !string.IsNullOrEmpty(str) ? TranslateUtils.ToBool(str) : defaultValue;
            return retval;
        }

        public bool IsPostExists(string name)
        {
            return PostData.TryGetValue(name, out _);
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

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var timeFormat = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            settings.Converters.Add(timeFormat);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public string GetPostString(string name)
        {
            return PostData[name]?.ToString();
        }

        public int GetPostInt(string name, int defaultValue = 0)
        {
            return TranslateUtils.ToIntWithNagetive(PostData[name]?.ToString(), defaultValue);
        }

        public decimal GetPostDecimal(string name, decimal defaultValue = 0)
        {
            return TranslateUtils.ToDecimalWithNagetive(PostData[name]?.ToString(), defaultValue);
        }

        public bool GetPostBool(string name, bool defaultValue = false)
        {
            return TranslateUtils.ToBool(PostData[name]?.ToString(), defaultValue);
        }

        public NameValueCollection GetPostCollection()
        {
            var formCollection = new NameValueCollection();
            foreach (var item in PostData)
            {
                formCollection[item.Key] = item.Value.ToString();
            }

            return formCollection;
        }

        #region Log

        public void AddSiteLog(int siteId, string action)
        {
            AddSiteLog(siteId, 0, 0, action, string.Empty);
        }

        public void AddSiteLog(int siteId, string action, string summary)
        {
            AddSiteLog(siteId, 0, 0, action, summary);
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

        public void SetCookie(string name, string value, DateTime expires)
        {
            CookieUtils.SetCookie(name, value, expires);
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

                var adminName = string.Empty;
                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        adminName = groupInfo.AdminName;
                    }
                }
                _userPermissionsImpl = new PermissionsImpl(adminName);

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

                var adminName = string.Empty;
                if (AdminInfo != null)
                {
                    adminName = AdminInfo.UserName;
                }
                _adminPermissionsImpl = new PermissionsImpl(adminName);

                return _adminPermissionsImpl;
            }
        }

        public IPermissions AdminPermissions => AdminPermissionsImpl;

        #region Administrator

        public int AdminId => AdminInfo?.Id ?? 0;

        public string AdminName => AdminInfo?.UserName ?? string.Empty;

        public bool IsAdminLoggin => AdminInfo != null;

        public AdministratorInfo AdminInfo { get; private set; }

        public string AdminToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(CookieUtils.GetCookie(AuthKeyAdminCookie)))
                {
                    accessTokenStr = CookieUtils.GetCookie(AuthKeyAdminCookie);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(AuthKeyAdminHeader)))
                {
                    accessTokenStr = HttpRequest.Headers.Get(AuthKeyAdminHeader);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.QueryString[AuthKeyAdminQuery]))
                {
                    accessTokenStr = HttpRequest.QueryString[AuthKeyAdminQuery];
                }

                if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
                {
                    accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        private void AuthAdministrator()
        {
            var tokenObj = ParseAccessToken(AdminToken);
            if (tokenObj.UserId <= 0 || string.IsNullOrEmpty(tokenObj.UserName)) return;
            var adminInfo = AdminManager.GetAdminInfoByUserId(tokenObj.UserId);
            if (adminInfo != null && !adminInfo.IsLockedOut && adminInfo.UserName == tokenObj.UserName)
            {
                AdminInfo = adminInfo;
            }
        }

        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.IsLockedOut) return null;

            AdminInfo = adminInfo;

            var expiresAt = DateTime.Now.AddDays(AccessTokenExpireDays);
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
            CookieUtils.Erase(AuthKeyAdminCookie);
        }

        #endregion

        #region ApiKey

        private string ApiToken { get; set; }

        private void AuthApi()
        {
            if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(AuthKeyApiHeader)))
            {
                ApiToken = HttpRequest.Headers.Get(AuthKeyApiHeader);
            }
            else if (!string.IsNullOrEmpty(HttpRequest.QueryString[AuthKeyApiQuery]))
            {
                ApiToken = HttpRequest.QueryString[AuthKeyApiQuery];
            }
            else if (!string.IsNullOrEmpty(CookieUtils.GetCookie(AuthKeyApiCookie)))
            {
                ApiToken = CookieUtils.GetCookie(AuthKeyApiCookie);
            }

            if (!string.IsNullOrEmpty(ApiToken))
            {
                var tokenInfo = AccessTokenManager.GetAccessTokenInfo(ApiToken);
                var adminInfo = AdminManager.GetAdminInfoByUserName(tokenInfo?.AdminName);
                if (adminInfo != null && !adminInfo.IsLockedOut)
                {
                    AdminInfo = adminInfo;
                }
                
                IsApiAuthenticated = tokenInfo != null;
            }
        }

        public bool IsApiAuthenticated { get; private set; }

        public bool IsApiAuthorized => IsApiAuthenticated && !string.IsNullOrEmpty(_scope) && AccessTokenManager.IsScope(ApiToken, _scope);

        public bool IsUserAuthorized(int userId)
        {
            var isAuthorized = false;
            if (IsApiAuthenticated && IsApiAuthorized)
            {
                isAuthorized = true;
            }
            else if (IsUserLoggin && UserId == userId)
            {
                isAuthorized = true;
            }

            return isAuthorized;
        }

        #endregion

        #region User

        private string UserToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (!string.IsNullOrEmpty(CookieUtils.GetCookie(AuthKeyUserCookie)))
                {
                    accessTokenStr = CookieUtils.GetCookie(AuthKeyUserCookie);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(AuthKeyUserHeader)))
                {
                    accessTokenStr = HttpRequest.Headers.Get(AuthKeyUserHeader);
                }
                else if (!string.IsNullOrEmpty(HttpRequest.QueryString[AuthKeyUserQuery]))
                {
                    accessTokenStr = HttpRequest.QueryString[AuthKeyUserQuery];
                }

                if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
                {
                    accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        private void AuthUser()
        {
            var userToken = ParseAccessToken(UserToken);
            if (userToken.UserId <= 0 || string.IsNullOrEmpty(userToken.UserName)) return;
            var userInfo = UserManager.GetUserInfoByUserId(userToken.UserId);
            if (userInfo != null && !userInfo.IsLockedOut && userInfo.IsChecked && userInfo.UserName == userToken.UserName)
            {
                UserInfo = userInfo;
            }
        }

        public bool IsUserLoggin => UserInfo != null;

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public UserInfo UserInfo { get; private set; }

        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.IsLockedOut || !userInfo.IsChecked) return null;

            UserInfo = userInfo;

            var expiresAt = DateTime.Now.AddDays(AccessTokenExpireDays);
            var accessToken = GetAccessToken(UserId, UserName, expiresAt);

            DataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(UserInfo);
            LogUtils.AddUserLoginLog(userName);

            if (isAutoLogin)
            {
                CookieUtils.SetCookie(AuthKeyUserCookie, accessToken, expiresAt);
                CookieUtils.SetCookie(CookieUser, TranslateUtils.JsonSerialize(UserInfo), expiresAt, false);
                CookieUtils.SetCookie(CookieUserExpiresAt, DateUtils.ToUnixTime(expiresAt).ToString(), expiresAt, false);
            }
            else
            {
                CookieUtils.SetCookie(AuthKeyUserCookie, accessToken);
                CookieUtils.SetCookie(CookieUser, TranslateUtils.JsonSerialize(UserInfo), false);
                CookieUtils.SetCookie(CookieUserExpiresAt, DateUtils.ToUnixTime(expiresAt).ToString(), false);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            UserInfo = null;
            CookieUtils.Erase(AuthKeyUserCookie);
            CookieUtils.Erase(CookieUser);
            CookieUtils.Erase(CookieUserExpiresAt);
        }

        #endregion

        #region Utils

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

        #endregion
    }
}