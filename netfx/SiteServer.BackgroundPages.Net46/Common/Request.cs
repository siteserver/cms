using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Owin;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request : IRequest
    {
        private readonly HttpContext _httpContext;
        private AdministratorInfo _adminInfo;
        private readonly UserInfo _userInfo;

        public static Request Current
        {
            get
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null) return null;

                var owinContext = httpContext.GetOwinContext();
                var request = owinContext.Get<Request>("SiteServer.BackgroundPages.Common.Request");
                if (request != null) return request;

                request = new Request(owinContext, httpContext);
                owinContext.Set("SiteServer.BackgroundPages.Common.Request", request);
                return request;
            }
        }

        private Request(IOwinContext owinContext, HttpContext httpContext)
        {
            _httpContext = httpContext;

            Cookies = new RequestCookieCollection(_httpContext.Request.Cookies);
            Headers = new HeaderDictionary(owinContext.Request.Headers);
            RouteValues = _httpContext.Request.RequestContext.RouteData.Values.ToDictionary(x => x.Key, x => x.Value);

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

        public int SiteId => GetQueryInt("siteId");

        public int ChannelId => GetQueryInt("channelId");

        public int ContentId => GetQueryInt("contentId");

        public string Path => _httpContext.Request.Path;

        public string RawUrl => _httpContext.Request.RawUrl;

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

        public HttpFileCollection Files => _httpContext.Request.Files;

        public Stream Body => _httpContext.Request.InputStream;

        public bool IsHttps => _httpContext.Request.IsSecureConnection;

        public string Host
        {
            get
            {
                var url = _httpContext.Request.Url;

                if (url.HostNameType != UriHostNameType.Dns) return url.Host;

                var match = Regex.Match(url.Host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
                return match.Groups[1].Success ? match.Groups[1].Value : null;
            }
        }

        public Microsoft.AspNetCore.Http.IRequestCookieCollection Cookies { get; }

        public Microsoft.AspNetCore.Http.IHeaderDictionary Headers { get; }

        public IDictionary<string, object> RouteValues { get; }

        public IFormCollection Form { get; }
}
}