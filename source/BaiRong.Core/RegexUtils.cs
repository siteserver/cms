using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
    public class RegexUtils
    {

        /*
         * 通用：.*?
         * 所有链接：<a\s*.*?href=(?:"(?<url>[^"]*)"|'(?<url>[^']*)'|(?<url>\S+)).*?>
         * */

        public static RegexOptions Options = ((RegexOptions.Singleline | RegexOptions.IgnoreCase) | RegexOptions.IgnorePatternWhitespace);

        public static List<string> GetImageSrcs(string baseUrl, string html)
        {
            const string regex = "(img|input)[^><]*\\s+src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetUrls(regex, html, baseUrl);
        }

        public static List<string> GetOriginalImageSrcs(string html)
        {
            const string regex = "(img|input)[^><]*\\s+src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        public static List<string> GetOriginalLinkHrefs(string html)
        {
            const string regex = "a[^><]*\\s+href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        public static List<string> GetFlashSrcs(string baseUrl, string html)
        {
            const string regex = "embed\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|param\\s+[^><]*value\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetUrls(regex, html, baseUrl);
        }

        public static List<string> GetOriginalFlashSrcs(string html)
        {
            const string regex = "embed\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|param\\s+[^><]*value\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        public static List<string> GetStyleImageUrls(string baseUrl, string html)
        {
            const string regex = "url\\((?<url>[^\\(\\)]*)\\)";
            var arraylist = GetUrls(regex, html, baseUrl);
            var list = new List<string>();
            foreach (var url in arraylist)
            {
                if (!list.Contains(url) && EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(url)))
                {
                    list.Add(url);
                }
            }
            return list;
        }

        public static List<string> GetOriginalStyleImageUrls(string html)
        {
            //background-image: url(../images/leftline.gif);
            const string regex = "url\\((?<url>[^\\(\\)]*)\\)";
            var arraylist = GetContents("url", regex, html);
            var list = new List<string>();
            foreach (var url in arraylist)
            {
                if (!list.Contains(url) && EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(url)))
                {
                    list.Add(url);
                }
            }
            return list;
        }

        public static List<string> GetBackgroundImageSrcs(string baseUrl, string html)
        {
            const string regex = "background\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetUrls(regex, html, baseUrl);
        }

        public static List<string> GetOriginalBackgroundImageSrcs(string html)
        {
            const string regex = "background\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        public static List<string> GetCssHrefs(string baseUrl, string html)
        {
            //string regex = "link\\s+[^><]*href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>\\S+))|@import\\s*url\\((?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>\\S+))\\)";
            const string regex = "link\\s+[^><]*href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|\\@import\\s*url\\s*\\(\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>.*?))\\s*\\)";
            return GetUrls(regex, html, baseUrl);
        }

        public static List<string> GetOriginalCssHrefs(string html)
        {
            const string regex = "link\\s+[^><]*href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|\\@import\\s*url\\s*\\(\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>.*?))\\s*\\)";
            return GetContents("url", regex, html);
        }

        public static List<string> GetScriptSrcs(string baseUrl, string html)
        {
            const string regex = "script\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetUrls(regex, html, baseUrl);
        }

        public static List<string> GetOriginalScriptSrcs(string html)
        {
            const string regex = "script\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        public static List<string> GetTagInnerContents(string tagName, string html)
        {
            string regex = $"<{tagName}\\s+[^><]*>\\s*(?<content>[\\s\\S]+?)\\s*</{tagName}>";
            return GetContents("content", regex, html);
        }

        public static List<string> GetTagContents(string tagName, string html)
        {
            var list = new List<string>();

            string regex = $@"<({tagName})[^>]*>(.*?)</\1>|<{tagName}[^><]*/>";

            var matches = Regex.Matches(html, regex, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    list.Add(match.Result("$0"));
                }
            }

            return list;
        }

        public static string GetTagName(string html)
        {
            var match = Regex.Match(html, "<([^>\\s]+)[\\s\\SS]*>", RegexOptions.IgnoreCase);
            return match.Success ? match.Result("$1") : string.Empty;
        }

        public static string GetInnerContent(string tagName, string html)
        {
            string regex = $"<{tagName}[^><]*>(?<content>[\\s\\S]+?)</{tagName}>";
            return GetContent("content", regex, html);
        }

        public static string GetAttributeContent(string attributeName, string html)
        {
            string regex =
                $"<[^><]+\\s*{attributeName}\\s*=\\s*(?:\"(?<value>[^\"]*)\"|'(?<value>[^']*)'|(?<value>[^>\\s]*)).*?>";
            return GetContent("value", regex, html);
        }

        public static List<string> GetUrls(string html, string baseUrl)
        {
            const string regex = "<a\\s*.*?href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*)).*?>";
            return GetUrls(regex, html, baseUrl);
        }

        public static List<string> GetUrls(string regex, string html, string baseUrl)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(regex))
            {
                regex = "<a\\s*.*?href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*)).*?>";
            }
            var groupName = "url";
            var arraylist = GetContents(groupName, regex, html);
            foreach (var rawUrl in arraylist)
            {
                var url = PageUtils.GetUrlByBaseUrl(rawUrl, baseUrl);
                if (!string.IsNullOrEmpty(url) && !list.Contains(url))
                {
                    list.Add(url);
                }
            }
            return list;
        }

        public static string GetUrl(string regex, string html, string baseUrl)
        {
            return PageUtils.GetUrlByBaseUrl(GetContent("url", regex, html), baseUrl);
        }

        public static string GetContent(string groupName, string regex, string html)
        {
            var content = string.Empty;
            if (string.IsNullOrEmpty(regex)) return content;
            if (regex.IndexOf("<" + groupName + ">", StringComparison.Ordinal) == -1)
            {
                return regex;
            }

            var reg = new Regex(regex, Options);
            var match = reg.Match(html);
            if (match.Success)
            {
                content = match.Groups[groupName].Value;
            }

            return content;
        }

        public static string Replace(string regex, string input, string replacement)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var reg = new Regex(regex, Options);
            return reg.Replace(input, replacement);
        }

        public static string Replace(string regex, string input, string replacement, int count)
        {
            if (count == 0)
            {
                return Replace(regex, input, replacement);
            }
            if (string.IsNullOrEmpty(input)) return input;
            var reg = new Regex(regex, Options);
            return reg.Replace(input, replacement, count);
        }

        public static bool IsMatch(string regex, string input)
        {
            var reg = new Regex(regex, Options);
            return reg.IsMatch(input);
        }

        public static List<string> GetContents(string groupName, string regex, string html)
        {
            if (string.IsNullOrEmpty(regex)) return new List<string>();

            var list = new List<string>();
            var reg = new Regex(regex, Options);

            for (var match = reg.Match(html); match.Success; match = match.NextMatch())
            {
                var theValue = match.Groups[groupName].Value;
                if (!list.Contains(theValue))
                {
                    list.Add(theValue);
                }
            }
            return list;
        }

        public static string RemoveScripts(string html)
        {
            const string regex = "<script[^><]*>.*?<\\/script>";
            return Replace(regex, html, string.Empty);
        }

        public static string RemoveImages(string html)
        {
            const string regex = "<img[^><]*>";
            return Replace(regex, html, string.Empty);
        }
    }
}
