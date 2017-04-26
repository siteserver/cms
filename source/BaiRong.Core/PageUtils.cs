using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
    public class PageUtils
    {
        public const char SeparatorChar = '/';

        public const string UnclickedUrl = "javascript:;";

        public static string ParseNavigationUrl(string url)
        {
            //			string retval = string.Empty;
            //			if (string.IsNullOrEmpty(url))
            //			{
            //				return retval;
            //			}
            //			if (url.StartsWith("~"))
            //			{
            //				retval = Combine(HttpContext.Current.Request.ApplicationPath ,url.Substring(1));
            //			}
            //			else
            //			{
            //				retval = url;
            //			}
            //			return retval;
            //            //return AddProtocolToUrl(retval);
            return ParseNavigationUrl(url, WebConfigUtils.ApplicationPath);
        }

        public static string ParseNavigationUrl(string url, string domainUrl)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            return url.StartsWith("~") ? Combine(domainUrl, url.Substring(1)) : url;
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
            if (url == UnclickedUrl)
            {
                return url;
            }
            var retval = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                if (IsProtocolUrl(url))
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

        public static string AddQuestionOrAndToUrl(string pageUrl)
        {
            var url = pageUrl;
            if (string.IsNullOrEmpty(url))
            {
                url = "?";
            }
            else
            {
                if (url.IndexOf('?') == -1)
                {
                    url = url + "?";
                }
                else if (!url.EndsWith("?"))
                {
                    url = url + "&";
                }
            }
            return url;
        }

        public static string RemovePortFromUrl(string url)
        {
            var retval = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                var regex = new Regex(@":\d+");
                retval = regex.Replace(url, "");
            }
            return retval;
        }

        public static string RemoveFileNameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            url = url.Trim();
            if (url.Contains("/"))
            {
                var fileName = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal));
                if (fileName.Contains("."))
                {
                    return url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));
                }
            }

            return url;
        }

        public static string RemoveProtocolFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            url = url.Trim();
            return IsProtocolUrl(url) ? url.Substring(url.IndexOf("://", StringComparison.Ordinal) + 3) : url;
        }

        public static bool IsProtocolUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            url = url.Trim();
            return url.IndexOf("://", StringComparison.Ordinal) != -1 || url.StartsWith("javascript:");
        }

        public static string GetAbsoluteUrl()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public static string PathDifference(string path1, string path2, bool compareCase)
        {
            var num2 = -1;
            var num1 = 0;
            while ((num1 < path1.Length) && (num1 < path2.Length))
            {
                if ((path1[num1] != path2[num1]) && (compareCase || (char.ToLower(path1[num1], CultureInfo.InvariantCulture) != char.ToLower(path2[num1], CultureInfo.InvariantCulture))))
                {
                    break;
                }
                if (path1[num1] == '/')
                {
                    num2 = num1;
                }
                num1++;
            }
            if (num1 == 0)
            {
                return path2;
            }
            if ((num1 == path1.Length) && (num1 == path2.Length))
            {
                return string.Empty;
            }
            var builder1 = new StringBuilder();
            while (num1 < path1.Length)
            {
                if (path1[num1] == '/')
                {
                    builder1.Append("../");
                }
                num1++;
            }
            return (builder1 + path2.Substring(num2 + 1));
        }

        /// <summary>
        /// 获取服务器根域名  
        /// </summary>
        /// <returns></returns>
        public static string GetMainDomain(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;

            url = RemoveProtocolFromUrl(url.ToLower());
            if (url.IndexOf('/') != -1)
            {
                url = url.Substring(0, url.IndexOf('/'));
            }

            if (url.IndexOf('.') <= 0) return url;

            var strArr = url.Split('.');
            var lastStr = strArr.GetValue(strArr.Length - 1).ToString();
            if (StringUtils.IsNumber(lastStr)) //如果最后一位是数字，那么说明是IP地址
            {
                return url;
            }
            var domainRules = ".com.cn|.net.cn|.org.cn|.gov.cn|.com|.net|.cn|.org|.cc|.me|.tel|.mobi|.asia|.biz|.info|.name|.tv|.hk|.公司|.中国|.网络".Split('|');
            var returnStr = string.Empty;
            foreach (var t in domainRules)
            {
                if (url.EndsWith(t.ToLower())) //如果最后有找到匹配项
                {
                    var findStr = t;
                    var replaceStr = url.Replace(findStr, "");
                    if (replaceStr.IndexOf('.') > 0) //存在二级域名或者三级，比如：www.px915
                    {
                        var replaceArr = replaceStr.Split('.'); // www px915
                        returnStr = replaceArr.GetValue(replaceArr.Length - 1) + findStr;
                        return returnStr;
                    }
                    returnStr = replaceStr + findStr; //连接起来输出为：px915.com
                    return returnStr;
                }
                returnStr = url;
            }
            return returnStr;
        }

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

            return (string.IsNullOrEmpty(scheme)) ? "http" : scheme.Trim().ToLower();
        }

        // 系统根目录访问地址
        public static string GetRootUrl(string relatedUrl)
        {
            return Combine(WebConfigUtils.ApplicationPath, relatedUrl);
        }

        public static string GetTemporaryFilesUrl(string relatedUrl)
        {
            return Combine(WebConfigUtils.ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedUrl);
        }

        public static NameValueCollection GetQueryString(string url)
        {
            if (string.IsNullOrEmpty(url) || url.IndexOf("?", StringComparison.Ordinal) == -1) return new NameValueCollection();

            var querystring = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
            return TranslateUtils.ToNameValueCollection(querystring);
        }

        public static NameValueCollection GetQueryStringFilterXss(string url)
        {
            if (string.IsNullOrEmpty(url) || url.IndexOf("?", StringComparison.Ordinal) == -1) return new NameValueCollection();

            var attributes = new NameValueCollection();
            
            var querystring = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
            var originals = TranslateUtils.ToNameValueCollection(querystring);
            foreach (string key in originals.Keys)
            {
                attributes[key] = FilterXss(originals[key]);
            }
            return attributes;
        }

        public static string Combine(params string[] urls)
        {
            if (urls == null || urls.Length <= 0) return string.Empty;

            var retval = urls[0]?.Replace(PathUtils.SeparatorChar, SeparatorChar) ?? string.Empty;
            for (var i = 1; i < urls.Length; i++)
            {
                var url = (urls[i] != null) ? urls[i].Replace(PathUtils.SeparatorChar, SeparatorChar) : string.Empty;
                retval = Combine(retval, url);
            }
            return retval;
        }

        private static string Combine(string url1, string url2)
        {
            if (url1 == null || url2 == null)
            {
                throw new ArgumentNullException(url1 == null ? "url1" : "url2");
            }
            if (url2.Length == 0)
            {
                return url1;
            }
            if (url1.Length == 0)
            {
                return url2;
            }

            return (url1.TrimEnd(SeparatorChar) + SeparatorChar + url2.TrimStart(SeparatorChar));
        }

        public static string AddQueryString(string url, string queryStringKey, string queryStringValue)
        {
            var queryString = new NameValueCollection
            {
                {queryStringKey, queryStringValue}
            };
            return AddQueryString(url, queryString);
        }

        public static string AddQueryString(string url, string queryString)
        {
            if (queryString == null || url == null) return url;

            queryString = queryString.TrimStart('?', '&');

            if (url.IndexOf("?", StringComparison.Ordinal) == -1)
            {
                return string.Concat(url, "?", queryString);
            }
            return url.EndsWith("?") ? string.Concat(url, queryString) : string.Concat(url, "&", queryString);
        }

        public static string AddQueryString(string url, NameValueCollection queryString)
        {
            if (queryString == null || url == null || queryString.Count == 0)
                return url;

            var builder = new StringBuilder();
            foreach (string key in queryString.Keys)
            {
                builder.Append($"&{key}={HttpUtility.UrlEncode(queryString[key])}");
            }
            if (url.IndexOf("?", StringComparison.Ordinal) == -1)
            {
                if (builder.Length > 0) builder.Remove(0, 1);
                return string.Concat(url, "?", builder.ToString());
            }
            if (url.EndsWith("?"))
            {
                if (builder.Length > 0) builder.Remove(0, 1);
            }
            return string.Concat(url, builder.ToString());
        }

        public static string RemoveQueryString(string url, string queryString)
        {
            if (queryString == null || url == null) return url;

            if (url.IndexOf("?", StringComparison.Ordinal) == -1 || url.EndsWith("?"))
            {
                return url;
            }
            var attributes = GetQueryString(url);
            attributes.Remove(queryString);
            url = url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));
            return AddQueryString(url, attributes);
        }

        public static string GetIpAddress()
        {
            //取CDN用户真实IP的方法
            //当用户使用代理时，取到的是代理IP
            var result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
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
                            if (IsIp(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                            {
                                result = t;
                            }
                        }
                        var str = result.Split(',');
                        if (str.Length > 0)
                            result = str[0].Trim();
                    }
                    else if (IsIp(result))
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

            return result;
        }

        public static bool IsIp(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
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

        public static string GetReturnUrl()
        {
            return GetReturnUrl(true);
        }

        public static string GetReturnUrl(bool toReferer)
        {
            var redirectUrl = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]))
            {
                redirectUrl = ParseNavigationUrl(HttpContext.Current.Request.QueryString["ReturnUrl"]);
            }
            else if (toReferer)
            {
                var referer = GetRefererUrl();
                redirectUrl = !string.IsNullOrEmpty(referer) ? referer : GetHost();
            }
            return redirectUrl;
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
                if (IsProtocolUrl(rawUrl))
                {
                    url = rawUrl;
                }
                else if (rawUrl.StartsWith("/"))
                {
                    var domain = GetUrlWithoutPathInfo(baseUrl);
                    url = domain + rawUrl;
                }
                else if (rawUrl.StartsWith("../"))
                {
                    var count = StringUtils.GetStartCount("../", rawUrl);
                    rawUrl = rawUrl.Remove(0, 3 * count);
                    baseUrl = GetUrlWithoutFileName(baseUrl).TrimEnd('/');
                    baseUrl = RemoveProtocolFromUrl(baseUrl);
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
                    url = Combine(AddProtocolToUrl(baseUrl), rawUrl);
                }
                else
                {
                    if (baseUrl != null && baseUrl.EndsWith("/"))
                    {
                        url = baseUrl + rawUrl;
                    }
                    else
                    {
                        var urlWithoutFileName = GetUrlWithoutFileName(baseUrl);
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

        /// <summary>
        /// 将Url地址的查询字符串去掉
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <returns></returns>
        public static string GetUrlWithoutQueryString(string rawUrl)
        {
            string urlWithoutQueryString;
            if (rawUrl != null && rawUrl.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                var queryString = rawUrl.Substring(rawUrl.IndexOf("?", StringComparison.Ordinal));
                urlWithoutQueryString = rawUrl.Replace(queryString, "");
            }
            else
            {
                urlWithoutQueryString = rawUrl;
            }
            return urlWithoutQueryString;
        }

        /// <summary>
        /// 将Url地址域名后的字符去掉
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <returns></returns>
        public static string GetUrlWithoutPathInfo(string rawUrl)
        {
            var urlWithoutPathInfo = string.Empty;
            if (rawUrl != null && rawUrl.Trim().Length > 0)
            {
                if (rawUrl.ToLower().StartsWith("http://"))
                {
                    urlWithoutPathInfo = rawUrl.Substring("http://".Length);
                }
                if (urlWithoutPathInfo.IndexOf("/", StringComparison.Ordinal) != -1)
                {
                    urlWithoutPathInfo = urlWithoutPathInfo.Substring(0, urlWithoutPathInfo.IndexOf("/", StringComparison.Ordinal));
                }
                if (string.IsNullOrEmpty(urlWithoutPathInfo))
                {
                    urlWithoutPathInfo = rawUrl;
                }
                urlWithoutPathInfo = "http://" + urlWithoutPathInfo;
            }
            return urlWithoutPathInfo;
        }

        /// <summary>
        /// 将Url地址后的文件名称去掉
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <returns></returns>
        public static string GetUrlWithoutFileName(string rawUrl)
        {
            if (string.IsNullOrEmpty(rawUrl)) return string.Empty;

            var urlWithoutFileName = string.Empty;
            if (rawUrl.ToLower().StartsWith("http://"))
            {
                urlWithoutFileName = rawUrl.Substring("http://".Length);
            }
            if (urlWithoutFileName.IndexOf("/", StringComparison.Ordinal) != -1 && !urlWithoutFileName.EndsWith("/"))
            {
                const string regex = "/(?<filename>[^/]*\\.[^/]*)[^/]*$";
                const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
                var reg = new Regex(regex, options);
                var match = reg.Match(urlWithoutFileName);
                if (match.Success)
                {
                    var fileName = match.Groups["filename"].Value;
                    urlWithoutFileName = urlWithoutFileName.Substring(0, urlWithoutFileName.LastIndexOf(fileName, StringComparison.Ordinal));
                }
            }
            urlWithoutFileName = "http://" + urlWithoutFileName;
            return urlWithoutFileName;
        }

        public static string GetUrlQueryString(string url)
        {
            var queryString = string.Empty;
            if (!string.IsNullOrEmpty(url) && url.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                queryString = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
            }
            return queryString;
        }

        public static string GetFileNameFromUrl(string rawUrl)
        {
            if (string.IsNullOrEmpty(rawUrl)) return string.Empty;

            var fileName = string.Empty;
            //if (rawUrl.ToLower().StartsWith("http://"))
            //{
            //    rawUrl = rawUrl.Substring("http://".Length);
            //}
            //if (rawUrl.IndexOf("?") != -1)
            //{
            //    int index = rawUrl.IndexOf("?");
            //    rawUrl = rawUrl.Remove(index, rawUrl.Length - index);
            //}
            rawUrl = RemoveProtocolFromUrl(rawUrl);
            rawUrl = GetUrlWithoutQueryString(rawUrl);
            if (rawUrl.IndexOf("/", StringComparison.Ordinal) != -1 && !rawUrl.EndsWith("/"))
            {
                const string regex = "/(?<filename>[^/]*\\.[^/]*)[^/]*$";
                const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
                var reg = new Regex(regex, options);
                var match = reg.Match(rawUrl);
                if (match.Success)
                {
                    fileName = match.Groups["filename"].Value;
                }
            }
            else
            {
                fileName = rawUrl;
            }
            return fileName;
        }

        public static string GetExtensionFromUrl(string rawUrl)
        {
            var extension = string.Empty;
            if (!string.IsNullOrEmpty(rawUrl))
            {
                rawUrl = RemoveProtocolFromUrl(rawUrl);
                rawUrl = GetUrlWithoutQueryString(rawUrl);
                rawUrl = rawUrl.TrimEnd('/');
                if (rawUrl.IndexOf('/') != -1)
                {
                    rawUrl = rawUrl.Substring(rawUrl.LastIndexOf('/'));
                    if (rawUrl.IndexOf('.') != -1)
                    {
                        extension = rawUrl.Substring(rawUrl.LastIndexOf('.'));
                    }
                }
            }
            return extension;
        }

        public static string UrlEncode(string urlString)
        {
            if (urlString == null || urlString == "$4")
            {
                return string.Empty;
            }

            var newValue = urlString.Replace("\"", "'");
            newValue = HttpUtility.UrlEncode(newValue);
            newValue = newValue?.Replace("%2f", "/");
            return newValue;
        }

        public static string UrlEncode(string urlString, string encoding)
        {
            if (urlString == null || urlString == "$4")
            {
                return string.Empty;
            }

            var newValue = urlString.Replace("\"", "'");
            newValue = HttpUtility.UrlEncode(newValue, Encoding.GetEncoding(encoding));
            newValue = newValue?.Replace("%2f", "/");
            return newValue;
        }

        public static string UrlEncode(string urlString, ECharset charset)
        {
            if (urlString == null || urlString == "$4")
            {
                return string.Empty;
            }

            var newValue = urlString.Replace("\"", "'");
            newValue = HttpUtility.UrlEncode(newValue, ECharsetUtils.GetEncoding(charset));
            newValue = newValue?.Replace("%2f", "/");
            return newValue;
        }

        public static string UrlDecode(string urlString, string encoding)
        {
            return HttpUtility.UrlDecode(urlString, Encoding.GetEncoding(encoding));
        }

        public static string UrlDecode(string urlString, ECharset charset)
        {
            return HttpUtility.UrlDecode(urlString, ECharsetUtils.GetEncoding(charset));
        }

        public static string UrlDecode(string urlString)
        {
            return HttpContext.Current.Server.UrlDecode(urlString);
        }

        public static void Redirect(string url)
        {
            HttpContext.Current.Response.Redirect(url, true);
        }

        public static void Download(HttpResponse response, string filePath, string fileName)
        {
            var fileType = PathUtils.GetExtension(filePath);
            var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileType);
            response.Buffer = true;
            response.Clear();
            response.ContentType = EFileSystemTypeUtils.GetResponseContentType(fileSystemType);
            response.AddHeader("Content-Disposition", "attachment; filename=" + UrlEncode(fileName));
            response.WriteFile(filePath);
            response.Flush();
            response.End();
        }

        public static void Download(HttpResponse response, string filePath)
        {
            var fileName = PathUtils.GetFileName(filePath);
            Download(response, filePath, fileName);
        }

        public static string GetAdminDirectoryUrl(string relatedUrl)
        {
            return Combine(WebConfigUtils.ApplicationPath, FileConfigManager.Instance.AdminDirectoryName, relatedUrl);
        }

        public static string GetSiteServerUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(className.ToLower() + ".aspx"), queryString);
        }

        public static string GetPlatformUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("platform", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetAdminUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("admin", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetAnalysisUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("analysis", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetSysUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("sys", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetUserUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("user", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetServiceUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("service", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetSettingsUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("settings", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetCmsUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("cms", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetAjaxUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("ajax", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetWcmUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("wcm", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetWeiXinUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminDirectoryUrl(Combine("weixin", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetUserFilesUrl(string userName, string relatedUrl)
        {
            if (IsVirtualUrl(relatedUrl))
            {
                return ParseNavigationUrl(relatedUrl);
            }
            return Combine(WebConfigUtils.ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.UserFiles, userName, relatedUrl);
        }

        public static string GetUserFileSystemManagementDirectoryUrl(string userName, string currentRootPath)
        {
            var directoryUrl = string.Empty;
            if (string.IsNullOrEmpty(currentRootPath) || !(currentRootPath.StartsWith("~/")))
            {
                currentRootPath = "~/" + currentRootPath;
            }
            var directoryNames = currentRootPath.Split('/');
            foreach (var directoryName in directoryNames)
            {
                if (!string.IsNullOrEmpty(directoryName))
                {
                    directoryUrl = directoryName.Equals("~") ? GetUserFilesUrl(string.Empty, userName) : Combine(directoryUrl, directoryName);
                }

            }
            return directoryUrl;
        }

        /// <summary>
        /// 判断是否需要安装，并转到页面。
        /// </summary>
        public static bool DetermineRedirectToInstaller()
        {
            if (!AppManager.IsNeedInstall()) return false;
            Redirect(GetAdminDirectoryUrl("Installer"));
            return true;
        }

        public static void RedirectToErrorPage(string errorMessage)
        {
            Redirect(GetAdminDirectoryUrl($"error.aspx?ErrorMessage={StringUtils.ValueToUrl(errorMessage)}"));
        }

        public static void CheckRequestParameter(params string[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter) && HttpContext.Current.Request.QueryString[parameter] == null)
                {
                    RedirectToErrorPage(MessageUtils.PageErrorParameterIsNotCorrect);
                    return;
                }
            }
        }

        public static void RedirectToLoginPage()
        {
            RedirectToLoginPage(string.Empty);
        }

        public static void RedirectToLoginPage(string error)
        {
            var pageUrl = GetAdminDirectoryUrl("login.aspx");

            if (!string.IsNullOrEmpty(error))
            {
                pageUrl = pageUrl + "?error=" + error;
            }
            Redirect(pageUrl);
        }

        public static void RedirectToLoadingPage(string pageUrl)
        {
            string url;
            var loadingPageUrl = GetAdminDirectoryUrl("loading.aspx?RedirectUrl={0}");
            if (pageUrl.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                var redirectUrl = pageUrl.Substring(0, pageUrl.IndexOf("?", StringComparison.Ordinal));
                url = string.Format(loadingPageUrl, redirectUrl);
                url = AddQueryString(url, GetQueryString(pageUrl));
            }
            else
            {
                var redirectUrl = pageUrl;
                url = string.Format(loadingPageUrl, redirectUrl);
            }
            Redirect(url);
        }

        public static string AddReturnUrl(string url, string returnUrl)
        {
            return AddQueryString(url, "ReturnUrl", returnUrl);
        }

        public static string GetRootUrlByPhysicalPath(string physicalPath)
        {
            var requestPath = PathUtils.GetPathDifference(WebConfigUtils.PhysicalApplicationPath, physicalPath);
            requestPath = requestPath.Replace(PathUtils.SeparatorChar, SeparatorChar);
            return GetRootUrl(requestPath);
        }

        public static string GetApiUrl()
        {
            return Combine(WebConfigUtils.ApplicationPath, "api").ToLower();
        }

        public static string GetHomeUrl()
        {
            return Combine(WebConfigUtils.ApplicationPath, "home").ToLower();
        }

        public static string ParseConfigRootUrl(string url)
        {
            return ParseNavigationUrl(url);
        }

        public static string GetFileSystemManagementDirectoryUrl(string currentRootPath, string publishementSystemDir)
        {
            var directoryUrl = string.Empty;
            if (string.IsNullOrEmpty(currentRootPath) || !(currentRootPath.StartsWith("~/") || currentRootPath.StartsWith("@/")))
            {
                currentRootPath = "@/" + currentRootPath;
            }
            var directoryNames = currentRootPath.Split('/');
            foreach (var directoryName in directoryNames)
            {
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (directoryName.Equals("~"))
                    {
                        directoryUrl = WebConfigUtils.ApplicationPath;
                    }
                    else if (directoryName.Equals("@"))
                    {
                        directoryUrl = Combine(WebConfigUtils.ApplicationPath, publishementSystemDir);
                    }
                    else
                    {
                        directoryUrl = Combine(directoryUrl, directoryName);
                    }
                }
            }
            return directoryUrl;
        }

        public static bool IsVirtualUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("~") || url.StartsWith("@"))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetLoadingUrl(string url)
        {
            return GetAdminDirectoryUrl($"loading.aspx?RedirectType=Loading&RedirectUrl={FilterXss(url)}");
        }

        public static string GetSafeHtmlFragment(string content)
        {
            return Microsoft.Security.Application.AntiXss.GetSafeHtmlFragment(content);
        }

        /// <summary> 
        ///sql和xss脚本过滤
        /// </summary> 
        public static string FilterSqlAndXss(string objStr)
        {
            return FilterXss(FilterSql(objStr));
        }

        /// <summary>     
        /// 过滤xss攻击脚本     
        /// </summary>      
        public static string FilterXss(string html)
        {
            var retval = html;
            if (!string.IsNullOrEmpty(retval))
            {
                retval = retval.Replace("@", "_at_");
                retval = retval.Replace("&", "_and_");
                retval = retval.Replace("#", "_sharp_");
                retval = retval.Replace(";", "_semicolon_");
                retval = retval.Replace(":", "_colon_");
                retval = retval.Replace("=", "_equal_");
                retval = retval.Replace("，", "_cn_comma_");
                retval = retval.Replace("“", "_quotel_");
                retval = retval.Replace("”", "_quoter_");
                retval = retval.Replace("/", "_slash_");
                retval = retval.Replace("|", "_or_");
                retval = retval.Replace("-", "_shortOne_");
                retval = retval.Replace(",", "_comma_");

                //中文标点符号
                retval = retval.Replace("；", "_cn_semicolon_");
                retval = retval.Replace("：", "_cn_colon_");
                retval = retval.Replace("。", "_cn_stop_");
                retval = retval.Replace("、", "_cn_tempstop_");
                retval = retval.Replace("？", "_cn_question_");
                retval = retval.Replace("《", "_cn_lbracket_");
                retval = retval.Replace("》", "_cn_rbracket_");
                retval = retval.Replace("‘", "_cn_rmark_");
                retval = retval.Replace("’", "_cn_lmark_");
                retval = retval.Replace("【", "_cn_slbracket_");
                retval = retval.Replace("】", "_cn_srbracket_");
                retval = retval.Replace("——", "_cn_extension_");
                retval = Microsoft.Security.Application.AntiXss.HtmlEncode(retval);
                //中文标点符号
                retval = retval.Replace("_cn_semicolon_", "；");
                retval = retval.Replace("_cn_colon_", "：");
                retval = retval.Replace("_cn_stop_", "。");
                retval = retval.Replace("_cn_tempstop_", "、");
                retval = retval.Replace("_cn_question_", "？");
                retval = retval.Replace("_cn_lbracket_", "《");
                retval = retval.Replace("_cn_rbracket_", "》");
                retval = retval.Replace("_cn_rmark_", "‘");
                retval = retval.Replace("_cn_lmark_", "’");
                retval = retval.Replace("_cn_slbracket_", "【");
                retval = retval.Replace("_cn_srbracket_", "】");
                retval = retval.Replace("_cn_extension_", "——");

                retval = retval.Replace("_at_", "@");
                retval = retval.Replace("_and_", "&");
                retval = retval.Replace("_sharp_", "#");
                retval = retval.Replace("_semicolon_", ";");
                retval = retval.Replace("_colon_", ":");
                retval = retval.Replace("_equal_", "=");
                retval = retval.Replace("_cn_comma_", "，");
                retval = retval.Replace("_quotel_", "“");
                retval = retval.Replace("_quoter_", "”");
                retval = retval.Replace("_slash_", "/");
                retval = retval.Replace("_or_", "|");
                retval = retval.Replace("_shortOne_", "-");
                retval = retval.Replace("_comma_", ",");
            }
            return retval;
        }

        /// <summary> 
        /// 过滤sql攻击脚本 
        /// </summary> 
        public static string FilterSql(string objStr)
        {
            if (string.IsNullOrEmpty(objStr)) return string.Empty;

            var isSqlExists = false;
            const string strSql = "',--,\\(,\\)";
            var strSqls = strSql.Split(',');
            foreach (var sql in strSqls)
            {
                if (objStr.IndexOf(sql, StringComparison.Ordinal) != -1)
                {
                    isSqlExists = true;
                    break;
                }
            }
            if (isSqlExists)
            {
                return objStr.Replace("'", "_sqlquote_").Replace("--", "_sqldoulbeline_").Replace("\\(", "_sqlleftparenthesis_").Replace("\\)", "_sqlrightparenthesis_");
            }
            return objStr;
        }

        public static string UnFilterSql(string objStr)
        {
            if (string.IsNullOrEmpty(objStr)) return string.Empty;

            return objStr.Replace("_sqlquote_", "'").Replace("_sqldoulbeline_", "--").Replace("_sqlleftparenthesis_", "\\(").Replace("_sqlrightparenthesis_", "\\)");
        }

        public static void ResponseToJson(string jsonString)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Write(jsonString);
            HttpContext.Current.Response.End();
        }

        public class Api
        {
            private static string GetUrl(string relatedPath, string proco)
            {
                return Combine(string.IsNullOrEmpty(proco) ? GetRootUrl("api") : proco, relatedPath);
            }

            public static string GetPublishmentSystemClearCacheUrl()
            {
                return GetUrl("cache/clearPublishmentSystemCache", string.Empty);
            }

            public static string GetUserClearCacheUrl()
            {
                return GetUrl("cache/removeUserCache", string.Empty);
            }

            public static string GetTableStyleClearCacheUrl()
            {
                return GetUrl("cache/RemoveTableManagerCache", string.Empty);
            }

            public static string GetUserConfigClearCacheUrl()
            {
                return GetUrl("cache/RemoveUserConfigCache", string.Empty);
            }
        }

        public const string HidePopWin = "if (window.parent.closeWindow) window.parent.closeWindow();if (window.parent.layer) window.parent.layer.closeAll();";

        public const string TipsSuccess = "success";
        public const string TipsError = "error";
        public const string TipsInfo = "info";
        public const string TipsWarn = "warn";

        public static string GetOpenTipsString(string message, string tipsType)
        {
            return $@"openTips('{message}', '{tipsType}');";
        }

        public static string GetOpenTipsString(string message, string tipsType, bool isCloseOnly, string btnValue, string btnClick)
        {
            return
                $@"openTips('{message}', '{tipsType}', '{isCloseOnly.ToString().ToLower()}', '{btnValue}', '{btnClick}');";
        }

        public static string GetOpenWindowString(string title, string pageUrl, int width, int height)
        {
            return GetOpenWindowString(title, pageUrl, width, height, false);
        }

        public static string GetOpenWindowString(string title, string pageUrl)
        {
            return GetOpenWindowString(title, pageUrl, 0, 0, false);
        }

        public static string GetOpenWindowString(string title, string pageUrl, bool isCloseOnly)
        {
            return GetOpenWindowString(title, pageUrl, 0, 0, isCloseOnly);
        }

        public static string GetOpenWindowString(string title, string pageUrl, int width, int height, bool isCloseOnly)
        {
            if (height > 590) height = 0;
            return
                $@"openWindow('{title}','{pageUrl}',{width},{height},'{isCloseOnly.ToString().ToLower()}');return false;";
        }

        public static string GetOpenWindowStringWithTextBoxValue(string title, string pageUrl, string textBoxId)
        {
            return GetOpenWindowStringWithTextBoxValue(title, pageUrl, textBoxId, 0, 0, false);
        }

        public static string GetOpenWindowStringWithTextBoxValue(string title, string pageUrl, string textBoxId, int width, int height)
        {
            return GetOpenWindowStringWithTextBoxValue(title, pageUrl, textBoxId, width, height, false);
        }

        public static string GetOpenWindowStringWithTextBoxValue(string title, string pageUrl, string textBoxId, bool isCloseOnly)
        {
            return GetOpenWindowStringWithTextBoxValue(title, pageUrl, textBoxId, 0, 0, isCloseOnly);
        }

        public static string GetOpenWindowStringWithTextBoxValue(string title, string pageUrl, string textBoxId, int width, int height, bool isCloseOnly)
        {
            if (height > 590) height = 0;
            return
                $@"openWindow('{title}','{pageUrl}' + '&{textBoxId}=' + $('#{textBoxId}').val(),{width}, {height}, '{isCloseOnly
                    .ToString().ToLower()}');return false;";
        }

        public static string GetOpenWindowStringWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText, int width, int height)
        {
            return GetOpenWindowStringWithCheckBoxValue(title, pageUrl, checkBoxId, alertText, width, height, false);
        }

        public static string GetOpenWindowStringWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText)
        {
            return GetOpenWindowStringWithCheckBoxValue(title, pageUrl, checkBoxId, alertText, 0, 0, false);
        }

        public static string GetOpenWindowStringWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText, bool isCloseOnly)
        {
            return GetOpenWindowStringWithCheckBoxValue(title, pageUrl, checkBoxId, alertText, 0, 0, isCloseOnly);
        }

        public static string GetOpenWindowStringWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText, int width, int height, bool isCloseOnly)
        {
            if (height > 590) height = 0;
            if (!pageUrl.Contains("?"))
            {
                pageUrl += "?";
            }
            if (string.IsNullOrEmpty(alertText))
            {
                return
                    $@"openWindow('{title}', '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}')),{width}, {height}, '{isCloseOnly
                        .ToString().ToLower()}');return false;";
            }
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxId}'), '{alertText}')){{openWindow('{title}', '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}')),{width}, {height}, '{isCloseOnly
                    .ToString().ToLower()}');}};return false;";
        }

        public static string GetOpenWindowStringWithTwoCheckBoxValue(string title, string pageUrl, string checkBoxId1, string checkBoxId2, string alertText, int width, int height)
        {
            return GetOpenWindowStringWithTwoCheckBoxValue(title, pageUrl, checkBoxId1, checkBoxId2, alertText, width, height, false);
        }

        public static string GetOpenWindowStringWithTwoCheckBoxValue(string title, string pageUrl, string checkBoxId1, string checkBoxId2, string alertText, int width, int height, bool isCloseOnly)
        {
            if (height > 590) height = 0;
            if (!pageUrl.Contains("?"))
            {
                pageUrl += "?";
            }
            return
                $@"var collectionValue1 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}'));var collectionValue2 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}'));if (collectionValue1.length == 0 && collectionValue2.length == 0){{alert('{alertText}');}}else{{openWindow('{title}', '{pageUrl}' + '&{checkBoxId1}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}')) + '&{checkBoxId2}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}')),{width}, {height}, '{isCloseOnly
                    .ToString().ToLower()}');}};return false;";
        }

        public static void SetCancelAttribute(IAttributeAccessor accessor)
        {
            accessor.SetAttribute("onclick", HidePopWin);
        }

        public static void CloseModalPage(Page page)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{HidePopWin}window.parent.location.reload(false);</script>");
            //page.Response.End();
        }

        public static void CloseModalPage(Page page, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{scripts}</script>");
            page.Response.Write($"<script>window.parent.location.reload(false);{HidePopWin}</script>");
            //page.Response.End();
        }

        public static void CloseModalPageAndRedirect(Page page, string redirectUrl)
        {
            page.Response.Clear();
            page.Response.Write($"<script>window.parent.location.href = '{redirectUrl}';{HidePopWin}</script>");
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public static void CloseModalPageAndRedirect(Page page, string redirectUrl, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{scripts}</script>");
            page.Response.Write($"<script>window.parent.location.href = '{redirectUrl}';{HidePopWin}</script>");
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public static void CloseModalPageWithoutRefresh(Page page)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{HidePopWin}</script>");
            //page.Response.End();
        }

        public static void CloseModalPageWithoutRefresh(Page page, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{scripts}</script>");
            page.Response.Write($"<script>{HidePopWin}</script>");
            //page.Response.End();
        }

        public static string GetOpenLayerString(string title, string pageUrl)
        {
            return GetOpenLayerString(title, pageUrl, 0, 0);
        }

        public static string GetOpenLayerString(string title, string pageUrl, int width, int height)
        {
            string areaWidth = $"'{width}px'";
            string areaHeight = $"'{height}px'";
            var offsetLeft = "''";
            var offsetRight = "''";
            if (width == 0)
            {
                areaWidth = "($(window).width() - 10) +'px'";
                offsetRight = "'0px'";
            }
            if (height == 0)
            {
                areaHeight = "($(window).height() - 10) +'px'";
                offsetLeft = "'0px'";
            }
            return
                $@"$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}'}}, area: [{areaWidth}, {areaHeight}], offset: [{offsetLeft}, {offsetRight}]}});return false;";
        }

        public static string GetOpenLayerStringWithTextBoxValue(string title, string pageUrl, string textBoxId)
        {
            return GetOpenLayerStringWithTextBoxValue(title, pageUrl, textBoxId, 0, 0);
        }

        public static string GetOpenLayerStringWithTextBoxValue(string title, string pageUrl, string textBoxId, int width, int height)
        {
            string areaWidth = $"'{width}px'";
            string areaHeight = $"'{height}px'";
            var offset = string.Empty;
            if (width == 0)
            {
                areaWidth = "($(window).width() - 10) +'px'";
                offset = "offset: ['0px','0px'],";
            }
            if (height == 0)
            {
                areaHeight = "($(window).height() - 10) +'px'";
                offset = "offset: ['0px','0px'],";
            }
            return
                $@"$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{textBoxId}=' + $('#{textBoxId}').val()}}, area: [{areaWidth}, {areaHeight}], {offset}}});return false;";
        }

        public static string GetOpenLayerStringWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText)
        {
            return GetOpenLayerStringWithCheckBoxValue(title, pageUrl, checkBoxId, alertText, 0, 0);
        }

        public static string GetOpenLayerStringWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText, int width, int height)
        {
            string areaWidth = $"'{width}px'";
            string areaHeight = $"'{height}px'";
            var offset = string.Empty;
            if (width == 0)
            {
                areaWidth = "($(window).width() - 10) +'px'";
                offset = "offset: ['0px','0px'],";
            }
            if (height == 0)
            {
                areaHeight = "($(window).height() - 10) +'px'";
                offset = "offset: ['0px','0px'],";
            }

            if (string.IsNullOrEmpty(alertText))
            {
                return
                    $@"$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}'))}}, area: [{areaWidth}, {areaHeight}], {offset}}});return false;";
            }
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxId}'), '{alertText}')){{$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}'))}}, area: [{areaWidth}, {areaHeight}], {offset}}});}};return false;";
        }

        public static string GetOpenLayerStringWithTwoCheckBoxValue(string title, string pageUrl, string checkBoxId1, string checkBoxId2, string alertText, int width, int height)
        {
            var offset = string.Empty;
            if (width == 0)
            {
                offset = "offset: ['0px','0px'],";
            }
            if (height == 0)
            {
                offset = "offset: ['0px','0px'],";
            }

            return
                $@"var collectionValue1 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}'));var collectionValue2 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}'));if (collectionValue1.length == 0 && collectionValue2.length == 0){{alert('{alertText}');}}else{{$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{checkBoxId1}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}')) + '&{checkBoxId2}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}'))}}, area: [{width}, {height}], {offset}}});}};return false;";
        }

        public static void ResponseScripts(Page page, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script language=\"javascript\">{scripts}</script>");
            //page.Response.End();
        }


        public static string GetRedirectStringWithCheckBoxValue(string redirectUrl, string checkBoxServerId, string checkBoxClientId, string emptyAlertText)
        {
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxClientId}'), '{emptyAlertText}')){{_goto('{redirectUrl}' + '&{checkBoxServerId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxClientId}')));}};return false;";
        }

        public static string GetRedirectStringWithCheckBoxValueAndAlert(string redirectUrl, string checkBoxServerId, string checkBoxClientId, string emptyAlertText, string alertText)
        {
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxClientId}'), '{emptyAlertText}')){{if (confirm('{alertText}'))_goto('{redirectUrl}' + '&{checkBoxServerId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxClientId}')));}};return false;";

        }

        public static string GetRedirectStringWithConfirm(string redirectUrl, string confirmString)
        {
            return $@"if (window.confirm('{confirmString}')){{window.location.href='{redirectUrl}';}};return false;";
        }

        public static string GetRedirectString(string redirectUrl)
        {
            return $@"window.location.href='{redirectUrl}';return false;";
        }
    }
}
