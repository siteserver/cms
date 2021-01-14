using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Datory.Tests.Utils
{
    public static class PageUtils
    {
        public const char SeparatorChar = '/';

        public const string UnClickableUrl = "javascript:;";

        public static string AddEndSlashToUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.EndsWith("/"))
            {
                url += "/";
            }

            return url;
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
                builder.Append($"&{key}={UrlEncode(queryString[key])}");
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

        public static string RemoveQueryString(string url)
        {
            if (url == null) return null;

            if (url.IndexOf("?", StringComparison.Ordinal) == -1 || url.EndsWith("?"))
            {
                return url;
            }

            return url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));
        }

        public static string GetIpAddress(IPAddress ipAddress)
        {
            if (ipAddress == null) return "127.0.0.1";
            return GetIpAddress(ipAddress.ToString());
        }

        public static string GetIpAddress(string remoteIpAddress)
        {
            var result = string.Empty;

            //取CDN用户真实IP的方法
            //当用户使用代理时，取到的是代理IP
            result = remoteIpAddress;
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

            if (string.IsNullOrEmpty(result) || result == "::1" || result == "127.0.0.1")
            {
                result = "localhost";
            }

            return result;
        }

        public static bool IsIpAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
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
        public static string UrlEncode(string url)
        {
            return WebUtility.UrlEncode(url);
        }

        public static string UrlDecode(string url)
        {
            return WebUtility.UrlDecode(url);
        }

        //public static string UrlEncode(string urlString)
        //{
        //    if (urlString == null || urlString == "$4")
        //    {
        //        return string.Empty;
        //    }

        //    var newValue = urlString.Replace("\"", "'");
        //    newValue = HttpUtility.UrlEncode(newValue);
        //    newValue = newValue.Replace("%2f", "/");
        //    return newValue;
        //}

        //public static string UrlEncode(string urlString, string encoding)
        //{
        //    if (urlString == null || urlString == "$4")
        //    {
        //        return string.Empty;
        //    }

        //    var newValue = urlString.Replace("\"", "'");
        //    newValue = HttpUtility.UrlEncode(newValue, Encoding.GetEncoding(encoding));
        //    newValue = newValue.Replace("%2f", "/");
        //    return newValue;
        //}

        //public static string UrlEncode(string urlString, ECharset charset)
        //{
        //    if (urlString == null || urlString == "$4")
        //    {
        //        return string.Empty;
        //    }

        //    var newValue = urlString.Replace("\"", "'");
        //    newValue = HttpUtility.UrlEncode(newValue, ECharsetUtils.GetEncoding(charset));
        //    newValue = newValue.Replace("%2f", "/");
        //    return newValue;
        //}

        //public static string UrlDecode(string urlString, string encoding)
        //{
        //    return HttpUtility.UrlDecode(urlString, Encoding.GetEncoding(encoding));
        //}

        //public static string UrlDecode(string urlString, ECharset charset)
        //{
        //    return HttpUtility.UrlDecode(urlString, ECharsetUtils.GetEncoding(charset));
        //}

        //public static string UrlDecode(string urlString)
        //{
        //    return HttpUtility.UrlDecode(urlString);
        //}

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
