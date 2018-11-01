using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "翻页项", Description = "通过 stl:pageItem 标签在模板中显示翻页项（上一页、下一页、当前页、页跳转、页导航等）")]
    public class StlPageItem
    {
        private StlPageItem() { }
        public const string ElementName = "stl:pageItem";

        [StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "显示的文字")]
        private const string Text = nameof(Text);

        [StlAttribute(Title = "链接CSS样式")]
        private const string LinkClass = nameof(LinkClass);

        [StlAttribute(Title = "文字CSS样式")]
        private const string TextClass = nameof(TextClass);

        [StlAttribute(Title = "页导航或页跳转显示链接数")]
        private const string ListNum = nameof(ListNum);

        [StlAttribute(Title = "页导航或页跳转链接太多时显示的省略号")]
        private const string ListEllipsis = nameof(ListEllipsis);

        [StlAttribute(Title = "页码导航是否包含左右字符")]
        private const string HasLr = nameof(HasLr);

        [StlAttribute(Title = "页面左字符")]
        private const string LStr = nameof(LStr);

        [StlAttribute(Title = "页面右字符")]
        private const string RStr = nameof(RStr);

        [StlAttribute(Title = "页码总是超链接，包括无连接时")]
        private const string AlwaysA = nameof(AlwaysA);

        public const string TypePreviousPage = "PreviousPage";				            //上一页
        public const string TypeNextPage = "NextPage";						            //下一页
        public const string TypeFirstPage = "FirstPage";						        //首页
        public const string TypeLastPage = "LastPage";						            //末页
        public const string TypeCurrentPageIndex = "CurrentPageIndex";		            //当前页索引
        public const string TypeTotalPageNum = "TotalPageNum";		                    //总页数
        public const string TypeTotalNum = "TotalNum";		                            //总内容数
        public const string TypePageNavigation = "PageNavigation";			            //页导航
        public const string TypePageSelect = "PageSelect";			                    //页跳转

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypePreviousPage, "上一页"},
            {TypeNextPage, "下一页"},
            {TypeFirstPage, "首页"},
            {TypeLastPage, "末页"},
            {TypeCurrentPageIndex, "当前页索引"},
            {TypeTotalPageNum, "总页数"},
            {TypeTotalNum, "总内容数"},
            {TypePageNavigation, "页导航"},
            {TypePageSelect, "页跳转"}
        };

        //对“翻页项”（pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public static string ParseElement(string stlElement, PageInfo pageInfo, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, EContextType contextType)
        {
            var parsedContent = string.Empty;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

                if (!StringUtils.EqualsIgnoreCase(stlElementInfo.Name, ElementName)) return string.Empty;

                var text = string.Empty;
                var type = string.Empty;
                var linkClass = string.Empty;
                var textClass = string.Empty;
                var listNum = 9;
                var listEllipsis = "...";
                var hasLr = true;
                //string lrStr = string.Empty;
                var lStr = string.Empty;
                var rStr = string.Empty;
                var alwaysA = true;
                var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();

                foreach (var name in stlElementInfo.Attributes.AllKeys)
                {
                    var value = stlElementInfo.Attributes[name];

                    if (StringUtils.EqualsIgnoreCase(name, Type))
                    {
                        type = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, Text))
                    {
                        text = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, ListNum))
                    {
                        listNum = TranslateUtils.ToInt(value, 9);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, ListEllipsis))
                    {
                        listEllipsis = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, LinkClass))
                    {
                        linkClass = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, TextClass))
                    {
                        textClass = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, HasLr))
                    {
                        hasLr = TranslateUtils.ToBool(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, LStr))
                    {
                        lStr = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, RStr))
                    {
                        rStr = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, AlwaysA))
                    {
                        alwaysA = TranslateUtils.ToBool(value);
                    }
                    else
                    {
                        attributes[name] = value;
                    }
                }

                StlParserUtility.GetYesNo(stlElementInfo.InnerHtml, out var successTemplateString, out var failureTemplateString);
                if (!string.IsNullOrEmpty(stlElementInfo.InnerHtml) && string.IsNullOrEmpty(failureTemplateString))
                {
                    failureTemplateString = successTemplateString;
                }

                //以下三个对象仅isChannelPage=true时需要
                ChannelInfo nodeInfo = null;

                string pageUrl;
                if (contextType == EContextType.Channel)
                {
                    nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
                    pageUrl = PagerUtility.GetUrlInChannelPage(type, pageInfo.SiteInfo, nodeInfo, 0, currentPageIndex, pageCount, pageInfo.IsLocal);
                }
                else
                {
                    pageUrl = PagerUtility.GetUrlInContentPage(type, pageInfo.SiteInfo, channelId, contentId, 0, currentPageIndex, pageCount, pageInfo.IsLocal);
                }

                var isActive = false;
                var isAddSpan = false;

                if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage) || StringUtils.EqualsIgnoreCase(type, TypeLastPage) || StringUtils.EqualsIgnoreCase(type, TypePreviousPage) || StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "首页";
                        }
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isActive = true;
                        }
                        else
                        {
                            pageUrl = PageUtils.UnclickedUrl;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeLastPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "末页";
                        }
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isActive = true;
                        }
                        else
                        {
                            pageUrl = PageUtils.UnclickedUrl;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypePreviousPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "上一页";
                        }
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isActive = true;
                        }
                        else
                        {
                            pageUrl = PageUtils.UnclickedUrl;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                    {
                        if (text.Equals(string.Empty))
                        {
                            text = "下一页";
                        }
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isActive = true;
                        }
                        else
                        {
                            pageUrl = PageUtils.UnclickedUrl;
                        }
                    }

                    if (isActive)
                    {
                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            parsedContent = GetParsedContent(successTemplateString, pageUrl, Convert.ToString(currentPageIndex + 1), pageInfo);
                        }
                        else
                        {
                            var pageHyperLink = new HyperLink();
                            ControlUtils.AddAttributesIfNotExists(pageHyperLink, attributes);
                            pageHyperLink.Text = text;
                            if (!string.IsNullOrEmpty(linkClass))
                            {
                                pageHyperLink.CssClass = linkClass;
                            }
                            pageHyperLink.NavigateUrl = pageUrl;
                            parsedContent = ControlUtils.GetControlRenderHtml(pageHyperLink);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(failureTemplateString))
                        {
                            parsedContent = GetParsedContent(failureTemplateString, pageUrl, Convert.ToString(currentPageIndex + 1), pageInfo);
                        }
                        else
                        {
                            isAddSpan = true;
                            parsedContent = text;
                        }
                    }
                }

                else if (StringUtils.EqualsIgnoreCase(type, TypeCurrentPageIndex))//当前页索引
                {
                    var currentPageHtml = text + Convert.ToString(currentPageIndex + 1);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeTotalPageNum))//总页数
                {
                    var currentPageHtml = text + Convert.ToString(pageCount);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeTotalNum))//总内容数
                {
                    isAddSpan = true;
                    parsedContent = text + Convert.ToString(totalNum);
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypePageNavigation))//页导航
                {
                    var leftText = "[";
                    var rightText = "]";
                    if (hasLr)
                    {
                        if (!string.IsNullOrEmpty(lStr) && !string.IsNullOrEmpty(rStr))
                        {
                            leftText = lStr;
                            rightText = rStr;
                        }
                        else if (!string.IsNullOrEmpty(lStr))
                        {
                            leftText = rightText = lStr;
                        }
                        else if (!string.IsNullOrEmpty(rStr))
                        {
                            leftText = rightText = rStr;
                        }
                    }
                    else if (!hasLr)
                    {
                        leftText = rightText = string.Empty;
                    }

                    var pageBuilder = new StringBuilder();

                    var pageLength = listNum;
                    var pageHalf = Convert.ToInt32(listNum / 2);

                    var index = currentPageIndex + 1;
                    var totalPage = currentPageIndex + pageLength;
                    if (totalPage > pageCount)
                    {
                        if (index + pageHalf < pageCount)
                        {
                            index = currentPageIndex + 1 - pageHalf;
                            if (index <= 0)
                            {
                                index = 1;
                                totalPage = pageCount;
                            }
                            else
                            {
                                totalPage = currentPageIndex + 1 + pageHalf;
                            }
                        }
                        else
                        {
                            index = pageCount - pageLength > 0 ? pageCount - pageLength + 1 : 1;
                            totalPage = pageCount;
                        }
                    }
                    else
                    {
                        index = currentPageIndex + 1 - pageHalf;
                        if (index <= 0)
                        {
                            index = 1;
                            totalPage = pageLength;
                        }
                        else
                        {
                            totalPage = index + pageLength - 1;
                        }
                    }

                    //pre ellipsis
                    if (index + pageLength < currentPageIndex + 1 && !string.IsNullOrEmpty(listEllipsis))
                    {
                        pageUrl = contextType == EContextType.Channel ? PagerUtility.GetUrlInChannelPage(type, pageInfo.SiteInfo, nodeInfo, index, currentPageIndex, pageCount, pageInfo.IsLocal) : PagerUtility.GetUrlInContentPage(type, pageInfo.SiteInfo, channelId, contentId, index, currentPageIndex, pageCount, pageInfo.IsLocal);

                        pageBuilder.Append(!string.IsNullOrEmpty(successTemplateString)
                            ? GetParsedContent(successTemplateString, pageUrl, listEllipsis, pageInfo)
                            : $@"<a href=""{pageUrl}"" {TranslateUtils.ToAttributesString(attributes)}>{listEllipsis}</a>");
                    }

                    for (; index <= totalPage; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            pageUrl = contextType == EContextType.Channel ? PagerUtility.GetUrlInChannelPage(type, pageInfo.SiteInfo, nodeInfo, index, currentPageIndex, pageCount, pageInfo.IsLocal) : PagerUtility.GetUrlInContentPage(type, pageInfo.SiteInfo, channelId, contentId, index, currentPageIndex, pageCount, pageInfo.IsLocal);

                            if (!string.IsNullOrEmpty(successTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                var pageHyperLink = new HyperLink
                                {
                                    NavigateUrl = pageUrl,
                                    Text = $"{leftText}{index}{rightText}"
                                };
                                if (!string.IsNullOrEmpty(linkClass))
                                {
                                    pageHyperLink.CssClass = linkClass;
                                }
                                pageBuilder.Append(ControlUtils.GetControlRenderHtml(pageHyperLink) + "&nbsp;");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(failureTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(failureTemplateString, pageUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                isAddSpan = true;
                                pageBuilder.Append(!alwaysA
                                    ? $"{leftText}{index}{rightText}&nbsp;"
                                    : $"<a href='javascript:void(0);'>{leftText}{index}{rightText}</a>&nbsp;");
                            }
                        }
                    }

                    //pre ellipsis
                    if (index < pageCount && !string.IsNullOrEmpty(listEllipsis))
                    {
                        pageUrl = contextType == EContextType.Channel ? PagerUtility.GetUrlInChannelPage(type, pageInfo.SiteInfo, nodeInfo, index, currentPageIndex, pageCount, pageInfo.IsLocal) : PagerUtility.GetUrlInContentPage(type, pageInfo.SiteInfo, channelId, contentId, index, currentPageIndex, pageCount, pageInfo.IsLocal);

                        pageBuilder.Append(!string.IsNullOrEmpty(successTemplateString)
                            ? GetParsedContent(successTemplateString, pageUrl, listEllipsis, pageInfo)
                            : $@"<a href=""{pageUrl}"" {TranslateUtils.ToAttributesString(attributes)}>{listEllipsis}</a>");
                    }

                    parsedContent = text + pageBuilder;
                }
                else if (type.ToLower().Equals(TypePageSelect.ToLower()))//页跳转
                {
                    var selectControl = new HtmlSelect();
                    if (!string.IsNullOrEmpty(textClass))
                    {
                        selectControl.Attributes.Add("class", textClass);
                    }
                    foreach (string key in attributes.Keys)
                    {
                        selectControl.Attributes[key] = attributes[key];
                    }

                    var uniqueId = "PageSelect_" + pageInfo.UniqueId;
                    selectControl.ID = uniqueId;

                    string scriptHtml =
                        $"<script language=\"JavaScript\">function {uniqueId}_jumpMenu(targ,selObj,restore){{eval(targ+\".location=\'\"+selObj.options[selObj.selectedIndex].value+\"\'\");if (restore) selObj.selectedIndex=0;}}</script>";
                    selectControl.Attributes.Add("onChange", $"{uniqueId}_jumpMenu('self',this,0)");

                    for (var index = 1; index <= pageCount; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            pageUrl = contextType == EContextType.Channel ? PagerUtility.GetUrlInChannelPage(type, pageInfo.SiteInfo, nodeInfo, index, currentPageIndex, pageCount, pageInfo.IsLocal) : PagerUtility.GetUrlInContentPage(type, pageInfo.SiteInfo, channelId, contentId, index, currentPageIndex, pageCount, pageInfo.IsLocal);

                            var listitem = new ListItem(index.ToString(), pageUrl);
                            selectControl.Items.Add(listitem);
                        }
                        else
                        {
                            var listitem = new ListItem(index.ToString(), string.Empty) {Selected = true};
                            selectControl.Items.Add(listitem);
                        }
                    }

                    parsedContent = scriptHtml + ControlUtils.GetControlRenderHtml(selectControl);
                }

                if (isAddSpan && !string.IsNullOrEmpty(textClass))
                {
                    parsedContent = $@"<span class=""{textClass}"">{parsedContent}</span>";
                }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(pageInfo, ElementName, stlElement, ex);
            }

            return parsedContent;

            //return parsedContent;
        }

        public static string ParseEntity(string stlEntity, PageInfo pageInfo, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent, EContextType contextType)
        {
            var parsedContent = string.Empty;
            try
            {
                var type = stlEntity.Substring(stlEntity.IndexOf(".", StringComparison.Ordinal) + 1);
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.TrimEnd('}').Trim();
                }
                var isHyperlink = false;

                //以下三个对象仅isChannelPage=true时需要

                string pageUrl;

                if (contextType == EContextType.Channel)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
                    pageUrl = PagerUtility.GetUrlInChannelPage(type, pageInfo.SiteInfo, nodeInfo, 0, currentPageIndex, pageCount, pageInfo.IsLocal);
                }
                else
                {
                    pageUrl = PagerUtility.GetUrlInContentPage(type, pageInfo.SiteInfo, channelId, contentId, 0, currentPageIndex, pageCount, pageInfo.IsLocal);
                }

                if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage) || StringUtils.EqualsIgnoreCase(type, TypeLastPage) || StringUtils.EqualsIgnoreCase(type, TypePreviousPage) || StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeLastPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypePreviousPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }

                    parsedContent = isHyperlink ? pageUrl : PageUtils.UnclickedUrl;
                }
                else if (type.ToLower().Equals(TypeCurrentPageIndex.ToLower()))//当前页索引
                {
                    parsedContent = Convert.ToString(currentPageIndex + 1);
                }
                else if (type.ToLower().Equals(TypeTotalPageNum.ToLower()))//总页数
                {
                    parsedContent = Convert.ToString(pageCount);
                }
                else if (type.ToLower().Equals(TypeTotalNum.ToLower()))//总内容数
                {
                    parsedContent = Convert.ToString(totalNum);
                }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(pageInfo, ElementName, stlEntity, ex);
            }

            return parsedContent;
        }

        public static string ParseElementInSearchPage(string stlElement, PageInfo pageInfo, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            var parsedContent = string.Empty;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

                if (!StringUtils.EqualsIgnoreCase(stlElementInfo.Name, ElementName)) return string.Empty;

                var text = string.Empty;
                var type = string.Empty;
                var linkClass = string.Empty;
                var textClass = string.Empty;
                var listNum = 9;
                var listEllipsis = "...";
                var hasLr = true;
                //string lrStr = string.Empty;
                var lStr = string.Empty;
                var rStr = string.Empty;
                var alwaysA = true;
                var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();

                foreach (var name in stlElementInfo.Attributes.AllKeys)
                {
                    var value = stlElementInfo.Attributes[name];

                    if (StringUtils.EqualsIgnoreCase(name, Type))
                    {
                        type = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, Text))
                    {
                        text = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, ListNum))
                    {
                        listNum = TranslateUtils.ToInt(value, 9);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, ListEllipsis))
                    {
                        listEllipsis = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, LinkClass))
                    {
                        linkClass = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, TextClass))
                    {
                        textClass = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, HasLr))
                    {
                        hasLr = TranslateUtils.ToBool(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, LStr))
                    {
                        lStr = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, RStr))
                    {
                        rStr = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, AlwaysA))
                    {
                        alwaysA = TranslateUtils.ToBool(value);
                    }
                    else
                    {
                        attributes[name] = value;
                    }
                }

                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;

                if (!string.IsNullOrEmpty(stlElementInfo.InnerHtml))
                {
                    var stlElementList = StlParserUtility.GetStlElementList(stlElementInfo.InnerHtml);
                    if (stlElementList.Count > 0)
                    {
                        foreach (var theStlElement in stlElementList)
                        {
                            if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                            {
                                successTemplateString = StlParserUtility.GetInnerHtml(theStlElement);
                            }
                            else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                            {
                                failureTemplateString = StlParserUtility.GetInnerHtml(theStlElement);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                    {
                        successTemplateString = failureTemplateString = stlElementInfo.InnerHtml;
                    }
                }

                var clickString = PagerUtility.GetClickStringInSearchPage(type, ajaxDivId, 0, currentPageIndex, pageCount);

                var isActive = false;
                var isAddSpan = false;

                if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage) || StringUtils.EqualsIgnoreCase(type, TypeLastPage) || StringUtils.EqualsIgnoreCase(type, TypePreviousPage) || StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "首页";
                        }
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isActive = true;
                        }
                        else
                        {
                            clickString = string.Empty;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeLastPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "末页";
                        }
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isActive = true;
                        }
                        else
                        {
                            clickString = string.Empty;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypePreviousPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "上一页";
                        }
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isActive = true;
                        }
                        else
                        {
                            clickString = string.Empty;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                    {
                        if (text.Equals(string.Empty))
                        {
                            text = "下一页";
                        }
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isActive = true;
                        }
                        else
                        {
                            clickString = string.Empty;
                        }
                    }

                    if (isActive)//当前页不为首页
                    {
                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            string pageUrl = $"javascript:{clickString}";
                            parsedContent = GetParsedContent(successTemplateString, pageUrl, Convert.ToString(currentPageIndex + 1), pageInfo);
                        }
                        else
                        {
                            var pageHyperLink = new HyperLink();
                            ControlUtils.AddAttributesIfNotExists(pageHyperLink, attributes);
                            pageHyperLink.NavigateUrl = PageUtils.UnclickedUrl;
                            pageHyperLink.Attributes.Add("onclick", clickString);
                            pageHyperLink.Text = text;
                            if (!string.IsNullOrEmpty(linkClass))
                            {
                                pageHyperLink.CssClass = linkClass;
                            }
                            parsedContent = ControlUtils.GetControlRenderHtml(pageHyperLink);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(failureTemplateString))
                        {
                            parsedContent = GetParsedContent(failureTemplateString, PageUtils.UnclickedUrl, Convert.ToString(currentPageIndex + 1), pageInfo);
                        }
                        else
                        {
                            isAddSpan = true;
                            parsedContent = text;
                        }
                    }
                }

                else if (type.ToLower().Equals(TypeCurrentPageIndex.ToLower()))//当前页索引
                {
                    var currentPageHtml = text + Convert.ToString(currentPageIndex + 1);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(TypeTotalPageNum.ToLower()))//总页数
                {
                    var currentPageHtml = text + Convert.ToString(pageCount);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(TypeTotalNum.ToLower()))//总内容数
                {
                    isAddSpan = true;
                    parsedContent = text + Convert.ToString(totalNum);
                }
                else if (type.ToLower().Equals(TypePageNavigation.ToLower()))//页导航
                {
                    var leftText = "[";
                    var rightText = "]";
                    if (hasLr)
                    {
                        if (!string.IsNullOrEmpty(lStr) && !string.IsNullOrEmpty(rStr))
                        {
                            leftText = lStr;
                            rightText = rStr;
                        }
                        else if (!string.IsNullOrEmpty(lStr))
                        {
                            leftText = rightText = lStr;
                        }
                        else if (!string.IsNullOrEmpty(rStr))
                        {
                            leftText = rightText = rStr;
                        }
                    }
                    else if (!hasLr)
                    {
                        leftText = rightText = string.Empty;
                    }
                    var pageBuilder = new StringBuilder();

                    var pageLength = listNum;
                    var pageHalf = Convert.ToInt32(listNum / 2);

                    var index = currentPageIndex + 1;
                    var totalPage = currentPageIndex + pageLength;
                    if (totalPage > pageCount)
                    {
                        if (index + pageHalf < pageCount)
                        {
                            index = currentPageIndex + 1 - pageHalf;
                            if (index <= 0)
                            {
                                index = 1;
                                totalPage = pageCount;
                            }
                            else
                            {
                                totalPage = currentPageIndex + 1 + pageHalf;
                            }
                        }
                        else
                        {
                            index = pageCount - pageLength > 0 ? pageCount - pageLength + 1 : 1;
                            totalPage = pageCount;
                        }
                    }
                    else
                    {
                        index = currentPageIndex + 1 - pageHalf;
                        if (index <= 0)
                        {
                            index = 1;
                            totalPage = pageLength;
                        }
                        else
                        {
                            totalPage = index + pageLength - 1;
                        }
                    }

                    //pre ellipsis
                    if (index + pageLength < currentPageIndex + 1 && !string.IsNullOrEmpty(listEllipsis))
                    {
                        clickString = PagerUtility.GetClickStringInSearchPage(type, ajaxDivId, index, currentPageIndex, pageCount);

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            string pageUrl = $"javascript:{clickString}";
                            pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, listEllipsis, pageInfo));
                        }
                        else
                        {
                            pageBuilder.Append(
                                $@"<a href=""{PageUtils.UnclickedUrl}"" onclick=""{clickString}"" {TranslateUtils
                                    .ToAttributesString(attributes)}>{listEllipsis}</a>");
                        }
                    }

                    for (; index <= totalPage; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            clickString = PagerUtility.GetClickStringInSearchPage(type, ajaxDivId, index, currentPageIndex, pageCount);
                            if (!string.IsNullOrEmpty(successTemplateString))
                            {
                                string pageUrl = $"javascript:{clickString}";
                                pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                var pageHyperLink = new HyperLink {NavigateUrl = PageUtils.UnclickedUrl};
                                pageHyperLink.Attributes.Add("onclick", clickString);
                                pageHyperLink.Text = $"{leftText}{index}{rightText}";
                                if (!string.IsNullOrEmpty(linkClass))
                                {
                                    pageHyperLink.CssClass = linkClass;
                                }
                                pageBuilder.Append(ControlUtils.GetControlRenderHtml(pageHyperLink) + "&nbsp;");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(failureTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(failureTemplateString, PageUtils.UnclickedUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                isAddSpan = true;
                                pageBuilder.Append(!alwaysA
                                    ? $"{leftText}{index}{rightText}&nbsp;"
                                    : $"<a href='javascript:void(0);'>{leftText}{index}{rightText}</a>&nbsp;");
                            }
                        }
                    }

                    //pre ellipsis
                    if (index < pageCount && !string.IsNullOrEmpty(listEllipsis))
                    {
                        clickString = PagerUtility.GetClickStringInSearchPage(type, ajaxDivId, index, currentPageIndex, pageCount);

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            string pageUrl = $"javascript:{clickString}";
                            pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, listEllipsis, pageInfo));
                        }
                        else
                        {
                            pageBuilder.Append(
                                $@"<a href=""{PageUtils.UnclickedUrl}"" onclick=""{clickString}"" {TranslateUtils
                                    .ToAttributesString(attributes)}>{listEllipsis}</a>");
                        }
                    }

                    parsedContent = text + pageBuilder;
                }
                else if (type.ToLower().Equals(TypePageSelect.ToLower()))//页跳转
                {
                    var selectControl = new HtmlSelect();
                    if (!string.IsNullOrEmpty(textClass))
                    {
                        selectControl.Attributes.Add("class", textClass);
                    }
                    foreach (string key in attributes.Keys)
                    {
                        selectControl.Attributes[key] = attributes[key];
                    }
                    var uniqueId = "PageSelect_" + pageInfo.UniqueId;
                    selectControl.ID = uniqueId;
                    selectControl.Attributes.Add("onChange", clickString);
                    selectControl.Attributes.Add("style", "display:none");
                    for (var index = 1; index <= pageCount; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            var listitem = new ListItem(index.ToString(), $"{index - 1}");
                            selectControl.Items.Add(listitem);
                        }
                        else
                        {
                            var listitem = new ListItem(index.ToString(), string.Empty) {Selected = true};
                            selectControl.Items.Add(listitem);
                        }
                    }

                    parsedContent = ControlUtils.GetControlRenderHtml(selectControl);
                }

                if (isAddSpan && !string.IsNullOrEmpty(textClass))
                {
                    parsedContent = $@"<span class=""{textClass}"">{parsedContent}</span>";
                }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(pageInfo, ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static string ParseEntityInSearchPage(string stlEntity, PageInfo pageInfo, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            var parsedContent = string.Empty;
            try
            {
                var type = stlEntity.Substring(stlEntity.IndexOf(".", StringComparison.Ordinal) + 1);
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.TrimEnd('}').Trim();
                }
                var isHyperlink = false;

                var clickString = PagerUtility.GetClickStringInSearchPage(type, ajaxDivId, 0, currentPageIndex, pageCount);

                if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage) || StringUtils.EqualsIgnoreCase(type, TypeLastPage) || StringUtils.EqualsIgnoreCase(type, TypePreviousPage) || StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeLastPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypePreviousPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }

                    parsedContent = isHyperlink ? $"javascript:{clickString}" : PageUtils.UnclickedUrl;
                }
                else if (type.ToLower().Equals(TypeCurrentPageIndex.ToLower()))//当前页索引
                {
                    parsedContent = Convert.ToString(currentPageIndex + 1);
                }
                else if (type.ToLower().Equals(TypeTotalPageNum.ToLower()))//总页数
                {
                    parsedContent = Convert.ToString(pageCount);
                }
                else if (type.ToLower().Equals(TypeTotalNum.ToLower()))//总内容数
                {
                    parsedContent = Convert.ToString(totalNum);
                }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(pageInfo, ElementName, stlEntity, ex);
            }

            return parsedContent;
        }


        public static string ParseElementInDynamicPage(string stlElement, PageInfo pageInfo, string pageUrl, int channelId, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            var parsedContent = string.Empty;
            try
            {
                var contextInfo = new ContextInfo(pageInfo);

                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

                if (!StringUtils.EqualsIgnoreCase(stlElementInfo.Name, ElementName)) return string.Empty;

                var text = string.Empty;
                var type = string.Empty;
                var linkClass = string.Empty;
                var textClass = string.Empty;
                var listNum = 9;
                var listEllipsis = "...";
                var hasLr = true;
                //string lrStr = string.Empty;
                var lStr = string.Empty;
                var rStr = string.Empty;
                var alwaysA = true;
                var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();

                foreach (var name in stlElementInfo.Attributes.AllKeys)
                {
                    var value = stlElementInfo.Attributes[name];

                    if (StringUtils.EqualsIgnoreCase(name, Type))
                    {
                        type = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, Text))
                    {
                        text = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, ListNum))
                    {
                        listNum = TranslateUtils.ToInt(value, 9);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, ListEllipsis))
                    {
                        listEllipsis = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, LinkClass))
                    {
                        linkClass = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, TextClass))
                    {
                        textClass = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, HasLr))
                    {
                        hasLr = TranslateUtils.ToBool(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, LStr))
                    {
                        lStr = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, RStr))
                    {
                        rStr = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, AlwaysA))
                    {
                        alwaysA = TranslateUtils.ToBool(value);
                    }
                    else
                    {
                        attributes[name] = value;
                    }
                }

                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;

                if (!string.IsNullOrEmpty(stlElementInfo.InnerHtml))
                {
                    var stlElementList = StlParserUtility.GetStlElementList(stlElementInfo.InnerHtml);
                    if (stlElementList.Count > 0)
                    {
                        foreach (var theStlElement in stlElementList)
                        {
                            if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                            {
                                successTemplateString = StlParserUtility.GetInnerHtml(theStlElement);
                            }
                            else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                            {
                                failureTemplateString = StlParserUtility.GetInnerHtml(theStlElement);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                    {
                        successTemplateString = failureTemplateString = stlElementInfo.InnerHtml;
                    }
                }

                var jsMethod = PagerUtility.GetJsMethodInDynamicPage(type, pageInfo.SiteInfo, contextInfo.ChannelId, contextInfo.ContentId, pageUrl, 0, currentPageIndex, pageCount, isPageRefresh, ajaxDivId, pageInfo.IsLocal);

                var isActive = false;
                var isAddSpan = false;

                if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage) || StringUtils.EqualsIgnoreCase(type, TypeLastPage) || StringUtils.EqualsIgnoreCase(type, TypePreviousPage) || StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "首页";
                        }
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isActive = true;
                        }
                        else
                        {
                            jsMethod = string.Empty;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeLastPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "末页";
                        }
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isActive = true;
                        }
                        else
                        {
                            jsMethod = string.Empty;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypePreviousPage))
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            text = "上一页";
                        }
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isActive = true;
                        }
                        else
                        {
                            jsMethod = string.Empty;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                    {
                        if (text.Equals(string.Empty))
                        {
                            text = "下一页";
                        }
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isActive = true;
                        }
                        else
                        {
                            jsMethod = string.Empty;
                        }
                    }

                    if (isActive)//当前页不为首页
                    {
                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            parsedContent = GetParsedContent(successTemplateString, $"javascript:{jsMethod}", Convert.ToString(currentPageIndex + 1), pageInfo);
                        }
                        else
                        {
                            var pageHyperLink = new HyperLink();
                            ControlUtils.AddAttributesIfNotExists(pageHyperLink, attributes);
                            pageHyperLink.NavigateUrl = PageUtils.UnclickedUrl;
                            pageHyperLink.Attributes.Add("onclick", jsMethod + ";return false;");
                            pageHyperLink.Text = text;
                            if (!string.IsNullOrEmpty(linkClass))
                            {
                                pageHyperLink.CssClass = linkClass;
                            }
                            parsedContent = ControlUtils.GetControlRenderHtml(pageHyperLink);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(failureTemplateString))
                        {
                            parsedContent = GetParsedContent(failureTemplateString, PageUtils.UnclickedUrl, Convert.ToString(currentPageIndex + 1), pageInfo);
                        }
                        else
                        {
                            isAddSpan = true;
                            parsedContent = text;
                        }
                    }
                }
                else if (type.ToLower().Equals(TypeCurrentPageIndex.ToLower()))//当前页索引
                {
                    var currentPageHtml = text + Convert.ToString(currentPageIndex + 1);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(TypeTotalPageNum.ToLower()))//总页数
                {
                    var currentPageHtml = text + Convert.ToString(pageCount);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(TypeTotalNum.ToLower()))//总内容数
                {
                    isAddSpan = true;
                    parsedContent = text + Convert.ToString(totalNum);
                }
                else if (type.ToLower().Equals(TypePageNavigation.ToLower()))//页导航
                {
                    var leftText = "[";
                    var rightText = "]";
                    if (hasLr)
                    {
                        if (!string.IsNullOrEmpty(lStr) && !string.IsNullOrEmpty(rStr))
                        {
                            leftText = lStr;
                            rightText = rStr;
                        }
                        else if (!string.IsNullOrEmpty(lStr))
                        {
                            leftText = rightText = lStr;
                        }
                        else if (!string.IsNullOrEmpty(rStr))
                        {
                            leftText = rightText = rStr;
                        }
                    }
                    else if (!hasLr)
                    {
                        leftText = rightText = string.Empty;
                    }
                    var pageBuilder = new StringBuilder();

                    var pageLength = listNum;
                    var pageHalf = Convert.ToInt32(listNum / 2);

                    var index = currentPageIndex + 1;
                    var totalPage = currentPageIndex + pageLength;
                    if (totalPage > pageCount)
                    {
                        if (index + pageHalf < pageCount)
                        {
                            index = currentPageIndex + 1 - pageHalf;
                            if (index <= 0)
                            {
                                index = 1;
                                totalPage = pageCount;
                            }
                            else
                            {
                                totalPage = currentPageIndex + 1 + pageHalf;
                            }
                        }
                        else
                        {
                            index = pageCount - pageLength > 0 ? pageCount - pageLength + 1 : 1;
                            totalPage = pageCount;
                        }
                    }
                    else
                    {
                        index = currentPageIndex + 1 - pageHalf;
                        if (index <= 0)
                        {
                            index = 1;
                            totalPage = pageLength;
                        }
                        else
                        {
                            totalPage = index + pageLength - 1;
                        }
                    }

                    //pre ellipsis
                    if (index + pageLength < currentPageIndex + 1 && !string.IsNullOrEmpty(listEllipsis))
                    {
                        jsMethod = PagerUtility.GetJsMethodInDynamicPage(type, pageInfo.SiteInfo, contextInfo.ChannelId, contextInfo.ContentId, pageUrl, index, currentPageIndex, pageCount, isPageRefresh, ajaxDivId, pageInfo.IsLocal);

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            pageBuilder = new StringBuilder(GetParsedContent(successTemplateString, $"javascript:{jsMethod}", listEllipsis, pageInfo));
                        }
                        else
                        {
                            pageBuilder.Append(
                                $@"<a href=""{PageUtils.UnclickedUrl}"" onclick=""{jsMethod};return false;"" {TranslateUtils
                                    .ToAttributesString(attributes)}>{listEllipsis}</a>");
                        }
                    }

                    for (; index <= totalPage; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            jsMethod = PagerUtility.GetJsMethodInDynamicPage(type, pageInfo.SiteInfo, contextInfo.ChannelId, contextInfo.ContentId, pageUrl, index, currentPageIndex, pageCount, isPageRefresh, ajaxDivId, pageInfo.IsLocal);

                            if (!string.IsNullOrEmpty(successTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(successTemplateString,
                                    $"javascript:{jsMethod}", Convert.ToString(index), pageInfo));
                            }
                            else
                            {
                                var pageHyperLink = new HyperLink {NavigateUrl = PageUtils.UnclickedUrl};
                                pageHyperLink.Attributes.Add("onclick", jsMethod + ";return false;");
                                pageHyperLink.Text = $"{leftText}{index}{rightText}";
                                if (!string.IsNullOrEmpty(linkClass))
                                {
                                    pageHyperLink.CssClass = linkClass;
                                }
                                pageBuilder.Append(ControlUtils.GetControlRenderHtml(pageHyperLink) + "&nbsp;");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(failureTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(failureTemplateString, PageUtils.UnclickedUrl, Convert.ToString(currentPageIndex + 1), pageInfo));
                            }
                            else
                            {
                                isAddSpan = true;
                                pageBuilder.Append(!alwaysA
                                    ? $"{leftText}{index}{rightText}&nbsp;"
                                    : $"<a href='javascript:void(0);'>{leftText}{index}{rightText}</a>&nbsp;");
                            }
                        }
                    }

                    //pre ellipsis
                    if (index < pageCount && !string.IsNullOrEmpty(listEllipsis))
                    {
                        jsMethod = PagerUtility.GetJsMethodInDynamicPage(type, pageInfo.SiteInfo, contextInfo.ChannelId, contextInfo.ContentId, pageUrl, index, currentPageIndex, pageCount, isPageRefresh, ajaxDivId, pageInfo.IsLocal);

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            pageBuilder = new StringBuilder(GetParsedContent(successTemplateString, $"javascript:{jsMethod}", Convert.ToString(currentPageIndex + 1), pageInfo));
                        }
                        else
                        {
                            pageBuilder.Append(
                                $@"<a href=""{PageUtils.UnclickedUrl}"" onclick=""{jsMethod};return false;"" {TranslateUtils
                                    .ToAttributesString(attributes)}>{listEllipsis}</a>");
                        }
                    }

                    parsedContent = text + pageBuilder;
                }
                else if (type.ToLower().Equals(TypePageSelect.ToLower()))//页跳转
                {
                    var selectControl = new HtmlSelect();
                    if (!string.IsNullOrEmpty(textClass))
                    {
                        selectControl.Attributes.Add("class", textClass);
                    }
                    foreach (string key in attributes.Keys)
                    {
                        selectControl.Attributes[key] = attributes[key];
                    }

                    selectControl.Attributes.Add("onChange", jsMethod + ";return false;");

                    for (var index = 1; index <= pageCount; index++)
                    {
                        var listitem = new ListItem(index.ToString(), index.ToString());
                        if (currentPageIndex + 1 == index)
                        {
                            listitem.Selected = true;
                        }
                        selectControl.Items.Add(listitem);
                    }

                    parsedContent = ControlUtils.GetControlRenderHtml(selectControl);
                }

                if (isAddSpan && !string.IsNullOrEmpty(textClass))
                {
                    parsedContent = $@"<span class=""{textClass}"">{parsedContent}</span>";
                }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(pageInfo, ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static string ParseEntityInDynamicPage(string stlEntity, PageInfo pageInfo, string pageUrl, int channelId, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            var parsedContent = string.Empty;
            try
            {
                var contextInfo = new ContextInfo(pageInfo);

                var type = stlEntity.Substring(stlEntity.IndexOf(".", StringComparison.Ordinal) + 1);
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.TrimEnd('}').Trim();
                }
                var isHyperlink = false;

                var jsMethod = PagerUtility.GetJsMethodInDynamicPage(type, pageInfo.SiteInfo, contextInfo.ChannelId, contextInfo.ContentId, pageUrl, 0, currentPageIndex, pageCount, isPageRefresh, ajaxDivId, pageInfo.IsLocal);

                if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage) || StringUtils.EqualsIgnoreCase(type, TypeLastPage) || StringUtils.EqualsIgnoreCase(type, TypePreviousPage) || StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, TypeFirstPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeLastPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypePreviousPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, TypeNextPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }

                    parsedContent = isHyperlink ? $"javascript:{jsMethod}" : PageUtils.UnclickedUrl;
                }
                else if (type.ToLower().Equals(TypeCurrentPageIndex.ToLower()))//当前页索引
                {
                    parsedContent = Convert.ToString(currentPageIndex + 1);
                }
                else if (type.ToLower().Equals(TypeTotalPageNum.ToLower()))//总页数
                {
                    parsedContent = Convert.ToString(pageCount);
                }
                else if (type.ToLower().Equals(TypeTotalNum.ToLower()))//总内容数
                {
                    parsedContent = Convert.ToString(totalNum);
                }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(pageInfo, ElementName, stlEntity, ex);
            }

            return parsedContent;
        }

        private static string GetParsedContent(string content, string pageUrl, string pageNum, PageInfo pageInfo)
        {
            var parsedContent = StringUtils.ReplaceIgnoreCase(content, "{Current.Url}", pageUrl);
            parsedContent = StringUtils.ReplaceIgnoreCase(parsedContent, "{Current.Num}", pageNum);

            var innerBuilder = new StringBuilder(parsedContent);
            var contextInfo = new ContextInfo(pageInfo);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            return innerBuilder.ToString();
        }
    }
}
