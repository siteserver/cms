using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Http;
using SSCMS.Configuration;

namespace SSCMS.Utils
{
    public static class PageUtils
    {
        public const char SeparatorChar = '/';
        public const string DoubleSeparator = "//";
        public const string Separator = "/";
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

        public static string GetHost(HttpRequest request)
        {
            var host = string.Empty;
            if (request == null) return string.IsNullOrEmpty(host) ? string.Empty : host.Trim().ToLower();
            host = request.Headers["HOST"];
            if (string.IsNullOrEmpty(host))
            {
                host = request.Host.Host;
            }

            return string.IsNullOrEmpty(host) ? string.Empty : host.Trim().ToLower();
        }

        public static NameValueCollection GetQueryString(string url)
        {
            if (string.IsNullOrEmpty(url) || url.IndexOf("?", StringComparison.Ordinal) == -1) return new NameValueCollection();

            var querystring = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
            return TranslateUtils.ToNameValueCollection(querystring);
        }

        public static NameValueCollection GetQueryStringFilterSqlAndXss(string url)
        {
            if (string.IsNullOrEmpty(url) || url.IndexOf("?", StringComparison.Ordinal) == -1) return new NameValueCollection();

            var attributes = new NameValueCollection();

            var querystring = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
            var originals = TranslateUtils.ToNameValueCollection(querystring);
            foreach (string key in originals.Keys)
            {
                attributes[key] = AttackUtils.FilterSqlAndXss(originals[key]);
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

        public static string GetIpAddress(HttpRequest request)
        {
            var result = string.Empty;

            try
            {
                result = request.Headers["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    if (result.IndexOf(".", StringComparison.Ordinal) == -1)
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            result = result.Replace("  ", "").Replace("'", "");
                            var temporary = result.Split(",;".ToCharArray());
                            foreach (var t in temporary)
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
                {
                    result = request.Headers["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = request.HttpContext.Connection.RemoteIpAddress.ToString();
                }

                if (string.IsNullOrEmpty(result) || result == "::1")
                {
                    result = "127.0.0.1";
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }

        public static bool IsAllowed(string ipAddress, List<string> blockList, List<string> allowList)
        {
            var isAllowed = true;

            if (blockList != null && blockList.Count > 0)
            {
                var list = new IpList();
                foreach (var restriction in blockList)
                {
                    AddRestrictionToIpList(list, restriction);
                }
                if (list.CheckNumber(ipAddress))
                {
                    isAllowed = false;
                }
            }
            else if (allowList != null && allowList.Count > 0)
            {
                isAllowed = false;
                var list = new IpList();
                foreach (var restriction in allowList)
                {
                    AddRestrictionToIpList(list, restriction);
                }
                if (list.CheckNumber(ipAddress))
                {
                    isAllowed = true;
                }
            }

            return isAllowed;
        }

        private static void AddRestrictionToIpList(IpList list, string restriction)
        {
            if (string.IsNullOrEmpty(restriction)) return;

            if (StringUtils.Contains(restriction, "-"))
            {
                restriction = restriction.Trim(' ', '-');
                var arr = restriction.Split('-');
                list.AddRange(arr[0].Trim(), arr[1].Trim());
            }
            else if (StringUtils.Contains(restriction, "*"))
            {
                var ipPrefix = restriction.Substring(0, restriction.IndexOf('*'));
                ipPrefix = ipPrefix.Trim(' ', '.');
                var dotNum = StringUtils.GetCount(".", ipPrefix);

                string ipNumber;
                string mask;
                if (dotNum == 0)
                {
                    ipNumber = ipPrefix + ".0.0.0";
                    mask = "255.0.0.0";
                }
                else if (dotNum == 1)
                {
                    ipNumber = ipPrefix + ".0.0";
                    mask = "255.255.0.0";
                }
                else
                {
                    ipNumber = ipPrefix + ".0";
                    mask = "255.255.255.0";
                }
                list.Add(ipNumber, mask);
            }
            else
            {
                list.Add(restriction);
            }
        }

        public static string GetLocalApiUrl(params string[] paths)
        {
            return Combine(Constants.ApiPrefix, Combine(paths));
        }

        private class IpList
        {
            private readonly List<IpRangeList> _ipRangeList = new List<IpRangeList>();
            private readonly SortedList _maskList = new SortedList();
            private readonly List<int> _usedList = new List<int>();

            public IpList()
            {
                // Initialize IP mask list and create IPArrayList into the ipRangeList
                uint mask = 0x00000000;
                for (var level = 1; level < 33; level++)
                {
                    mask = (mask >> 1) | 0x80000000;
                    _maskList.Add(mask, level);
                    _ipRangeList.Add(new IpRangeList(mask));
                }
            }

            // Parse a String IP address to a 32 bit unsigned integer
            // We can't use System.Net.IPAddress as it will not parse
            // our masks correctly eg. 255.255.0.0 is pased as 65535 !
            private uint parseIP(string ipNumber)
            {
                uint res = 0;
                var elements = ipNumber.Split('.');
                if (elements.Length == 4)
                {
                    res = (uint)Convert.ToInt32(elements[0]) << 24;
                    res += (uint)Convert.ToInt32(elements[1]) << 16;
                    res += (uint)Convert.ToInt32(elements[2]) << 8;
                    res += (uint)Convert.ToInt32(elements[3]);
                }

                return res;
            }

            /// <summary>
            /// Add a single IP number to the list as a string, ex. 10.1.1.1
            /// </summary>
            public void Add(string ipNumber)
            {
                Add(parseIP(ipNumber));
            }

            /// <summary>
            /// Add a single IP number to the list as a unsigned integer, ex. 0x0A010101
            /// </summary>
            private void Add(uint ip)
            {
                _ipRangeList[31].Add(ip);
                if (_usedList.Contains(31)) return;
                _usedList.Add(31);
                _usedList.Sort();
            }

            /// <summary>
            /// Adds IP numbers using a mask for range where the mask specifies the number of
            /// fixed bits, ex. 172.16.0.0 255.255.0.0 will add 172.16.0.0 - 172.16.255.255
            /// </summary>
            public void Add(string ipNumber, string mask)
            {
                Add(parseIP(ipNumber), parseIP(mask));
            }

            /// <summary>
            /// Adds IP numbers using a mask for range where the mask specifies the number of
            /// fixed bits, ex. 0xAC1000 0xFFFF0000 will add 172.16.0.0 - 172.16.255.255
            /// </summary>
            public void Add(uint ip, uint uMask)
            {
                var level = _maskList[uMask];
                if (level == null) return;
                ip = ip & uMask;
                _ipRangeList[(int)level - 1].Add(ip);
                if (_usedList.Contains((int)level - 1)) return;
                _usedList.Add((int)level - 1);
                _usedList.Sort();
            }

            /// <summary>
            /// Adds IP numbers using a mask for range where the mask specifies the number of
            /// fixed bits, ex. 192.168.1.0/24 which will add 192.168.1.0 - 192.168.1.255
            /// </summary>
            public void Add(string ipNumber, int maskLevel)
            {
                Add(parseIP(ipNumber), (uint)_maskList.GetKey(_maskList.IndexOfValue(maskLevel)));
            }

            /// <summary>
            /// Adds IP numbers using a from and to IP number. The method checks the range and
            /// splits it into normal ip/mask blocks.
            /// </summary>
            public void AddRange(string fromIp, string toIp)
            {
                AddRange(parseIP(fromIp), parseIP(toIp));
            }

            /// <summary>
            /// Adds IP numbers using a from and to IP number. The method checks the range and
            /// splits it into normal ip/mask blocks.
            /// </summary>
            private void AddRange(uint fromIp, uint toIp)
            {
                // If the order is not asending, switch the IP numbers.
                if (fromIp > toIp)
                {
                    var tempIp = fromIp;
                    fromIp = toIp;
                    toIp = tempIp;
                }

                if (fromIp == toIp)
                {
                    Add(fromIp);
                }
                else
                {
                    var diff = toIp - fromIp;
                    var diffLevel = 1;
                    var range = 0x80000000;
                    if (diff < 256)
                    {
                        diffLevel = 24;
                        range = 0x00000100;
                    }

                    while (range > diff)
                    {
                        range = range >> 1;
                        diffLevel++;
                    }

                    var mask = (uint)_maskList.GetKey(_maskList.IndexOfValue(diffLevel));
                    var minIp = fromIp & mask;
                    if (minIp < fromIp) minIp += range;
                    if (minIp > fromIp)
                    {
                        AddRange(fromIp, minIp - 1);
                        fromIp = minIp;
                    }

                    if (fromIp == toIp)
                    {
                        Add(fromIp);
                    }
                    else
                    {
                        if ((minIp + (range - 1)) <= toIp)
                        {
                            Add(minIp, mask);
                            fromIp = minIp + range;
                        }

                        if (fromIp == toIp)
                        {
                            Add(toIp);
                        }
                        else
                        {
                            if (fromIp < toIp) AddRange(fromIp, toIp);
                        }
                    }
                }
            }

            /// <summary>
            /// Checks if an IP number is contained in the lists, ex. 10.0.0.1
            /// </summary>
            public bool CheckNumber(string ipNumber)
            {
                return CheckNumber(parseIP(ipNumber));
            }

            /// <summary>
            /// Checks if an IP number is contained in the lists, ex. 0x0A000001
            /// </summary>
            private bool CheckNumber(uint ip)
            {
                var found = false;
                var i = 0;
                while (!found && i < _usedList.Count)
                {
                    found = _ipRangeList[_usedList[i]].Check(ip);
                    i++;
                }

                return found;
            }

            /// <summary>
            /// Clears all lists of IP numbers
            /// </summary>
            public void Clear()
            {
                foreach (var i in _usedList)
                {
                    _ipRangeList[i].Clear();
                }

                _usedList.Clear();
            }

            /// <summary>
            /// Generates a list of all IP ranges in printable format
            /// </summary>
            public override string ToString()
            {
                var buffer = new StringBuilder();
                foreach (var i in _usedList)
                {
                    buffer.Append("\r\nRange with mask of ").Append(i + 1).Append("\r\n");
                    buffer.Append(_ipRangeList[i]);
                }

                return buffer.ToString();
            }
        }

        private class IpRangeList
        {
            private bool _isSorted;
            private readonly List<uint> _ipNumList = new List<uint>();
            private readonly uint _ipMask;

            /// <summary>
            /// Constructor that sets the mask for the list
            /// </summary>
            public IpRangeList(uint mask)
            {
                _ipMask = mask;
            }

            /// <summary>
            /// Add a new IP numer (range) to the list
            /// </summary>
            public void Add(uint ipNum)
            {
                _isSorted = false;
                _ipNumList.Add(ipNum & _ipMask);
            }

            /// <summary>
            /// Checks if an IP number is within the ranges included by the list
            /// </summary>
            public bool Check(uint ipNum)
            {
                var found = false;
                if (_ipNumList.Count > 0)
                {
                    if (!_isSorted)
                    {
                        _ipNumList.Sort();
                        _isSorted = true;
                    }

                    ipNum = ipNum & _ipMask;
                    if (_ipNumList.BinarySearch(ipNum) >= 0) found = true;
                }

                return found;
            }

            /// <summary>
            /// Clears the list
            /// </summary>
            public void Clear()
            {
                _ipNumList.Clear();
                _isSorted = false;
            }

            /// <summary>
            /// The ToString is overriden to generate a list of the IP numbers
            /// </summary>
            public override string ToString()
            {
                var buf = new StringBuilder();
                foreach (uint ipNum in _ipNumList)
                {
                    if (buf.Length > 0) buf.Append("\r\n");
                    buf.Append(((int)ipNum & 0xFF000000) >> 24).Append('.');
                    buf.Append(((int)ipNum & 0x00FF0000) >> 16).Append('.');
                    buf.Append(((int)ipNum & 0x0000FF00) >> 8).Append('.');
                    buf.Append(((int)ipNum & 0x000000FF));
                }

                return buf.ToString();
            }
        }
    }
}
