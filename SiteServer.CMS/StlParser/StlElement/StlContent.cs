using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "获取内容值", Description = "通过 stl:content 标签在模板中显示指定内容的属性值")]
    public class StlContent
    {
        private StlContent() { }
        public const string ElementName = "stl:content";

        [StlAttribute(Title = "显示的类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "显示在信息前的文字")]
        private const string LeftText = nameof(LeftText);

        [StlAttribute(Title = "显示在信息后的文字")]
        private const string RightText = nameof(RightText);

        [StlAttribute(Title = "显示的格式")]
        private const string FormatString = nameof(FormatString);

        [StlAttribute(Title = "显示第几项")]
        private const string No = nameof(No);

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

        [StlAttribute(Title = "是否清除HTML标签")]
        private const string IsClearTags = nameof(IsClearTags);

        [StlAttribute(Title = "是否将回车替换为HTML换行标签")]
        private const string IsReturnToBr = nameof(IsReturnToBr);

        [StlAttribute(Title = "是否转换为小写")]
        private const string IsLower = nameof(IsLower);

        [StlAttribute(Title = "是否转换为大写")]
        private const string IsUpper = nameof(IsUpper);

        [StlAttribute(Title = "如果是引用内容，是否获取所引用内容的值")]
        private const string IsOriginal = nameof(IsOriginal);
        
        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var leftText = string.Empty;
            var rightText = string.Empty;
            var formatString = string.Empty;
            var no = "0";
            string separator = null;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = Constants.Ellipsis;
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

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value.ToLower();
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FormatString))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, No))
                {
                    no = value;
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
                    isClearTags = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsReturnToBr))
                {
                    isReturnToBrStr = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOriginal))
                {
                    isOriginal = TranslateUtils.ToBool(value, true);
                }
            }

            var contentId = contextInfo.ContentId;
            var contentInfo = await contextInfo.GetContentAsync();

            if (contextInfo.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return contentInfo.ToDictionary();
            }

            var parsedContent = await ParseImplAsync(pageInfo, contextInfo, leftText, rightText, formatString, no, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBrStr, isLower, isUpper, isOriginal, type, contentInfo, contentId);

            var innerBuilder = new StringBuilder(parsedContent);
            await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
            parsedContent = innerBuilder.ToString();

            if (!StringUtils.EqualsIgnoreCase(type, ContentAttribute.PageContent))
            {
                parsedContent = parsedContent.Replace(ContentUtility.PagePlaceHolder, string.Empty);
            }

            return parsedContent;
        }

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string formatString, string no, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, string isReturnToBrStr, bool isLower, bool isUpper, bool isOriginal, string type, Content content, int contentId)
        {
            if (content == null) return string.Empty;

            var parsedContent = string.Empty;

            if (string.IsNullOrEmpty(type))
            {
                type = ContentAttribute.Title.ToLower();
            }

            var isReturnToBr = false;
            if (string.IsNullOrEmpty(isReturnToBrStr))
            {
                if (ContentAttribute.Summary.ToLower().Equals(type))
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
                if (content.ReferenceId > 0 && content.SourceId > 0 && content.Get<string>(ContentAttribute.TranslateContentType) == ETranslateContentType.Reference.ToString())
                {
                    var targetChannelId = content.SourceId;
                    //var targetSiteId = DataProvider.ChannelDao.GetSiteId(targetChannelId);
                    var targetSiteId = await StlChannelCache.GetSiteIdAsync(targetChannelId);
                    var targetSite = await SiteManager.GetSiteAsync(targetSiteId);
                    var targetNodeInfo = await ChannelManager.GetChannelAsync(targetSiteId, targetChannelId);

                    //var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, content.ReferenceId);
                    var targetContentInfo = await ContentManager.GetContentInfoAsync(targetSite, targetNodeInfo, content.ReferenceId);
                    if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                    {
                        //标题可以使用自己的
                        targetContentInfo.Title = content.Title;
                        content = targetContentInfo;
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
                    var nodeInfo = await ChannelManager.GetChannelAsync(pageInfo.SiteId, content.ChannelId);
                    var relatedIdentities = TableStyleManager.GetRelatedIdentities(nodeInfo);
                    var tableName = await ChannelManager.GetTableNameAsync(pageInfo.Site, nodeInfo);

                    var styleInfo = await TableStyleManager.GetTableStyleAsync(tableName, type, relatedIdentities);
                    parsedContent = await InputParserUtility.GetContentByTableStyleAsync(content.Title, separator, pageInfo.Site, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerHtml, false);
                    parsedContent = InputTypeUtils.ParseString(styleInfo.Type, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                    if (!isClearTags && !string.IsNullOrEmpty(content.Get<string>(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title))))
                    {
                        parsedContent = ContentUtility.FormatTitle(content.Get<string>(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)), parsedContent);
                    }

                    if (pageInfo.Site.IsContentTitleBreakLine)
                    {
                        parsedContent = parsedContent.Replace("  ", !contextInfo.IsInnerElement ? "<br />" : string.Empty);
                    }
                }
                else if (ContentAttribute.SubTitle.ToLower().Equals(type))
                {
                    parsedContent = InputTypeUtils.ParseString(InputType.Text, content.Get<string>(ContentAttribute.SubTitle), replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                    if (pageInfo.Site.IsContentSubTitleBreakLine)
                    {
                        parsedContent = parsedContent.Replace("  ", !contextInfo.IsInnerElement ? "<br />" : string.Empty);
                    }
                }
                else if (ContentAttribute.Summary.ToLower().Equals(type))
                {
                    parsedContent = InputTypeUtils.ParseString(InputType.TextArea,  content.Get<string>(ContentAttribute.Summary), replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                }
                else if (ContentAttribute.Content.ToLower().Equals(type))
                {
                    parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.Site, content.Get<string>(ContentAttribute.Content), pageInfo.IsLocal);

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
                        parsedContent = WebUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    }

                    if (!string.IsNullOrEmpty(formatString))
                    {
                        parsedContent = string.Format(formatString, parsedContent);
                    }
                }
                else if (ContentAttribute.PageContent.ToLower().Equals(type))
                {
                    //if (contextInfo.IsInnerElement)
                    // {
                    parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.Site, content.Get<string>(ContentAttribute.Content), pageInfo.IsLocal);

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
                        parsedContent = WebUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    }

                    if (!string.IsNullOrEmpty(formatString))
                    {
                        parsedContent = string.Format(formatString, parsedContent);
                    }
                }
                else if (ContentAttribute.AddDate.ToLower().Equals(type))
                {
                    parsedContent = DateUtils.Format(content.AddDate, formatString);
                }
                else if (ContentAttribute.LastEditDate.ToLower().Equals(type))
                {
                    parsedContent = DateUtils.Format(content.LastEditDate, formatString);
                }
                else if (ContentAttribute.LastHitsDate.ToLower().Equals(type))
                {
                    parsedContent = DateUtils.Format(content.LastHitsDate, formatString);
                }
                else if (ContentAttribute.ImageUrl.ToLower().Equals(type))
                {
                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        //第一条
                        sbParsedContent.Append(contextInfo.IsStlEntity
                            ? PageUtility.ParseNavigationUrl(pageInfo.Site,
                                content.Get<string>(ContentAttribute.ImageUrl), pageInfo.IsLocal)
                            : InputParserUtility.GetImageOrFlashHtml(pageInfo.Site,
                                content.Get<string>(ContentAttribute.ImageUrl),
                                contextInfo.Attributes, false));
                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl);
                        var extendValues = content.Get<string>(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {
                                var newExtendValue = extendValue;
                                sbParsedContent.Append(contextInfo.IsStlEntity
                                    ? PageUtility.ParseNavigationUrl(pageInfo.Site, newExtendValue, pageInfo.IsLocal)
                                    : InputParserUtility.GetImageOrFlashHtml(pageInfo.Site,
                                        newExtendValue, contextInfo.Attributes, false));
                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no);
                        if (num <= 1)
                        {
                            parsedContent = contextInfo.IsStlEntity ? PageUtility.ParseNavigationUrl(pageInfo.Site, content.Get<string>(ContentAttribute.ImageUrl), pageInfo.IsLocal) : InputParserUtility.GetImageOrFlashHtml(pageInfo.Site, content.Get<string>(ContentAttribute.ImageUrl), contextInfo.Attributes, false);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl);
                            var extendValues = content.Get<string>(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    var newExtendValue = extendValue;
                                    if (index == num)
                                    {
                                        parsedContent = contextInfo.IsStlEntity ? PageUtility.ParseNavigationUrl(pageInfo.Site, newExtendValue, pageInfo.IsLocal) : InputParserUtility.GetImageOrFlashHtml(pageInfo.Site, newExtendValue, contextInfo.Attributes, false);
                                        break;
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                }
                else if (ContentAttribute.VideoUrl.ToLower().Equals(type))
                {
                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        //第一条
                        sbParsedContent.Append(await InputParserUtility.GetVideoHtmlAsync(pageInfo.Site, content.Get<string>(ContentAttribute.VideoUrl), contextInfo.Attributes, contextInfo.IsStlEntity));

                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.VideoUrl);
                        var extendValues = content.Get<string>(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {

                                sbParsedContent.Append(await InputParserUtility.GetVideoHtmlAsync(pageInfo.Site, extendValue, contextInfo.Attributes, contextInfo.IsStlEntity));

                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no);
                        if (num <= 1)
                        {
                            parsedContent = await InputParserUtility.GetVideoHtmlAsync(pageInfo.Site, content.Get<string>(ContentAttribute.VideoUrl), contextInfo.Attributes, contextInfo.IsStlEntity);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.VideoUrl);
                            var extendValues = content.Get<string>(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    if (index == num)
                                    {
                                        parsedContent = await InputParserUtility.GetVideoHtmlAsync(pageInfo.Site, extendValue, contextInfo.Attributes, contextInfo.IsStlEntity);
                                        break;
                                    }
                                    index++;
                                }
                            }
                        }
                    }

                }
                else if (ContentAttribute.FileUrl.ToLower().Equals(type))
                {

                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        if (contextInfo.IsStlEntity)
                        {
                            //第一条
                            sbParsedContent.Append(content.Get<string>(ContentAttribute.FileUrl));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl);
                            var extendValues = content.Get<string>(extendAttributeName);
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
                            sbParsedContent.Append(await InputParserUtility.GetFileHtmlWithCountAsync(pageInfo.Site, content.ChannelId, content.Id, content.Get<string>(ContentAttribute.FileUrl), contextInfo.Attributes, contextInfo.InnerHtml, false, isLower, isUpper));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl);
                            var extendValues = content.Get<string>(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    sbParsedContent.Append(await InputParserUtility.GetFileHtmlWithCountAsync(pageInfo.Site, content.ChannelId, content.Id, extendValue, contextInfo.Attributes, contextInfo.InnerHtml, false, isLower, isUpper));
                                }
                            }

                        }

                        parsedContent = sbParsedContent.ToString();

                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no);
                        if (contextInfo.IsStlEntity)
                        {
                            if (num <= 1)
                            {
                                parsedContent = content.Get<string>(ContentAttribute.FileUrl);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl);
                                var extendValues = content.Get<string>(extendAttributeName);
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
                                parsedContent = PageUtility.ParseNavigationUrl(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                            }
                        }
                        else
                        {
                            if (num <= 1)
                            {
                                parsedContent = await InputParserUtility.GetFileHtmlWithCountAsync(pageInfo.Site, content.ChannelId, content.Id, content.Get<string>(ContentAttribute.FileUrl), contextInfo.Attributes, contextInfo.InnerHtml, false, isLower, isUpper);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl);
                                var extendValues = content.Get<string>(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                    {
                                        if (index == num)
                                        {
                                            parsedContent = await InputParserUtility.GetFileHtmlWithCountAsync(pageInfo.Site, content.ChannelId, content.Id, extendValue, contextInfo.Attributes, contextInfo.InnerHtml, false, isLower, isUpper);
                                            break;
                                        }
                                        index++;
                                    }
                                }
                            }
                        }
                    }


                }
                else if (ContentAttribute.NavigationUrl.ToLower().Equals(type))
                {
                    parsedContent = await PageUtility.GetContentUrlAsync(pageInfo.Site, content, pageInfo.IsLocal);
                }
                else if (ContentAttribute.Tags.ToLower().Equals(type))
                {
                    parsedContent = content.Tags;
                }
                else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ContentItem != null)
                {
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.ItemIndex, type, contextInfo);
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else if (ContentAttribute.AddUserName.ToLower().Equals(type))
                {
                    if (!string.IsNullOrEmpty(content.AddUserName))
                    {
                        parsedContent = content.AddUserName;
                    }
                }
                else
                {
                    var nodeInfo = await ChannelManager.GetChannelAsync(pageInfo.SiteId, content.ChannelId);

                    if (content.ContainsKey(type))
                    {
                        if (!StringUtils.ContainsIgnoreCase(ContentAttribute.AllAttributes.Value, type))
                        {
                            var relatedIdentities = TableStyleManager.GetRelatedIdentities(nodeInfo);
                            var tableName = await ChannelManager.GetTableNameAsync(pageInfo.Site, nodeInfo);
                            var styleInfo = await TableStyleManager.GetTableStyleAsync(tableName, type, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            var num = TranslateUtils.ToInt(no);
                            parsedContent = await InputParserUtility.GetContentByTableStyleAsync(content, separator, pageInfo.Site, styleInfo, formatString, num, contextInfo.Attributes, contextInfo.InnerHtml, false);
                            parsedContent = InputTypeUtils.ParseString(styleInfo.Type, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                        else
                        {
                            parsedContent = content.Get<string>(type);
                            parsedContent = InputTypeUtils.ParseString(InputType.Text, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
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
