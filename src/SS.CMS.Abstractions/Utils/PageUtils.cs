using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SS.CMS.Abstractions
{
    public static class PageUtils
    {
        public const char SeparatorChar = '/';
        public const string UnClickableUrl = "javascript:;";

        public static string AddProtocolToUrl(string url)
        {
            return AddProtocolToUrl(url, string.Empty);
        }

        public static string AddProtocolToUrl(string url, string host)
        {
            if (url == UnClickableUrl)
            {
                return url;
            }
            var retVal = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                if (IsProtocolUrl(url))
                {
                    retVal = url;
                }
                else
                {
                    retVal = url.StartsWith("/") ? host.TrimEnd('/') + url : host + url;
                }
            }
            return retVal;
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

        

        //public static string HttpContextRootDomain
        //{
        //    get
        //    {
        //        var url = HttpContext.Current.Request.Url;

        //        if (url.HostNameType != UriHostNameType.Dns) return url.Host;

        //        var match = Regex.Match(url.Host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
        //        return match.Groups[1].Success ? match.Groups[1].Value : null;
        //    }
        //}

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

            var retVal = urls[0]?.Replace(PathUtils.SeparatorChar, SeparatorChar) ?? string.Empty;
            for (var i = 1; i < urls.Length; i++)
            {
                var url = (urls[i] != null) ? urls[i].Replace(PathUtils.SeparatorChar, SeparatorChar) : string.Empty;
                retVal = Combine(retVal, url);
            }
            return retVal;
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
    }
}
