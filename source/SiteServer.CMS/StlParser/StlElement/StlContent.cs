using System;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取内容值", Description = "通过 stl:content 标签在模板中显示指定内容的属性值")]
    public class StlContent
    {
        private StlContent() { }
        public const string ElementName = "stl:content";

        public const string AttributeType = "type";
        public const string AttributeLeftText = "leftText";
        public const string AttributeRightText = "rightText";
        public const string AttributeFormatString = "formatString";
        public const string AttributeNo = "no";
        public const string AttributeSeparator = "separator";
        public const string AttributeStartIndex = "startIndex";
        public const string AttributeLength = "length";
        public const string AttributeWordNum = "wordNum";
        public const string AttributeEllipsis = "ellipsis";
        public const string AttributeReplace = "replace";
        public const string AttributeTo = "to";
        public const string AttributeIsClearTags = "isClearTags";
        public const string AttributeIsReturnToBr = "isReturnToBr";
        public const string AttributeIsLower = "isLower";
        public const string AttributeIsUpper = "isUpper";
        public const string AttributeIsOriginal = "isOriginal";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, "显示的类型"},
            {AttributeLeftText, "显示在信息前的文字"},
            {AttributeRightText, "显示在信息后的文字"},
            {AttributeFormatString, "显示的格式"},
            {AttributeNo, "显示第几项"},
            {AttributeSeparator, "显示多项时的分割字符串"},
            {AttributeStartIndex, "字符开始位置"},
            {AttributeLength, "指定字符长度"},
            {AttributeWordNum, "显示字符的数目"},
            {AttributeEllipsis, "文字超出部分显示的文字"},
            {AttributeReplace, "需要替换的文字，可以是正则表达式"},
            {AttributeTo, "替换replace的文字信息"},
            {AttributeIsClearTags, "是否清除HTML标签"},
            {AttributeIsReturnToBr, "是否将回车替换为HTML换行标签"},
            {AttributeIsLower, "是否转换为小写"},
            {AttributeIsUpper, "是否转换为大写"},
            {AttributeIsOriginal, "如果是引用内容，是否获取所引用内容的值"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
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
            var type = ContentAttribute.Title.ToLower();

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value.ToLower();
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeLeftText))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeRightText))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeFormatString))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeNo))
                {
                    no = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSeparator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeStartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeLength))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeEllipsis))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeReplace))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTo))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsReturnToBr))
                {
                    isReturnToBrStr = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsOriginal))
                {
                    isOriginal = TranslateUtils.ToBool(value, true);
                }
            }

            var parsedContent = ParseImpl(pageInfo, contextInfo, leftText, rightText, formatString, no, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBrStr, isLower, isUpper, isOriginal, type);

            var innerBuilder = new StringBuilder(parsedContent);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            parsedContent = innerBuilder.ToString();

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string formatString, string no, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, string isReturnToBrStr, bool isLower, bool isUpper, bool isOriginal, string type)
        {
            var parsedContent = string.Empty;

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

            var contentId = contextInfo.ContentId;
            var contentInfo = contextInfo.ContentInfo;
            if (contentInfo == null) return string.Empty;

            if (isOriginal)
            {
                if (contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0 && contentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) == ETranslateContentType.Reference.ToString())
                {
                    var targetNodeId = contentInfo.SourceId;
                    //var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    //var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
                    var targetContentInfo = Content.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
                    if (targetContentInfo != null && targetContentInfo.NodeId > 0)
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
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                    var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);

                    var styleInfo = TableStyleManager.GetTableStyleInfo(tableStyle, tableName, type, relatedIdentities);
                    parsedContent = InputParserUtility.GetContentByTableStyle(contentInfo.Title, separator, pageInfo.PublishmentSystemInfo, tableStyle, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerXml, false);
                    parsedContent = StringUtils.ParseString(InputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                    if (!isClearTags && !string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.TitleFormatString)))
                    {
                        parsedContent = ContentUtility.FormatTitle(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.TitleFormatString), parsedContent);
                    }

                    if (!contextInfo.IsInnerElement)
                    {
                        parsedContent = parsedContent.Replace("&", "&amp;");
                    }

                    if (pageInfo.PublishmentSystemInfo.Additional.IsContentTitleBreakLine)
                    {
                        parsedContent = parsedContent.Replace("  ", !contextInfo.IsInnerElement ? "<br />" : string.Empty);
                    }
                }
                else if (BackgroundContentAttribute.Summary.ToLower().Equals(type))
                {
                    parsedContent = StringUtils.ParseString(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                    if (!contextInfo.IsInnerElement)
                    {
                        parsedContent = parsedContent.Replace("&", "&amp;");
                    }
                }
                else if (BackgroundContentAttribute.Content.ToLower().Equals(type))
                {
                    parsedContent = StringUtility.TextEditorContentDecode(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content), pageInfo.PublishmentSystemInfo);

                    if (pageInfo.PublishmentSystemInfo.Additional.IsInnerLink)
                    {
                        var innerLinkInfoList = pageInfo.CacheOfInnerLinkInfoList;
                        if (innerLinkInfoList != null && innerLinkInfoList.Count > 0)
                        {
                            InnerLinkInfo newInnerLinkInfo;
                            for (var i = 0; i < innerLinkInfoList.Count - 1; i++)
                            {
                                for (var j = i + 1; j < innerLinkInfoList.Count; j++)
                                {
                                    if (innerLinkInfoList[i].InnerLinkName.Length < innerLinkInfoList[j].InnerLinkName.Length)
                                    {
                                        newInnerLinkInfo = innerLinkInfoList[i];
                                        innerLinkInfoList[i] = innerLinkInfoList[j];
                                        innerLinkInfoList[j] = newInnerLinkInfo;
                                    }
                                }
                            }

                            var arrayLinkName = new List<string>();
                            var arrayInnerLink = new List<string>();
                            foreach (InnerLinkInfo innerLinkInfo in innerLinkInfoList)
                            {
                                newInnerLinkInfo = innerLinkInfo;
                                arrayLinkName.Add(newInnerLinkInfo.InnerLinkName);
                                arrayInnerLink.Add(newInnerLinkInfo.InnerString);
                            }
                            for (var m = 0; m < arrayLinkName.Count; m++)
                            {
                                var innerLinkName = arrayLinkName[m];
                                arrayLinkName[m] = Guid.NewGuid().ToString();
                                parsedContent = RegexUtils.Replace(
                                    $"({innerLinkName.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", parsedContent, arrayLinkName[m], pageInfo.PublishmentSystemInfo.Additional.InnerLinkMaxNum);

                            }
                            for (var n = 0; n < arrayLinkName.Count; n++)
                            {
                                parsedContent = RegexUtils.Replace(
                                    $"({arrayLinkName[n].Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", parsedContent, arrayInnerLink[n], pageInfo.PublishmentSystemInfo.Additional.InnerLinkMaxNum);
                            }
                        }
                    }

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

                    if (!contextInfo.IsInnerElement)
                    {
                        parsedContent = parsedContent.Replace("&", "&amp;");
                    }
                }
                else if (BackgroundContentAttribute.PageContent.ToLower().Equals(type))
                {
                    //if (contextInfo.IsInnerElement)
                    // {
                    parsedContent = StringUtility.TextEditorContentDecode(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content), pageInfo.PublishmentSystemInfo);


                    if (pageInfo.PublishmentSystemInfo.Additional.IsInnerLink)
                    {
                        var innerLinkInfoList = pageInfo.CacheOfInnerLinkInfoList;
                        if (innerLinkInfoList != null && innerLinkInfoList.Count > 0)
                        {
                            InnerLinkInfo newInnerLinkInfo;
                            for (var i = 0; i < innerLinkInfoList.Count - 1; i++)
                            {
                                for (var j = i + 1; j < innerLinkInfoList.Count; j++)
                                {

                                    if (innerLinkInfoList[i].InnerLinkName.Length < innerLinkInfoList[j].InnerLinkName.Length)
                                    {
                                        newInnerLinkInfo = innerLinkInfoList[i];
                                        innerLinkInfoList[i] = innerLinkInfoList[j];
                                        innerLinkInfoList[j] = newInnerLinkInfo;
                                    }
                                }
                            }
                            for (var i = 0; i < innerLinkInfoList.Count; i++)
                            {
                                newInnerLinkInfo = innerLinkInfoList[i];
                                for (var j = i + 1; j < innerLinkInfoList.Count; j++)
                                {
                                    var lastInnerLinkInfo = innerLinkInfoList[j];
                                    if (newInnerLinkInfo.InnerLinkName.Contains(lastInnerLinkInfo.InnerLinkName))
                                    {
                                        innerLinkInfoList.Remove(lastInnerLinkInfo);
                                    }
                                }
                                parsedContent = RegexUtils.Replace(
                                    $"({newInnerLinkInfo.InnerLinkName.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", parsedContent, newInnerLinkInfo.InnerString, pageInfo.PublishmentSystemInfo.Additional.InnerLinkMaxNum);
                            }
                        }
                    }

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
                        sbParsedContent.Append(contextInfo.IsCurlyBrace
                            ? PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo,
                                contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl))
                            : InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo,
                                contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl),
                                contextInfo.Attributes, false));
                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
                        var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {
                                var newExtendValue = extendValue;
                                sbParsedContent.Append(contextInfo.IsCurlyBrace
                                    ? PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, newExtendValue)
                                    : InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo,
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
                            parsedContent = contextInfo.IsCurlyBrace ? PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl)) : InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl), contextInfo.Attributes, false);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
                            var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    var newExtendValue = extendValue;
                                    if (index == num)
                                    {
                                        parsedContent = contextInfo.IsCurlyBrace ? PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, newExtendValue) : InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, newExtendValue, contextInfo.Attributes, false);
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
                        sbParsedContent.Append(InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl), contextInfo.Attributes, contextInfo.IsCurlyBrace));

                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.VideoUrl);
                        var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {

                                sbParsedContent.Append(InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, extendValue, contextInfo.Attributes, contextInfo.IsCurlyBrace));

                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (num <= 1)
                        {
                            parsedContent = InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl), contextInfo.Attributes, contextInfo.IsCurlyBrace);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.VideoUrl);
                            var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    if (index == num)
                                    {
                                        parsedContent = InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, extendValue, contextInfo.Attributes, contextInfo.IsCurlyBrace);
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
                        if (contextInfo.IsCurlyBrace)
                        {
                            //第一条
                            sbParsedContent.Append(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                            var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
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
                            sbParsedContent.Append(InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl), contextInfo.Attributes, contextInfo.InnerXml, false));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                            var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    sbParsedContent.Append(InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, extendValue, contextInfo.Attributes, contextInfo.InnerXml, false));
                                }
                            }

                        }

                        parsedContent = sbParsedContent.ToString();

                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (contextInfo.IsCurlyBrace)
                        {
                            if (num <= 1)
                            {
                                parsedContent = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                                var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
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
                                parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, parsedContent);
                            }
                        }
                        else
                        {
                            if (num <= 1)
                            {
                                parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl), contextInfo.Attributes, contextInfo.InnerXml, false);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                                var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                    {
                                        if (index == num)
                                        {
                                            parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, extendValue, contextInfo.Attributes, contextInfo.InnerXml, false);
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
                    parsedContent = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo);
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
                        //var displayName = BaiRongDataProvider.AdministratorDao.GetDisplayName(contentInfo.AddUserName);
                        var displayName = Administrator.GetDisplayName(contentInfo.AddUserName);
                        parsedContent = string.IsNullOrEmpty(displayName) ? contentInfo.AddUserName : displayName;
                    }
                }
                else
                {
                    var isSelected = false;
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                    var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);

                    if (!isSelected && contentInfo.ContainsKey(type))
                    {
                        if (!ContentAttribute.HiddenAttributes.Contains(type.ToLower()))
                        {
                            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);
                            var styleInfo = TableStyleManager.GetTableStyleInfo(tableStyle, tableName, type, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            if (styleInfo.IsVisible)
                            {
                                var num = TranslateUtils.ToInt(no);
                                parsedContent = InputParserUtility.GetContentByTableStyle(contentInfo, separator, pageInfo.PublishmentSystemInfo, tableStyle, styleInfo, formatString, num, contextInfo.Attributes, contextInfo.InnerXml, false);
                                parsedContent = StringUtils.ParseString(InputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                            }
                            else
                            {
                                parsedContent = string.Empty;
                            } 
                        }
                        else
                        {
                            parsedContent = contentInfo.GetExtendedAttribute(type);
                            parsedContent = StringUtils.ParseString(parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                    }

                    if (!contextInfo.IsInnerElement)
                    {
                        parsedContent = parsedContent.Replace("&", "&amp;");
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
