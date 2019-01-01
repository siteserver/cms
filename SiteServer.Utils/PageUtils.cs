using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SiteServer.Utils.Enumerations;

namespace SiteServer.Utils
{
    public static class PageUtils
    {
        public const char SeparatorChar = '/';

        public const string UnclickedUrl = "javascript:;";

        public static string ParseNavigationUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            url = url.StartsWith("~") ? Combine(ApplicationPath, url.Substring(1)) : url;
            url = url.Replace(PathUtils.SeparatorChar, SeparatorChar);
            return url;
        }

        public static string AddEndSlashToUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.EndsWith("/"))
            {
                url += "/";
            }

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

        public static bool IsAbsoluteUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            url = url.Trim();
            return url.StartsWith("/") || url.IndexOf("://", StringComparison.Ordinal) != -1 || url.StartsWith("javascript:");
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

            return string.IsNullOrEmpty(scheme) ? "http" : scheme.Trim().ToLower();
        }

        public static string ApplicationPath => HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath) ? HttpContext.Current.Request.ApplicationPath : "/";

        // 系统根目录访问地址
        public static string GetRootUrl(string relatedUrl)
        {
            return Combine(ApplicationPath, relatedUrl);
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
                attributes[key] = AttackUtils.FilterXss(originals[key]);
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

            return url1.TrimEnd(SeparatorChar) + SeparatorChar + url2.TrimStart(SeparatorChar);
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

        public static string AddQueryStringIfNotExists(string url, NameValueCollection queryString)
        {
            if (queryString == null || url == null || queryString.Count == 0)
                return url;

            var index = url.IndexOf("?", StringComparison.Ordinal);
            if (index != -1)
            {
                var query = TranslateUtils.ToNameValueCollection(url.Substring(index).Trim('?', '&'), '&');

                foreach (string key in query.Keys)
                {
                    if (queryString[key] != null)
                    {
                        queryString.Remove(key);
                    }
                }
            }

            return AddQueryString(url, queryString);
        }

        public static string RemoveQueryString(string url)
        {
            if (url == null) return null;

            if (url.IndexOf("?", StringComparison.Ordinal) == -1 || url.EndsWith("?"))
            {
                return url;
            }
            
            return url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));
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

        public static string RemoveQueryString(string url, List<string> queryNames)
        {
            if (queryNames == null || queryNames.Count == 0 || url == null) return url;

            if (url.IndexOf("?", StringComparison.Ordinal) == -1 || url.EndsWith("?"))
            {
                return url;
            }
            var attributes = GetQueryString(url);
            foreach (var queryName in queryNames)
            {
                attributes.Remove(queryName);
            }
            url = url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));
            return AddQueryString(url, attributes);
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
                                if (IsIpAddress(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                                {
                                    result = t;
                                }
                            }
                            var str = result.Split(',');
                            if (str.Length > 0)
                                result = str[0].Trim();
                        }
                        else if (IsIpAddress(result))
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

        public static bool IsIpAddress(string ip)
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
            newValue = newValue.Replace("%2f", "/");
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
            newValue = newValue.Replace("%2f", "/");
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
            newValue = newValue.Replace("%2f", "/");
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
            return HttpUtility.UrlDecode(urlString);
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

        public static string GetMainUrl(int siteId)
        {
            return GetAdminUrl($"main.cshtml?siteId={siteId}");
        }

        public static string GetAdminUrl(string relatedUrl)
        {
            return Combine(ApplicationPath, WebConfigUtils.AdminDirectory, relatedUrl);
        }

        public static string GetHomeUrl(string relatedUrl)
        {
            return Combine(ApplicationPath, WebConfigUtils.HomeDirectory, relatedUrl);
        }

        public static string GetSiteFilesUrl(string relatedUrl)
        {
            return Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, relatedUrl);
        }

        public static string GetTemporaryFilesUrl(string relatedUrl)
        {
            return Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedUrl);
        }

        public static string GetSiteTemplatesUrl(string relatedUrl)
        {
            return Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedUrl);
        }

        public static string GetSiteTemplateMetadataUrl(string siteTemplateUrl, string relatedUrl)
        {
            return Combine(siteTemplateUrl, DirectoryUtils.SiteTemplates.SiteTemplateMetadata, relatedUrl);
        }

        public static string ParsePluginUrl(string pluginId, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (IsProtocolUrl(url)) return url;

            if (StringUtils.StartsWith(url, "~/"))
            {
                return GetRootUrl(url.Substring(1));
            }

            if (StringUtils.StartsWith(url, "@/"))
            {
                return GetAdminUrl(url.Substring(1));
            }

            return GetSiteFilesUrl(Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, url));
        }

        public static string GetSiteServerUrl(string className)
        {
            return GetAdminUrl(className.ToCamelCase() + ".cshtml");
        }

        public static string GetSiteServerUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminUrl(className.ToCamelCase() + ".aspx"), queryString);
        }

        public static string GetPluginsUrl(string className)
        {
            return GetAdminUrl(Combine("plugins", className.ToCamelCase() + ".cshtml"));
        }

        public static string GetPluginsUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminUrl(Combine("plugins", className.ToCamelCase() + ".aspx")), queryString);
        }

        public static string GetSettingsUrl(string className)
        {
            return GetAdminUrl(Combine("settings", className.ToCamelCase() + ".cshtml"));
        }

        public static string GetSettingsUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminUrl(Combine("settings", className.ToCamelCase() + ".aspx")), queryString);
        }

        public static string GetCmsUrl(string pageName, int siteId, object param = null)
        {
            var url = GetAdminUrl(Combine("cms", $"{pageName.ToCamelCase()}.cshtml?siteId={siteId}"));
            return param == null ? url : param.GetType().GetProperties().Aggregate(url, (current, p) => current + $"&{p.Name.ToCamelCase()}={p.GetValue(param)}");
        }

        public static string GetCmsUrl(int siteId, string className, NameValueCollection queryString)
        {
            queryString = queryString ?? new NameValueCollection();
            queryString.Remove("siteId");
            return AddQueryString(GetAdminUrl($"cms/{className.ToCamelCase()}.aspx?siteId={siteId}"), queryString);
        }

        public static string GetCmsWebHandlerUrl(int siteId, string className, NameValueCollection queryString)
        {
            queryString = queryString ?? new NameValueCollection();
            queryString.Remove("siteId");
            return AddQueryString(GetAdminUrl($"cms/{className.ToCamelCase()}.ashx?siteId={siteId}"), queryString);
        }

        public static string GetAjaxUrl(string className, NameValueCollection queryString)
        {
            return AddQueryString(GetAdminUrl(Combine("ajax", className.ToLower() + ".aspx")), queryString);
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
            requestPath = requestPath.Replace(PathUtils.SeparatorChar, SeparatorChar);
            return GetRootUrl(requestPath);
        }

        public static string ParseConfigRootUrl(string url)
        {
            return ParseNavigationUrl(url);
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
            return GetAdminUrl($"loading.aspx?redirectUrl={TranslateUtils.EncryptStringBySecretKey(url)}");
        }

        public static string GetRedirectStringWithCheckBoxValue(string redirectUrl, string checkBoxServerId, string checkBoxClientId, string emptyAlertText)
        {
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxClientId}'), '{emptyAlertText}')){{_goto('{redirectUrl}' + '&{checkBoxServerId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxClientId}')));}};return false;";
        }

        public static string GetRedirectStringWithCheckBoxValueAndAlert(string redirectUrl, string checkBoxServerId, string checkBoxClientId, string emptyAlertText, string confirmAlertText)
        {
            return
                $@"_confirmCheckBoxCollection(document.getElementsByName('{checkBoxClientId}'), '{emptyAlertText}', '{confirmAlertText}', '{redirectUrl}' + '&{checkBoxServerId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxClientId}')));return false;";
        }

        public static string GetRedirectStringWithConfirm(string redirectUrl, string confirmString)
        {
            return $@"_confirm('{confirmString}', '{redirectUrl}');return false;";
        }
    }
}
