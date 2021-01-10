using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取值", Description = "通过 stl:value 标签在模板中获取值")]
    public static class StlValue
	{
        public const string ElementName = "stl:value";

		[StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);
        
        [StlAttribute(Title = "显示的格式")]
        private const string FormatString = nameof(FormatString);

        [StlAttribute(Title = "字符开始位置")]
        private const string StartIndex = nameof(StartIndex);
        
        [StlAttribute(Title = "指定字符长度")]
        private const string Length = nameof(Length);
        
        [StlAttribute(Title = "显示字符的数目")]
        private const string WordNum = nameof(WordNum);
        
        [StlAttribute(Title = "文字超出部分显示的文字")]
        private const string Ellipsis = nameof(Ellipsis);
        
        [StlAttribute(Title = "需要替换的文字，可以是正则表达式")]
        private const string Replace = nameof(Replace);
        
        [StlAttribute(Title = "替换replace的文字信息")]
        private const string To = nameof(To);
        
        [StlAttribute(Title = "是否清除标签信息")]
        private const string IsClearTags = nameof(IsClearTags);
        
        [StlAttribute(Title = "是否将回车替换为HTML换行标签")]
        private const string IsReturnToBr = nameof(IsReturnToBr);
        
        [StlAttribute(Title = "是否转换为小写")]
        private const string IsLower = nameof(IsLower);
        
        [StlAttribute(Title = "是否转换为大写")]
        private const string IsUpper = nameof(IsUpper);

        public const string TypeDate = "Date";
        public const string TypeDateOfTraditional = "DateOfTraditional";
        public static string TypeSiteId = "SiteId";
        public static string TypeSiteDir = "SiteDir";
        public static string TypeSiteName = "SiteName";
        public static string TypeSiteUrl = "SiteUrl";
        public static string TypeRootUrl = "RootUrl";
        public static string TypeApiUrl = "ApiUrl";
        public static string TypeCurrentUrl = "CurrentUrl";
        public static string TypeChannelUrl = "ChannelUrl";
        public static string TypeHomeUrl = "HomeUrl";
        public static string TypeLoginUrl = "LoginUrl";
        public static string TypeRegisterUrl = "RegisterUrl";
        public static string TypeLogoutUrl = "LogoutUrl";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeDate, "当前日期"},
            {TypeDateOfTraditional, "带农历的当前日期"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var type = string.Empty;
            var formatString = string.Empty;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;

		    foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
		    {
		        var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FormatString))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Length))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Ellipsis))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Replace))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, To))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsReturnToBr))
                {
                    isReturnToBr = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
            }

            return await ParseAsync(parseManager, type, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
		}

        private static async Task<object> ParseAsync(IParseManager parseManager, string type, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (string.IsNullOrEmpty(type)) return string.Empty;

            var parsedContent = string.Empty;

            if (contextInfo.ContextType == ParseType.Each)
            {
                parsedContent = contextInfo.ItemContainer.EachItem.Value as string;
                return parsedContent;
            }

            if (StringUtils.EqualsIgnoreCase(type, TypeDate))
            {
                if (!pageInfo.BodyCodes.ContainsKey("datestring.js"))
                {
                    var jsUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.DateString.Js);

                    pageInfo.BodyCodes.Add("datestring.js", $@"<script charset=""{Resources.DateString.Charset}"" src=""{jsUrl}"" type=""text/javascript""></script>");
                }

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(false);</script>";
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeDateOfTraditional))
            {
                if (!pageInfo.BodyCodes.ContainsKey("datestring"))
                {
                    var jsUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.DateString.Js);

                    pageInfo.BodyCodes.Add("datestring", $@"<script charset=""{Resources.DateString.Charset}"" src=""{jsUrl}"" type=""text/javascript""></script>");
                }

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(true);</script>";
            }
            else if (StringUtils.EqualsIgnoreCase(TypeRootUrl, type))//系统根目录地址
            {
                parsedContent = parseManager.PathManager.ParseUrl("~");
                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = parsedContent.TrimEnd('/');
                }
            }
            else if (StringUtils.EqualsIgnoreCase(TypeApiUrl, type))//API地址
            {
                parsedContent = parseManager.PathManager.GetRootUrl();
            }
            else if (StringUtils.EqualsIgnoreCase(TypeSiteId, type))//ID
            {
                parsedContent = pageInfo.SiteId.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(TypeSiteName, type))//名称
            {
                parsedContent = pageInfo.Site.SiteName;
            }
            else if (StringUtils.EqualsIgnoreCase(TypeSiteUrl, type))//域名地址
            {
                parsedContent = (await parseManager.PathManager.GetSiteUrlAsync(pageInfo.Site, pageInfo.IsLocal)).TrimEnd('/');
            }
            else if (StringUtils.EqualsIgnoreCase(TypeSiteDir, type))//文件夹
            {
                parsedContent = pageInfo.Site.SiteDir;
            }
            else if (StringUtils.EqualsIgnoreCase(TypeCurrentUrl, type))//当前页地址
            {
                var contentInfo = await parseManager.GetContentAsync();
                parsedContent = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
            }
            else if (StringUtils.EqualsIgnoreCase(TypeChannelUrl, type))//栏目页地址
            {
                parsedContent = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, await parseManager.DatabaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId), pageInfo.IsLocal);
            }
            else if (StringUtils.EqualsIgnoreCase(TypeHomeUrl, type))//用户中心地址
            {
                parsedContent = parseManager.PathManager.GetHomeUrl(string.Empty).TrimEnd('/');
            }
            else if (StringUtils.EqualsIgnoreCase(TypeLoginUrl, type))
            {
                var contentInfo = await parseManager.GetContentAsync();
                var returnUrl = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                parsedContent = parseManager.PathManager.GetHomeUrl($"pages/login.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
            }
            else if (StringUtils.EqualsIgnoreCase(TypeLogoutUrl, type))
            {
                var contentInfo = await parseManager.GetContentAsync();
                var returnUrl = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                parsedContent = parseManager.PathManager.GetHomeUrl($"pages/logout.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
            }
            else if (StringUtils.EqualsIgnoreCase(TypeRegisterUrl, type))
            {
                var contentInfo = await parseManager.GetContentAsync();
                var returnUrl = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                parsedContent = parseManager.PathManager.GetHomeUrl($"pages/register.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
            }
            else if (StringUtils.StartsWithIgnoreCase(type, "TableFor"))//
            {
                if (StringUtils.EqualsIgnoreCase(type, "TableForContent"))
                {
                    parsedContent = pageInfo.Site.TableName;
                }
            }
            else if (StringUtils.StartsWithIgnoreCase(type, "Site"))//
            {
                parsedContent = pageInfo.Site.Get<string>(type.Substring(4));
            }
            else if (pageInfo.Parameters != null && pageInfo.Parameters.ContainsKey(type))
            {
                pageInfo.Parameters.TryGetValue(type, out parsedContent);
                parsedContent = InputTypeUtils.ParseString(InputType.Text, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            }
            else
            {
                return await StlSite.ParseAsync(parseManager);
            }
            return parsedContent;
        }
	}
}
