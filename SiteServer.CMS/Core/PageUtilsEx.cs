using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class PageUtilsEx
    {
        public static string GetHost()
        {
            var host = string.Empty;
            if (HttpContext.Current == null) return string.IsNullOrEmpty(host) ? string.Empty : host.Trim().ToLower();
            host = HttpContext.Current.Request.Headers["HOST"];
            if (string.IsNullOrEmpty(host))
            {
                host = HttpContext.Current.Request.Url.Host;
            }

            return string.IsNullOrEmpty(host) ? string.Empty : host.Trim().ToLower();
        }

        public static string GetScheme()
        {
            var scheme = string.Empty;
            if (HttpContext.Current != null)
            {
                scheme = HttpContext.Current.Request.Headers["SCHEME"];
                if (string.IsNullOrEmpty(scheme))
                {
                    scheme = HttpContext.Current.Request.Url.Scheme;
                }
            }

            return string.IsNullOrEmpty(scheme) ? "http" : scheme.Trim().ToLower();
        }

        public static string ApplicationPath => HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath) ? HttpContext.Current.Request.ApplicationPath : "/";



        // 系统根目录访问地址
        public static string GetRootUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, relatedUrl);
        }

        public static string HttpContextRootDomain
        {
            get
            {
                var url = HttpContext.Current.Request.Url;

                if (url.HostNameType != UriHostNameType.Dns) return url.Host;

                var match = Regex.Match(url.Host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
                return match.Groups[1].Success ? match.Groups[1].Value : null;
            }
        }

        public static string GetIpAddress()
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

            return result;
        }

        public static string SessionId
        {
            get
            {
                var sessionId = CookieUtils.GetCookie("SiteServer.SessionID");
                if (!string.IsNullOrEmpty(sessionId)) return sessionId;
                long i = 1;
                foreach (var b in Guid.NewGuid().ToByteArray())
                {
                    i *= b + 1;
                }
                sessionId = $"{i - DateTime.Now.Ticks:x}";
                CookieUtils.SetCookie("SiteServer.SessionID", sessionId, DateTime.Now.AddDays(100));
                return sessionId;
            }
        }

        public static string GetRefererUrl()
        {
            var url = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            return url;
        }

        public static string GetReturnUrl(bool toReferer)
        {
            var redirectUrl = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]))
            {
                redirectUrl = PageUtilsEx.ParseNavigationUrl(HttpContext.Current.Request.QueryString["ReturnUrl"]);
            }
            else if (toReferer)
            {
                var referer = GetRefererUrl();
                redirectUrl = !string.IsNullOrEmpty(referer) ? referer : GetHost();
            }
            return redirectUrl;
        }

        public static string GetReturnUrl()
        {
            return GetReturnUrl(true);
        }

        public static void Redirect(string url)
        {
            var response = HttpContext.Current.Response;
            response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            response.BufferOutput = true;//设置输出缓冲
            if (!response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
            {
                response.Redirect(url, true);
            }
        }

        public static void Download(HttpResponse response, string filePath, string fileName)
        {
            var fileType = PathUtils.GetExtension(filePath);
            var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileType);
            response.Buffer = true;
            response.Clear();
            response.ContentType = EFileSystemTypeUtils.GetResponseContentType(fileSystemType);
            response.AddHeader("Content-Disposition", "attachment; filename=" + PageUtils.UrlEncode(fileName));
            response.WriteFile(filePath);
            response.Flush();
            response.End();
        }

        public static void Download(HttpResponse response, string filePath)
        {
            var fileName = PathUtils.GetFileName(filePath);
            Download(response, filePath, fileName);
        }

        public static string GetMainUrl(int siteId)
        {
            return GetAdminUrl($"main.cshtml?siteId={siteId}");
        }

        public static string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, WebConfigUtils.AdminDirectory, relatedUrl);
        }

        public static string GetHomeUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, WebConfigUtils.HomeDirectory, relatedUrl);
        }

        public static string GetSiteFilesUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, relatedUrl);
        }

        public static string GetTemporaryFilesUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedUrl);
        }

        public static string GetSiteTemplatesUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedUrl);
        }

        public static string GetSiteTemplateMetadataUrl(string siteTemplateUrl, string relatedUrl)
        {
            return PageUtils.Combine(siteTemplateUrl, DirectoryUtils.SiteTemplates.SiteTemplateMetadata, relatedUrl);
        }

        public static string ParsePluginUrl(string pluginId, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (PageUtils.IsProtocolUrl(url)) return url;

            if (StringUtils.StartsWith(url, "~/"))
            {
                return GetRootUrl(url.Substring(1));
            }

            if (StringUtils.StartsWith(url, "@/"))
            {
                return GetAdminUrl(url.Substring(1));
            }

            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, url));
        }

        public static string GetSiteServerUrl(string className)
        {
            return GetAdminUrl(className.ToCamelCase() + ".cshtml");
        }

        public static string GetSiteServerUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(className.ToCamelCase() + ".aspx"), queryString);
        }

        public static string GetPluginsUrl(string className)
        {
            return GetAdminUrl(PageUtils.Combine("plugins", className.ToCamelCase() + ".cshtml"));
        }

        public static string GetPluginsUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(PageUtils.Combine("plugins", className.ToCamelCase() + ".aspx")), queryString);
        }

        public static string GetSettingsUrl(string className)
        {
            return GetAdminUrl(PageUtils.Combine("settings", className.ToCamelCase() + ".cshtml"));
        }

        public static string GetSettingsUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(PageUtils.Combine("settings", className.ToCamelCase() + ".aspx")), queryString);
        }

        public static string GetCmsUrl(string pageName, int siteId, object param = null)
        {
            var url = GetAdminUrl(PageUtils.Combine("cms", $"{pageName.ToCamelCase()}.cshtml?siteId={siteId}"));
            return param == null ? url : param.GetType().GetProperties().Aggregate(url, (current, p) => current + $"&{p.Name.ToCamelCase()}={p.GetValue(param)}");
        }

        public static string GetCmsUrl(int siteId, string className, NameValueCollection queryString)
        {
            queryString = queryString ?? new NameValueCollection();
            queryString.Remove("siteId");
            return PageUtils.AddQueryString(GetAdminUrl($"cms/{className.ToCamelCase()}.aspx?siteId={siteId}"), queryString);
        }

        public static string GetCmsWebHandlerUrl(int siteId, string className, NameValueCollection queryString)
        {
            queryString = queryString ?? new NameValueCollection();
            queryString.Remove("siteId");
            return PageUtils.AddQueryString(GetAdminUrl($"cms/{className.ToCamelCase()}.ashx?siteId={siteId}"), queryString);
        }

        public static string GetAjaxUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(PageUtils.Combine("ajax", className.ToLower() + ".aspx")), queryString);
        }

        public static void RedirectToErrorPage(int logId)
        {
            Redirect(GetErrorPageUrl(logId));
        }

        public static void RedirectToErrorPage(string message)
        {
            Redirect(GetErrorPageUrl(message));
        }

        public static string GetErrorPageUrl(int logId)
        {
            return GetAdminUrl($"pageError.html?logId={logId}");
        }

        public static string GetErrorPageUrl(string message)
        {
            return GetAdminUrl($"pageError.html?message={HttpUtility.UrlPathEncode(message)}");
        }

        public static void CheckRequestParameter(params string[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter) && HttpContext.Current.Request.QueryString[parameter] == null)
                {
                    Redirect(GetErrorPageUrl(MessageUtils.PageErrorParameterIsNotCorrect));
                    return;
                }
            }
        }

        public static string GetLoginUrl()
        {
            return GetAdminUrl("pageLogin.cshtml");
        }

        public static void RedirectToLoginPage()
        {
            Redirect(GetLoginUrl());
        }

        public static string GetRootUrlByPhysicalPath(string physicalPath)
        {
            var requestPath = PathUtils.GetPathDifference(WebConfigUtils.PhysicalApplicationPath, physicalPath);
            requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            return GetRootUrl(requestPath);
        }

        public static string GetLoadingUrl(string url)
        {
            return GetAdminUrl($"loading.aspx?redirectUrl={TranslateUtils.EncryptStringBySecretKey(url)}");
        }

        public static string ParseNavigationUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            url = url.StartsWith("~") ? PageUtils.Combine(ApplicationPath, url.Substring(1)) : url;
            url = url.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            return url;
        }

        public static string AddProtocolToUrl(string url)
        {
            return AddProtocolToUrl(url, string.Empty);
        }

        /// <summary>
        /// 按照给定的host，添加Protocol
        /// Demo: 发送的邮件中，需要内容标题的链接为全连接，那么需要指定他的host
        /// </summary>
        /// <param name="url"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string AddProtocolToUrl(string url, string host)
        {
            if (url == PageUtils.UnclickedUrl)
            {
                return url;
            }
            var retval = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                if (PageUtils.IsProtocolUrl(url))
                {
                    retval = url;
                }
                else
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        retval = url.StartsWith("/") ? GetScheme() + "://" + GetHost() + url : GetScheme() + "://" + url;
                    }
                    else
                    {
                        retval = url.StartsWith("/") ? host.TrimEnd('/') + url : host + url;
                    }
                }
            }
            return retval;
        }

        public static string GetUrlWithReturnUrl(string pageUrl, string returnUrl)
        {
            var retval = pageUrl;
            returnUrl = $"ReturnUrl={returnUrl}";
            if (pageUrl.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                if (pageUrl.EndsWith("&"))
                {
                    retval += returnUrl;
                }
                else
                {
                    retval += "&" + returnUrl;
                }
            }
            else
            {
                retval += "?" + returnUrl;
            }
            return ParseNavigationUrl(retval);
        }

        public static string GetUrlByBaseUrl(string rawUrl, string baseUrl)
        {
            var url = string.Empty;
            if (!string.IsNullOrEmpty(rawUrl))
            {
                rawUrl = rawUrl.Trim().TrimEnd('#');
            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = baseUrl.Trim();
            }
            if (!string.IsNullOrEmpty(rawUrl))
            {
                rawUrl = rawUrl.Trim();
                if (PageUtils.IsProtocolUrl(rawUrl))
                {
                    url = rawUrl;
                }
                else if (rawUrl.StartsWith("/"))
                {
                    var domain = PageUtils.GetUrlWithoutPathInfo(baseUrl);
                    url = domain + rawUrl;
                }
                else if (rawUrl.StartsWith("../"))
                {
                    var count = StringUtils.GetStartCount("../", rawUrl);
                    rawUrl = rawUrl.Remove(0, 3 * count);
                    baseUrl = PageUtils.GetUrlWithoutFileName(baseUrl).TrimEnd('/');
                    baseUrl = PageUtils.RemoveProtocolFromUrl(baseUrl);
                    for (var i = 0; i < count; i++)
                    {
                        var j = baseUrl.LastIndexOf('/');
                        if (j != -1)
                        {
                            baseUrl = StringUtils.Remove(baseUrl, j);
                        }
                        else
                        {
                            break;
                        }
                    }
                    url = PageUtils.Combine(AddProtocolToUrl(baseUrl), rawUrl);
                }
                else
                {
                    if (baseUrl != null && baseUrl.EndsWith("/"))
                    {
                        url = baseUrl + rawUrl;
                    }
                    else
                    {
                        var urlWithoutFileName = PageUtils.GetUrlWithoutFileName(baseUrl);
                        if (!urlWithoutFileName.EndsWith("/"))
                        {
                            urlWithoutFileName += "/";
                        }
                        url = urlWithoutFileName + rawUrl;
                    }
                }
            }
            return url;
        }

        public static string ParseConfigRootUrl(string url)
        {
            return ParseNavigationUrl(url);
        }
    }
}
