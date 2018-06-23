using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "获取内容值", Description = "通过 stl:content 标签在模板中显示指定内容的属性值")]
    public class StlContent
    {
        private StlContent() { }
        public const string ElementName = "stl:content";

        private static readonly Attr Type = new Attr("type", "显示的类型");
        private static readonly Attr LeftText = new Attr("leftText", "显示在信息前的文字");
        private static readonly Attr RightText = new Attr("rightText", "显示在信息后的文字");
        private static readonly Attr FormatString = new Attr("formatString", "显示的格式");
        private static readonly Attr No = new Attr("no", "显示第几项");
        private static readonly Attr Separator = new Attr("separator", "显示多项时的分割字符串");
        private static readonly Attr StartIndex = new Attr("startIndex", "字符开始位置");
        private static readonly Attr Length = new Attr("length", "指定字符长度");
        private static readonly Attr WordNum = new Attr("wordNum", "显示字符的数目");
        private static readonly Attr Ellipsis = new Attr("ellipsis", "文字超出部分显示的文字");
        private static readonly Attr Replace = new Attr("replace", "需要替换的文字，可以是正则表达式");
        private static readonly Attr To = new Attr("to", "替换replace的文字信息");
        private static readonly Attr IsClearTags = new Attr("isClearTags", "是否清除HTML标签");
        private static readonly Attr IsReturnToBr = new Attr("isReturnToBr", "是否将回车替换为HTML换行标签");
        private static readonly Attr IsLower = new Attr("isLower", "是否转换为小写");
        private static readonly Attr IsUpper = new Attr("isUpper", "是否转换为大写");
        private static readonly Attr IsOriginal = new Attr("isOriginal", "如果是引用内容，是否获取所引用内容的值");

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var leftText = string.Empty;
            var rightText = string.Empty;
            var formatString = string.Empty;
            var no = "0";
            string separator = null;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = StringUtils.Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBrStr = string.Empty;
            var isLower = false;
            var isUpper = false;
            var isOriginal = true;//引用的时候，默认使用原来的数据
            var type = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value.ToLower();
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText.Name))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText.Name))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FormatString.Name))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, No.Name))
                {
                    no = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Separator.Name))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartIndex.Name))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Length.Name))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum.Name))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Ellipsis.Name))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Replace.Name))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, To.Name))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsClearTags.Name))
                {
                    isClearTags = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsReturnToBr.Name))
                {
                    isReturnToBrStr = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower.Name))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper.Name))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOriginal.Name))
                {
                    isOriginal = TranslateUtils.ToBool(value, true);
                }
            }

            var contentId = contextInfo.ContentId;
            var contentInfo = contextInfo.ContentInfo;

            if (contextInfo.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return contentInfo;
            }

            var parsedContent = ParseImpl(pageInfo, contextInfo, leftText, rightText, formatString, no, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBrStr, isLower, isUpper, isOriginal, type, contentInfo, contentId);

            var innerBuilder = new StringBuilder(parsedContent);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            parsedContent = innerBuilder.ToString();

            parsedContent = parsedContent.Replace(ContentUtility.PagePlaceHolder, string.Empty);

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string formatString, string no, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, string isReturnToBrStr, bool isLower, bool isUpper, bool isOriginal, string type, ContentInfo contentInfo, int contentId)
        {
            if (contentInfo == null) return string.Empty;

            var parsedContent = string.Empty;

            if (string.IsNullOrEmpty(type))
            {
                type = ContentAttribute.Title.ToLower();
            }

            var isReturnToBr = false;
            if (string.IsNullOrEmpty(isReturnToBrStr))
            {
                if (BackgroundContentAttribute.Summary.ToLower().Equals(type))
                {
                    isReturnToBr = true;
                }
            }
            else
            {
                isReturnToBr = TranslateUtils.ToBool(isReturnToBrStr, true);
            }

            if (isOriginal)
            {
                if (contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0 && contentInfo.GetString(ContentAttribute.TranslateContentType) == ETranslateContentType.Reference.ToString())
                {
                    var targetChannelId = contentInfo.SourceId;
                    //var targetSiteId = DataProvider.ChannelDao.GetSiteId(targetChannelId);
                    var targetSiteId = Node.GetSiteId(targetChannelId);
                    var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                    var targetNodeInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);

                    var tableName = ChannelManager.GetTableName(targetSiteInfo, targetNodeInfo);
                    //var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
                    var targetContentInfo = Content.GetContentInfo(tableName, contentInfo.ReferenceId);
                    if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                    {
                        //标题可以使用自己的
                        targetContentInfo.Title = contentInfo.Title;
                        contentInfo = targetContentInfo;
                    }
                }
            }

            if (!string.IsNullOrEmpty(formatString))
            {
                formatString = formatString.Trim();
                if (!formatString.StartsWith("{0"))
                {
                    formatString = "{0:" + formatString;
                }
                if (!formatString.EndsWith("}"))
                {
                    formatString = formatString + "}";
                }
            }

            if (contentId != 0)
            {
                if (ContentAttribute.Title.ToLower().Equals(type))
                {
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.SiteId, contentInfo.ChannelId);
                    var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contentInfo.ChannelId);
                    var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, nodeInfo);

                    var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, type, relatedIdentities);
                    parsedContent = InputParserUtility.GetContentByTableStyle(contentInfo.Title, separator, pageInfo.SiteInfo, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerHtml, false);
                    parsedContent = StringUtils.ParseString(styleInfo.InputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                    if (!isClearTags && !string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.TitleFormatString)))
                    {
                        parsedContent = ContentUtility.FormatTitle(contentInfo.GetString(BackgroundContentAttribute.TitleFormatString), parsedContent);
                    }

                    if (pageInfo.SiteInfo.Additional.IsContentTitleBreakLine)
                    {
                        parsedContent = parsedContent.Replace("  ", !contextInfo.IsInnerElement ? "<br />" : string.Empty);
                    }
                }
                else if (BackgroundContentAttribute.Summary.ToLower().Equals(type))
                {
                    parsedContent = StringUtils.ParseString(contentInfo.GetString(BackgroundContentAttribute.Summary), replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                }
                else if (BackgroundContentAttribute.Content.ToLower().Equals(type))
                {
                    parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.SiteInfo, contentInfo.GetString(BackgroundContentAttribute.Content), pageInfo.IsLocal);

                    if (isClearTags)
                    {
                        parsedContent = StringUtils.StripTags(parsedContent);
                    }

                    if (!string.IsNullOrEmpty(replace))
                    {
                        parsedContent = StringUtils.Replace(replace, parsedContent, to);
                    }

                    if (wordNum > 0 && !string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    }

                    if (!string.IsNullOrEmpty(formatString))
                    {
                        parsedContent = string.Format(formatString, parsedContent);
                    }
                }
                else if (BackgroundContentAttribute.PageContent.ToLower().Equals(type))
                {
                    //if (contextInfo.IsInnerElement)
                    // {
                    parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.SiteInfo, contentInfo.GetString(BackgroundContentAttribute.Content), pageInfo.IsLocal);

                    if (isClearTags)
                    {
                        parsedContent = StringUtils.StripTags(parsedContent);
                    }

                    if (!string.IsNullOrEmpty(replace))
                    {
                        parsedContent = StringUtils.Replace(replace, parsedContent, to);
                    }

                    if (wordNum > 0 && !string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    }

                    if (!string.IsNullOrEmpty(formatString))
                    {
                        parsedContent = string.Format(formatString, parsedContent);
                    }
                }
                else if (ContentAttribute.AddDate.ToLower().Equals(type))
                {
                    parsedContent = DateUtils.Format(contentInfo.AddDate, formatString);
                }
                else if (ContentAttribute.LastEditDate.ToLower().Equals(type))
                {
                    parsedContent = DateUtils.Format(contentInfo.LastEditDate, formatString);
                }
                else if (BackgroundContentAttribute.ImageUrl.ToLower().Equals(type))
                {
                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        //第一条
                        sbParsedContent.Append(contextInfo.IsStlEntity
                            ? PageUtility.ParseNavigationUrl(pageInfo.SiteInfo,
                                contentInfo.GetString(BackgroundContentAttribute.ImageUrl), pageInfo.IsLocal)
                            : InputParserUtility.GetImageOrFlashHtml(pageInfo.SiteInfo,
                                contentInfo.GetString(BackgroundContentAttribute.ImageUrl),
                                contextInfo.Attributes, false));
                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
                        var extendValues = contentInfo.GetString(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {
                                var newExtendValue = extendValue;
                                sbParsedContent.Append(contextInfo.IsStlEntity
                                    ? PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, newExtendValue, pageInfo.IsLocal)
                                    : InputParserUtility.GetImageOrFlashHtml(pageInfo.SiteInfo,
                                        newExtendValue, contextInfo.Attributes, false));
                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (num <= 1)
                        {
                            parsedContent = contextInfo.IsStlEntity ? PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, contentInfo.GetString(BackgroundContentAttribute.ImageUrl), pageInfo.IsLocal) : InputParserUtility.GetImageOrFlashHtml(pageInfo.SiteInfo, contentInfo.GetString(BackgroundContentAttribute.ImageUrl), contextInfo.Attributes, false);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
                            var extendValues = contentInfo.GetString(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    var newExtendValue = extendValue;
                                    if (index == num)
                                    {
                                        parsedContent = contextInfo.IsStlEntity ? PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, newExtendValue, pageInfo.IsLocal) : InputParserUtility.GetImageOrFlashHtml(pageInfo.SiteInfo, newExtendValue, contextInfo.Attributes, false);
                                        break;
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                }
                else if (BackgroundContentAttribute.VideoUrl.ToLower().Equals(type))
                {
                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        //第一条
                        sbParsedContent.Append(InputParserUtility.GetVideoHtml(pageInfo.SiteInfo, contentInfo.GetString(BackgroundContentAttribute.VideoUrl), contextInfo.Attributes, contextInfo.IsStlEntity));

                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.VideoUrl);
                        var extendValues = contentInfo.GetString(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {

                                sbParsedContent.Append(InputParserUtility.GetVideoHtml(pageInfo.SiteInfo, extendValue, contextInfo.Attributes, contextInfo.IsStlEntity));

                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (num <= 1)
                        {
                            parsedContent = InputParserUtility.GetVideoHtml(pageInfo.SiteInfo, contentInfo.GetString(BackgroundContentAttribute.VideoUrl), contextInfo.Attributes, contextInfo.IsStlEntity);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.VideoUrl);
                            var extendValues = contentInfo.GetString(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    if (index == num)
                                    {
                                        parsedContent = InputParserUtility.GetVideoHtml(pageInfo.SiteInfo, extendValue, contextInfo.Attributes, contextInfo.IsStlEntity);
                                        break;
                                    }
                                    index++;
                                }
                            }
                        }
                    }

                }
                else if (BackgroundContentAttribute.FileUrl.ToLower().Equals(type))
                {

                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        if (contextInfo.IsStlEntity)
                        {
                            //第一条
                            sbParsedContent.Append(contentInfo.GetString(BackgroundContentAttribute.FileUrl));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                            var extendValues = contentInfo.GetString(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    sbParsedContent.Append(extendValue);
                                }
                            }
                        }
                        else
                        {
                            //第一条
                            sbParsedContent.Append(InputParserUtility.GetFileHtmlWithCount(pageInfo.SiteInfo, contentInfo.ChannelId, contentInfo.Id, contentInfo.GetString(BackgroundContentAttribute.FileUrl), contextInfo.Attributes, contextInfo.InnerHtml, false));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                            var extendValues = contentInfo.GetString(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    sbParsedContent.Append(InputParserUtility.GetFileHtmlWithCount(pageInfo.SiteInfo, contentInfo.ChannelId, contentInfo.Id, extendValue, contextInfo.Attributes, contextInfo.InnerHtml, false));
                                }
                            }

                        }

                        parsedContent = sbParsedContent.ToString();

                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (contextInfo.IsStlEntity)
                        {
                            if (num <= 1)
                            {
                                parsedContent = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                                var extendValues = contentInfo.GetString(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                    {
                                        if (index == num)
                                        {
                                            parsedContent = extendValue;
                                            break;
                                        }
                                        index++;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(parsedContent))
                            {
                                parsedContent = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, parsedContent, pageInfo.IsLocal);
                            }
                        }
                        else
                        {
                            if (num <= 1)
                            {
                                parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.SiteInfo, contentInfo.ChannelId, contentInfo.Id, contentInfo.GetString(BackgroundContentAttribute.FileUrl), contextInfo.Attributes, contextInfo.InnerHtml, false);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                                var extendValues = contentInfo.GetString(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                    {
                                        if (index == num)
                                        {
                                            parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.SiteInfo, contentInfo.ChannelId, contentInfo.Id, extendValue, contextInfo.Attributes, contextInfo.InnerHtml, false);
                                            break;
                                        }
                                        index++;
                                    }
                                }
                            }
                        }
                    }


                }
                else if (BackgroundContentAttribute.NavigationUrl.ToLower().Equals(type))
                {
                    parsedContent = PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal);
                }
                else if (ContentAttribute.Tags.ToLower().Equals(type))
                {
                    parsedContent = contentInfo.Tags;
                }
                else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ContentItem != null)
                {
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.ItemIndex, type, contextInfo);
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else if (ContentAttribute.AddUserName.ToLower().Equals(type))
                {
                    if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                    {
                        //var displayName = DataProvider.AdministratorDao.GetDisplayName(contentInfo.AddUserName);
                        var displayName = Administrator.GetDisplayName(contentInfo.AddUserName);
                        parsedContent = string.IsNullOrEmpty(displayName) ? contentInfo.AddUserName : displayName;
                    }
                }
                else
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contentInfo.ChannelId);

                    if (contentInfo.ContainsKey(type))
                    {
                        if (!ContentAttribute.AllAttributesLowercase.Contains(type.ToLower()))
                        {
                            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.SiteId, contentInfo.ChannelId);
                            var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, nodeInfo);
                            var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, type, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            var num = TranslateUtils.ToInt(no);
                            parsedContent = InputParserUtility.GetContentByTableStyle(contentInfo, separator, pageInfo.SiteInfo, styleInfo, formatString, num, contextInfo.Attributes, contextInfo.InnerHtml, false);
                            parsedContent = StringUtils.ParseString(styleInfo.InputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                        else
                        {
                            parsedContent = contentInfo.GetString(type);
                            parsedContent = StringUtils.ParseString(parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }
    }
}
