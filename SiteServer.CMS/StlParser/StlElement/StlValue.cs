using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.StlElement
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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var type = string.Empty;
            var formatString = string.Empty;
            string separator = null;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = StringUtils.Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;

		    foreach (var name in contextInfo.Attributes.AllKeys)
		    {
		        var value = contextInfo.Attributes[name];

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

            return ParseImpl(pageInfo, contextInfo, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            if (string.IsNullOrEmpty(type)) return string.Empty;

            var parsedContent = string.Empty;

            if (contextInfo.ContextType == EContextType.Each)
            {
                parsedContent = contextInfo.ItemContainer.EachItem.DataItem as string;
                return parsedContent;
            }

            if (type.ToLower().Equals(TypeSiteName.ToLower()))
            {
                parsedContent = pageInfo.SiteInfo.SiteName;
            }
            else if (type.ToLower().Equals(TypeSiteUrl.ToLower()))
            {
                parsedContent = pageInfo.SiteInfo.Additional.WebUrl;
            }
            else if (type.ToLower().Equals(TypeDate.ToLower()))
            {
                if (!pageInfo.BodyCodes.ContainsKey("datestring.js"))
                {
                    pageInfo.BodyCodes.Add("datestring.js", $@"<script charset=""{SiteFilesAssets.DateString.Charset}"" src=""{SiteFilesAssets.GetUrl(
                        pageInfo.ApiUrl, SiteFilesAssets.DateString.Js)}"" type=""text/javascript""></script>");
                }

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(false);</script>";
            }
            else if (type.ToLower().Equals(TypeDateOfTraditional.ToLower()))
            {
                if (!pageInfo.BodyCodes.ContainsKey("datestring"))
                {
                    pageInfo.BodyCodes.Add("datestring", $@"<script charset=""{SiteFilesAssets.DateString.Charset}"" src=""{SiteFilesAssets.GetUrl(
                        pageInfo.ApiUrl, SiteFilesAssets.DateString.Js)}"" type=""text/javascript""></script>");
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
                if (pageInfo.SiteInfo.Additional.GetString(type) != null)
                {
                    parsedContent = pageInfo.SiteInfo.Additional.GetString(type);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        var styleInfo = TableStyleManager.GetTableStyleInfo(DataProvider.SiteDao.TableName, type, TableStyleManager.GetRelatedIdentities(pageInfo.SiteId));

                        // 如果 styleInfo.TableStyleId <= 0，表示此字段已经被删除了，不需要再显示值了 ekun008
                        if (styleInfo.Id > 0)
                        {
                            if (isClearTags && InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image, InputType.File))
                            {
                                parsedContent = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, parsedContent, pageInfo.IsLocal);
                            }
                            else
                            {
                                parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, separator, pageInfo.SiteInfo, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerHtml, false);
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
