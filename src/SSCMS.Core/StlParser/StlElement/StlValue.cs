using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取值", Description = "通过 stl:value 标签在模板中获取值")]
    public class StlValue
	{
		private StlValue(){}
		public const string ElementName = "stl:value";

		[StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);
        
        [StlAttribute(Title = "显示的格式")]
        private const string FormatString = nameof(FormatString);
        
        [StlAttribute(Title = "显示多项时的分割字符串")]
        private const string Separator = nameof(Separator);
        
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

        public const string TypeSiteName = "SiteName";
		public const string TypeSiteUrl = "SiteUrl";
        public const string TypeDate = "Date";
        public const string TypeDateOfTraditional = "DateOfTraditional";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeSiteName, "站点名称"},
            {TypeSiteUrl, "站点的域名地址"},
            {TypeDate, "当前日期"},
            {TypeDateOfTraditional, "带农历的当前日期"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var type = string.Empty;
            var formatString = string.Empty;
            string separator = null;
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
                else if (StringUtils.EqualsIgnoreCase(name, Separator))
                {
                    separator = value;
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

            return await ParseImplAsync(parseManager, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
		}

        private static async Task<string> ParseImplAsync(IParseManager parseManager, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (string.IsNullOrEmpty(type)) return string.Empty;

            var parsedContent = string.Empty;

            if (contextInfo.ContextType == ParseType.Each)
            {
                parsedContent = contextInfo.ItemContainer.EachItem.Value as string;
                return parsedContent;
            }

            if (StringUtils.EqualsIgnoreCase(type, TypeSiteName))
            {
                parsedContent = pageInfo.Site.SiteName;
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeSiteUrl))
            {
                parsedContent = await parseManager.PathManager.GetWebUrlAsync(pageInfo.Site);
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeDate))
            {
                if (!pageInfo.BodyCodes.ContainsKey("datestring.js"))
                {
                    var jsUrl = parseManager.PathManager.GetSiteFilesUrl(Resources.DateString.Js);

                    pageInfo.BodyCodes.Add("datestring.js", $@"<script charset=""{Resources.DateString.Charset}"" src=""{jsUrl}"" type=""text/javascript""></script>");
                }

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(false);</script>";
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeDateOfTraditional))
            {
                if (!pageInfo.BodyCodes.ContainsKey("datestring"))
                {
                    var jsUrl = parseManager.PathManager.GetSiteFilesUrl(Resources.DateString.Js);

                    pageInfo.BodyCodes.Add("datestring", $@"<script charset=""{Resources.DateString.Charset}"" src=""{jsUrl}"" type=""text/javascript""></script>");
                }

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(true);</script>";
            }
            else if (pageInfo.Parameters != null && pageInfo.Parameters.ContainsKey(type))
            {
                pageInfo.Parameters.TryGetValue(type, out parsedContent);
                parsedContent = InputTypeUtils.ParseString(InputType.Text, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            }
            else
            {
                if (pageInfo.Site.Get<string>(type) != null)
                {
                    parsedContent = pageInfo.Site.Get<string>(type);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(databaseManager.SiteRepository.TableName, type, databaseManager.TableStyleRepository.GetRelatedIdentities(pageInfo.SiteId));

                        // 如果 styleInfo.TableStyleId <= 0，表示此字段已经被删除了，不需要再显示值了 ekun008
                        if (styleInfo.Id > 0)
                        {
                            if (isClearTags && InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image, InputType.File))
                            {
                                parsedContent = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                            }
                            else
                            {
                                var inputParser = new InputParserManager(parseManager.PathManager);

                                parsedContent = await inputParser.GetContentByTableStyleAsync(parsedContent, separator, pageInfo.Site, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerHtml, false);
                                parsedContent = InputTypeUtils.ParseString(styleInfo.InputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                            }
                        }
                        else
                        { // 如果字段已经被删除或不再显示了，则此字段的值为空。有时虚拟字段值不会清空
                            parsedContent = string.Empty; 
                        }
                    }
                }
            }
            return parsedContent;
        }
	}
}
