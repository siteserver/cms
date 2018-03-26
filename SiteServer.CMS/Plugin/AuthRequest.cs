using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Auth;

namespace SiteServer.CMS.Plugin
{
    public class AuthRequest : IRequest
    {
        private const string UserAccessToken = "ss_user_access_token";
        private const string AdministratorAccessToken = "ss_administrator_access_token";
        private const int AccessTokenExpireDays = 7;

        public AuthRequest(): this(HttpContext.Current.Request)
        {
        }

        public AuthRequest(HttpRequest request)
        {
            HttpRequest = request;

            UserAuthentication();
            AdministratorAuthentication();
        }

        private JObject _postData;
        public JObject PostData
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

        public NameValueCollection Form => HttpRequest.Form;

        public int SiteId => GetQueryInt("siteId");

        public int ChannelId => GetQueryInt("channelId");

        public int ContentId => GetQueryInt("contentId");

        public string ReturnUrl => GetQueryString("ReturnUrl");

        public bool IsQueryExists(string name)
        {
            return HttpRequest.QueryString[name] != null;
        }

        public string GetQueryString(string name)
        {
            return !string.IsNullOrEmpty(HttpRequest.QueryString[name]) ? PageUtils.FilterSql(HttpRequest.QueryString[name]) : null;
        }

        public int GetQueryInt(string name, int defaultValue = 0)
        {
            return !string.IsNullOrEmpty(HttpRequest.QueryString[name]) ? TranslateUtils.ToInt(HttpRequest.QueryString[name]) : defaultValue;
        }

        public decimal GetQueryDecimal(string name, decimal defaultValue = 0)
        {
            return !string.IsNullOrEmpty(HttpRequest.QueryString[name]) ? TranslateUtils.ToDecimal(HttpRequest.QueryString[name]) : defaultValue;
        }

        public bool GetQueryBool(string name, bool defaultValue = false)
        {
            var str = HttpRequest.QueryString[name];
            var retval = !string.IsNullOrEmpty(str) ? TranslateUtils.ToBool(str) : defaultValue;
            return retval;
        }

        public bool IsPostExists(string name)
        {
            JToken value;
            return PostData.TryGetValue(name, out value);
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
            return TranslateUtils.ToInt(PostData[name]?.ToString(), defaultValue);
        }

        public decimal GetPostDecimal(string name, decimal defaultValue = 0)
        {
            return TranslateUtils.ToDecimal(PostData[name]?.ToString(), defaultValue);
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
            if (siteId <= 0)
            {
                LogUtils.AddAdminLog(AdminName, action, summary);
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(action))
                    {
                        action = StringUtils.MaxLengthText(action, 250);
                    }
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = StringUtils.MaxLengthText(summary, 250);
                    }
                    if (channelId < 0)
                    {
                        channelId = -channelId;
                    }
                    var logInfo = new SiteLogInfo(0, siteId, channelId, contentId, AdminName, PageUtils.GetIpAddress(), DateTime.Now, action, summary);
                    DataProvider.SiteLogDao.Insert(logInfo);
                }
                catch
                {
                    // ignored
                }
            }
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

        #region Administrator

        public string AdminName { get; private set; }

        public PermissionManager AdminPermissions { get; private set; }

        public bool IsAdminLoggin => !string.IsNullOrEmpty(AdminName) && AdminName != AdminManager.AnonymousUserName;

        private AdministratorInfo _adminInfo;
        public IAdministratorInfo AdminInfo
        {
            get
            {
                if (_adminInfo != null) return _adminInfo;

                if (!string.IsNullOrEmpty(AdminName))
                {
                    _adminInfo = AdminManager.GetAdminInfo(AdminName);
                }
                return _adminInfo ?? (_adminInfo = new AdministratorInfo());
            }
        }

        protected void AdministratorAuthentication()
        {
            var administratorTokenStr = string.Empty;
            if (!string.IsNullOrEmpty(CookieUtils.GetCookie(AdministratorAccessToken)))
            {
                administratorTokenStr = CookieUtils.GetCookie(AdministratorAccessToken);
            }
            else if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(AdministratorAccessToken)))
            {
                administratorTokenStr = HttpRequest.Headers.Get(AdministratorAccessToken);
            }
            else if (!string.IsNullOrEmpty(HttpRequest.QueryString[AdministratorAccessToken]))
            {
                administratorTokenStr = HttpRequest.QueryString[AdministratorAccessToken];
            }

            AdminName = string.IsNullOrEmpty(administratorTokenStr) ? AdminManager.AnonymousUserName : GetAdministratorToken(administratorTokenStr).AdministratorName;
            AdminPermissions = PermissionManager.GetInstance(AdminName);
        }

        public string GetAdminNameByToken(string token)
        {
            var userToken = GetAdministratorToken(token);
            return userToken?.AdministratorName;
        }

        public static AdministratorToken GetAdministratorToken(string tokenStr)
        {
            if (string.IsNullOrEmpty(tokenStr)) return new AdministratorToken();

            try
            {
                var userToken = JsonWebToken.DecodeToObject<AdministratorToken>(tokenStr, WebConfigUtils.SecretKey);

                if (userToken.AddDate.AddDays(AccessTokenExpireDays) > DateTime.Now)
                {
                    return userToken;
                }
            }
            catch
            {
                // ignored
            }
            return new AdministratorToken();
        }

        public string GetAdminTokenByAdminName(string administratorName)
        {
            if (string.IsNullOrEmpty(administratorName)) return null;

            var administratorToken = new AdministratorToken()
            {
                AdministratorName = administratorName,
                AddDate = DateTime.Now
            };

            return JsonWebToken.Encode(administratorToken, WebConfigUtils.SecretKey, JwtHashAlgorithm.HS256);
        }

        public void AdminLogin(string administratorName)
        {
            if (string.IsNullOrEmpty(administratorName)) return;

            AdminName = administratorName;
            LogUtils.AddAdminLog(administratorName, "管理员登录");
            CookieUtils.SetCookie(AdministratorAccessToken, GetAdminTokenByAdminName(administratorName), DateTime.Now.AddDays(AccessTokenExpireDays));
        }

        public void AdminLogout()
        {
            CookieUtils.Erase(AdministratorAccessToken);
        }

        #endregion

        #region User

        protected void UserAuthentication()
        {
            var userTokenStr = string.Empty;
            if (!string.IsNullOrEmpty(CookieUtils.GetCookie(UserAccessToken)))
            {
                userTokenStr = CookieUtils.GetCookie(UserAccessToken);
            }
            else if (!string.IsNullOrEmpty(HttpRequest.Headers.Get(UserAccessToken)))
            {
                userTokenStr = HttpRequest.Headers.Get(UserAccessToken);
            }
            else if (!string.IsNullOrEmpty(HttpRequest.QueryString[UserAccessToken]))
            {
                userTokenStr = HttpRequest.QueryString[UserAccessToken];
            }

            if (string.IsNullOrEmpty(userTokenStr)) return;

            UserName = GetUserToken(userTokenStr).UserName;
        }

        public bool IsUserLoggin => !string.IsNullOrEmpty(UserName);

        public string UserName { get; private set; }

        private UserInfo _userInfo;
        public IUserInfo UserInfo
        {
            get
            {
                if (_userInfo != null) return _userInfo;

                if (!string.IsNullOrEmpty(UserName))
                {
                    _userInfo = DataProvider.UserDao.GetUserInfoByUserName(UserName);
                }
                return _userInfo ?? (_userInfo = new UserInfo());
            }
        }

        public string GetUserNameByToken(string token)
        {
            var userToken = GetUserToken(token);
            return userToken?.UserName;
        }

        public static UserToken GetUserToken(string tokenStr)
        {
            if (string.IsNullOrEmpty(tokenStr)) return new UserToken();

            try
            {
                var userToken = JsonWebToken.DecodeToObject<UserToken>(tokenStr, WebConfigUtils.SecretKey);

                if (userToken.AddDate.AddDays(AccessTokenExpireDays) > DateTime.Now)
                {
                    return userToken;
                }
            }
            catch
            {
                // ignored
            }
            return new UserToken();
        }

        public string GetUserTokenByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userToken = new UserToken
            {
                UserName = userName,
                AddDate = DateTime.Now
            };

            return JsonWebToken.Encode(userToken, WebConfigUtils.SecretKey, JwtHashAlgorithm.HS256);
        }

        public void UserLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;

            UserName = userName;
            LogUtils.AddUserLoginLog(userName);
            CookieUtils.SetCookie(UserAccessToken, GetUserTokenByUserName(userName), DateTime.Now.AddDays(AccessTokenExpireDays));
        }

        public void UserLogout()
        {
            UserName = null;
            CookieUtils.Erase(UserAccessToken);
        }

        #endregion
    }
}