using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
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
        public const string AttributeIsDynamic = "isDynamic";


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
            {AttributeIsOriginal, "如果是引用内容，是否获取所引用内容的值"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var attributes = new StringDictionary();
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
                var isDynamic = false;
                var type = ContentAttribute.Title.ToLower();

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value.ToLower();
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLeftText))
                        {
                            leftText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeRightText))
                        {
                            rightText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFormatString))
                        {
                            formatString = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeNo))
                        {
                            no = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSeparator))
                        {
                            separator = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStartIndex))
                        {
                            startIndex = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLength))
                        {
                            length = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeEllipsis))
                        {
                            ellipsis = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeReplace))
                        {
                            replace = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTo))
                        {
                            to = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsClearTags))
                        {
                            isClearTags = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsReturnToBr))
                        {
                            isReturnToBrStr = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLower))
                        {
                            isLower = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsUpper))
                        {
                            isUpper = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsOriginal))
                        {
                            isOriginal = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else
                        {
                            attributes.Add(attr.Name, attr.Value);
                        }
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, leftText, rightText, formatString, no, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBrStr, isLower, isUpper, isOriginal, type, attributes);

                    var innerBuilder = new StringBuilder(parsedContent);
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                    parsedContent = innerBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string formatString, string no, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, string isReturnToBrStr, bool isLower, bool isUpper, bool isOriginal, string type, StringDictionary attributes)
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
                    var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
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
                    if (wordNum == 0)
                    {
                        wordNum = contextInfo.TitleWordNum;
                    }
                    parsedContent = InputParserUtility.GetContentByTableStyle(contentInfo.Title, separator, pageInfo.PublishmentSystemInfo, tableStyle, styleInfo, formatString, attributes, node.InnerXml, false);
                    parsedContent = StringUtils.ParseString(EInputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                    if (!isClearTags && !string.IsNullOrEmpty(contentInfo.Attributes[BackgroundContentAttribute.TitleFormatString]))
                    {
                        parsedContent = ContentUtility.FormatTitle(contentInfo.Attributes[BackgroundContentAttribute.TitleFormatString], parsedContent);
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
                            for (var i = 0; i < innerLinkInfoList.Count; i++)
                            {
                                newInnerLinkInfo = innerLinkInfoList[i];
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
                        if (contextInfo.IsCurlyBrace)
                        {
                            sbParsedContent.Append(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl)));
                        }
                        else
                        {
                            sbParsedContent.Append(InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl), attributes, false));
                        }
                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
                        var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {
                                var newExtendValue = extendValue;
                                if (contextInfo.IsCurlyBrace)
                                {
                                    sbParsedContent.Append(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, newExtendValue));
                                }
                                else
                                {
                                    sbParsedContent.Append(InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, newExtendValue, attributes, false));
                                }
                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (num <= 1)
                        {
                            if (contextInfo.IsCurlyBrace)
                            {
                                parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl));
                            }
                            else
                            {
                                parsedContent = InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl), attributes, false);
                            }
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
                                        if (contextInfo.IsCurlyBrace)
                                        {
                                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, newExtendValue);
                                        }
                                        else
                                        {
                                            parsedContent = InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, newExtendValue, attributes, false);
                                        }
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
                        sbParsedContent.Append(InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl), attributes, false));

                        //第n条
                        var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.VideoUrl);
                        var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                        if (!string.IsNullOrEmpty(extendValues))
                        {
                            foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                            {

                                sbParsedContent.Append(InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, extendValue, attributes, false));

                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no, 0);
                        if (num <= 1)
                        {
                            parsedContent = InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl), attributes, false);
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
                                        parsedContent = InputParserUtility.GetVideoHtml(pageInfo.PublishmentSystemInfo, extendValue, attributes, false);
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
                            sbParsedContent.Append(InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl), attributes, node.InnerXml, false));
                            //第n条
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl);
                            var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    sbParsedContent.Append(InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, extendValue, attributes, node.InnerXml, false));
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
                        }
                        else
                        {
                            if (num <= 1)
                            {
                                parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl), attributes, node.InnerXml, false);
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
                                            parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, extendValue, attributes, node.InnerXml, false);
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
                        var displayName = BaiRongDataProvider.AdministratorDao.GetDisplayName(contentInfo.AddUserName);
                        parsedContent = string.IsNullOrEmpty(displayName) ? contentInfo.AddUserName : displayName;
                    }
                }
                else
                {
                    var isSelected = false;
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                    var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);

                    //WCM SPECIFIC

                    if (tableStyle == ETableStyle.GovInteractContent)
                    {
                        isSelected = true;
                        if (GovInteractContentAttribute.State.ToLower().Equals(type))
                        {
                            parsedContent = EGovInteractStateUtils.GetText(EGovInteractStateUtils.GetEnumType(contentInfo.GetExtendedAttribute(GovInteractContentAttribute.State)));
                        }
                        else if (StringUtils.EqualsIgnoreCase(type, GovInteractContentAttribute.Reply))
                        {
                            var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(pageInfo.PublishmentSystemId, contentInfo.Id);
                            if (replyInfo != null)
                            {
                                parsedContent = replyInfo.Reply;
                                if (!string.IsNullOrEmpty(parsedContent))
                                {
                                    parsedContent = StringUtils.ParseString(EInputType.TextEditor, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                                }
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(type, GovInteractContentAttribute.ReplyDepartment))
                        {
                            var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(pageInfo.PublishmentSystemId, contentInfo.Id);
                            if (replyInfo != null)
                            {
                                parsedContent = DepartmentManager.GetDepartmentName(replyInfo.DepartmentID);
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(type, GovInteractContentAttribute.ReplyUserName))
                        {
                            var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(pageInfo.PublishmentSystemId, contentInfo.Id);
                            if (replyInfo != null)
                            {
                                parsedContent = replyInfo.UserName;
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(type, GovInteractContentAttribute.ReplyDate))
                        {
                            var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(pageInfo.PublishmentSystemId, contentInfo.Id);
                            if (replyInfo != null)
                            {
                                var addDate = replyInfo.AddDate;
                                parsedContent = DateUtils.Format(addDate, formatString);
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(type, GovInteractContentAttribute.ReplyFileUrl))
                        {
                            var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(pageInfo.PublishmentSystemId, contentInfo.Id);
                            if (replyInfo != null)
                            {
                                parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, replyInfo.FileUrl);
                            }
                        }
                        else
                        {
                            isSelected = false;
                        }
                    }

                    //WCM SPECIFIC

                    if (!isSelected && contentInfo.ContainsKey(type))
                    {
                        if (!ContentAttribute.HiddenAttributes.Contains(type.ToLower()))
                        {
                            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);
                            var styleInfo = TableStyleManager.GetTableStyleInfo(tableStyle, tableName, type, relatedIdentities);
                            var num = TranslateUtils.ToInt(no);
                            parsedContent = InputParserUtility.GetContentByTableStyle(contentInfo, separator, pageInfo.PublishmentSystemInfo, tableStyle, styleInfo, formatString, num, attributes, node.InnerXml, false);
                            parsedContent = StringUtils.ParseString(EInputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
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
            else
            {
                if (!string.IsNullOrEmpty(type) && contextInfo.ItemContainer?.ContentItem != null)
                {
                    parsedContent = DataBinder.Eval(contextInfo.ItemContainer.ContentItem.DataItem, type, "{0}");

                    parsedContent = StringUtils.ParseString(parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = leftText + parsedContent + rightText;
                    }
                }
            }

            return parsedContent;
        }
    }
}
