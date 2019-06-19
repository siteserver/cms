using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取站点值", Description = "通过 stl:site 标签在模板中显示站点值")]
    public class StlSite
    {
        private StlSite() { }
        public const string ElementName = "stl:site";

        [StlAttribute(Title = "站点名称")]
        private const string SiteName = nameof(SiteName);

        [StlAttribute(Title = "站点文件夹")]
        private const string SiteDir = nameof(SiteDir);

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

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeSiteName, "站点名称"},
            {TypeSiteUrl, "站点的域名地址"}
        };

        internal static object Parse(ParseContext parseContext)
        {
            var siteName = string.Empty;
            var siteDir = string.Empty;

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

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, SiteName))
                {
                    siteName = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteDir))
                {
                    siteDir = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
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

            var siteInfo = parseContext.SiteInfo;

            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = parseContext.SiteRepository.GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = parseContext.SiteRepository.GetSiteInfoByDirectory(siteDir);
            }

            if (parseContext.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return siteInfo;
            }

            return ParseImpl(parseContext, siteInfo, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
        }

        private static string ParseImpl(ParseContext parseContext, SiteInfo siteInfo, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            if (siteInfo == null) return string.Empty;

            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(parseContext.InnerHtml))
            {
                var preSiteInfo = parseContext.SiteInfo;
                var prePageChannelId = parseContext.PageChannelId;
                var prePageContentId = parseContext.PageContentId;

                parseContext.PageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, parseContext);

                var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                parseContext.ParseInnerContent(innerBuilder);
                parsedContent = innerBuilder.ToString();

                parseContext.PageInfo.ChangeSite(preSiteInfo, prePageChannelId, prePageContentId, parseContext);

                return parsedContent;
            }

            var inputType = InputType.Text;

            if (type.ToLower().Equals(TypeSiteName.ToLower()))
            {
                parsedContent = parseContext.SiteInfo.SiteName;
            }
            else if (type.ToLower().Equals(TypeSiteUrl.ToLower()))
            {
                parsedContent = parseContext.UrlManager.GetWebUrl(parseContext.SiteInfo);
            }
            else if (parseContext.SiteInfo.Get<string>(type) != null)
            {
                parsedContent = parseContext.SiteInfo.Get<string>(type);
                if (!string.IsNullOrEmpty(parsedContent))
                {
                    var styleInfo = parseContext.TableManager.GetTableStyleInfo(parseContext.SiteRepository.TableName, type, parseContext.TableManager.GetRelatedIdentities(parseContext.SiteId));

                    // 如果 styleInfo.TableStyleId <= 0，表示此字段已经被删除了，不需要再显示值了 ekun008
                    if (styleInfo.Id > 0)
                    {
                        if (isClearTags && InputTypeUtils.EqualsAny(styleInfo.Type, InputType.Image, InputType.File))
                        {
                            parsedContent = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                        }
                        else
                        {
                            parsedContent = InputParserUtility.GetContentByTableStyle(parseContext.FileManager, parseContext.UrlManager, parseContext.SettingsManager, parsedContent, separator, parseContext.SiteInfo, styleInfo, formatString, parseContext.Attributes, parseContext.InnerHtml, false);

                            inputType = styleInfo.Type;

                            //parsedContent = StringUtils.ParseString(styleInfo.InputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                    }
                    else
                    { // 如果字段已经被删除或不再显示了，则此字段的值为空。有时虚拟字段值不会清空
                        parsedContent = string.Empty;
                    }
                }
            }

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            return InputTypeUtils.ParseString(inputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
        }
    }
}
