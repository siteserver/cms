using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement.Inner;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlPageItem
    {
        private StlPageItem() { }
        public const string ElementName = "stl:pageitem";//翻页项

        public const string Attribute_Type = "type";					                //显示类型
        public const string Attribute_Text = "text";					                //显示的文字
        public const string Attribute_LinkClass = "linkclass";
        public const string Attribute_TextClass = "textclass";
        public const string Attribute_ListNum = "listnum";                              //页导航或页跳转显示链接数
        public const string Attribute_ListEllipsis = "listellipsis";                    //页导航或页跳转链接太多时显示的省略号

        public const string Attribute_HasLR = "haslr";                                  //页码导航是否包含左右字符
        //public const string Attribute_LRStr = "lrstr";                                  //页面左右字符
        public const string Attribute_LStr = "lstr";                                    //页面左字符
        public const string Attribute_RStr = "rstr";                                    //页面右字符
        public const string Attribute_AlwaysA = "alwaysa";                              //页码总是超链接，包括无连接时

        public const string Type_PreviousPage = "PreviousPage";				            //上一页
        public const string Type_NextPage = "NextPage";						            //下一页
        public const string Type_FirstPage = "FirstPage";						        //首页
        public const string Type_LastPage = "LastPage";						            //末页
        public const string Type_CurrentPageIndex = "CurrentPageIndex";		            //当前页索引
        public const string Type_TotalPageNum = "TotalPageNum";		                    //总页数
        public const string Type_TotalNum = "TotalNum";		                            //总内容数
        public const string Type_PageNavigation = "PageNavigation";			            //页导航
        public const string Type_PageSelect = "PageSelect";			                    //页跳转

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_Type, "显示类型");
                attributes.Add(Attribute_Text, "显示的文字");
                attributes.Add(Attribute_LinkClass, "链接CSS样式");
                attributes.Add(Attribute_TextClass, "文字CSS样式");
                attributes.Add(Attribute_ListNum, "页导航或页跳转显示链接数");
                attributes.Add(Attribute_ListEllipsis, "页导航或页跳转链接太多时显示的省略号");
                return attributes;
            }
        }


        //对“翻页项”（pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public static string ParseElement(string stlElement, PageInfo pageInfo, int nodeID, int contentID, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent, EContextType contextType)
        {
            var parsedContent = string.Empty;
            try
            {
                var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, isXmlContent);
                XmlNode node = xmlDocument.DocumentElement;
                node = node.FirstChild;
                var label = node.Name;
                if (!label.ToLower().Equals(ElementName)) return string.Empty;

                var ie = node.Attributes.GetEnumerator();
                var text = string.Empty;
                var type = string.Empty;
                var linkClass = string.Empty;
                var textClass = string.Empty;
                var listNum = 9;
                var listEllipsis = "...";
                var hasLR = true;
                //string lrStr = string.Empty;
                var lStr = string.Empty;
                var rStr = string.Empty;
                var alwaysA = true;
                var attributes = new StringDictionary();
                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Text))
                    {
                        text = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ListNum))
                    {
                        listNum = TranslateUtils.ToInt(attr.Value, 9);
                    }
                    else if (attributeName.Equals(Attribute_ListEllipsis))
                    {
                        listEllipsis = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_LinkClass))
                    {
                        linkClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_TextClass))
                    {
                        textClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_HasLR))
                    {
                        hasLR = TranslateUtils.ToBool(attr.Value);
                    }
                    //else if (attributeName.Equals(StlPageItem.Attribute_LRStr))
                    //{
                    //    lrStr = attr.Value;
                    //}
                    else if (attributeName.Equals(Attribute_LStr))
                    {
                        lStr = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_RStr))
                    {
                        rStr = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_AlwaysA))
                    {
                        alwaysA = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        attributes[attributeName] = attr.Value;
                    }
                }

                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;

                StlParserUtility.GetYesOrNoTemplateString(node, pageInfo, out successTemplateString, out failureTemplateString);
                if (!string.IsNullOrEmpty(node.InnerXml) && string.IsNullOrEmpty(failureTemplateString))
                {
                    failureTemplateString = successTemplateString;
                }

                //以下三个对象仅isChannelPage=true时需要
                NodeInfo nodeInfo = null;

                var pageUrl = string.Empty;
                if (contextType == EContextType.Channel)
                {
                    nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeID);
                    pageUrl = URLUtility.GetUrlInChannelPage(type, pageInfo.PublishmentSystemInfo, nodeInfo, 0, currentPageIndex, pageCount);
                }
                else
                {
                    pageUrl = URLUtility.GetUrlInContentPage(type, pageInfo.PublishmentSystemInfo, nodeID, contentID, 0, currentPageIndex, pageCount);
                }

                var isActive = false;
                var isAddSpan = false;

                if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage) || StringUtils.EqualsIgnoreCase(type, Type_LastPage) || StringUtils.EqualsIgnoreCase(type, Type_PreviousPage) || StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_LastPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_PreviousPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_NextPage))
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

                else if (type.ToLower().Equals(Type_CurrentPageIndex.ToLower()))//当前页索引
                {
                    var currentPageHtml = text + Convert.ToString(currentPageIndex + 1);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(Type_TotalPageNum.ToLower()))//总页数
                {
                    var currentPageHtml = text + Convert.ToString(pageCount);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(Type_TotalNum.ToLower()))//总内容数
                {
                    isAddSpan = true;
                    parsedContent = text + Convert.ToString(totalNum);
                }
                else if (type.ToLower().Equals(Type_PageNavigation.ToLower()))//页导航
                {
                    var leftText = "[";
                    var rightText = "]";
                    if (hasLR)
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
                    else if (!hasLR)
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
                            index = (currentPageIndex + 1) - pageHalf;
                            if (index <= 0)
                            {
                                index = 1;
                                totalPage = pageCount;
                            }
                            else
                            {
                                totalPage = (currentPageIndex + 1) + pageHalf;
                            }
                        }
                        else
                        {
                            index = (pageCount - pageLength) > 0 ? (pageCount - pageLength + 1) : 1;
                            totalPage = pageCount;
                        }
                    }
                    else
                    {
                        index = (currentPageIndex + 1) - pageHalf;
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
                        if (contextType == EContextType.Channel)
                        {
                            pageUrl = URLUtility.GetUrlInChannelPage(type, pageInfo.PublishmentSystemInfo, nodeInfo, index, currentPageIndex, pageCount);
                        }
                        else
                        {
                            pageUrl = URLUtility.GetUrlInContentPage(type, pageInfo.PublishmentSystemInfo, nodeID, contentID, index, currentPageIndex, pageCount);
                        }

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, listEllipsis, pageInfo));
                        }
                        else
                        {
                            pageBuilder.Append(
                                $@"<a href=""{pageUrl}"" {TranslateUtils.ToAttributesString(attributes)}>{listEllipsis}</a>");
                        }
                    }

                    for (; index <= totalPage; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            if (contextType == EContextType.Channel)
                            {
                                pageUrl = URLUtility.GetUrlInChannelPage(type, pageInfo.PublishmentSystemInfo, nodeInfo, index, currentPageIndex, pageCount);
                            }
                            else
                            {
                                pageUrl = URLUtility.GetUrlInContentPage(type, pageInfo.PublishmentSystemInfo, nodeID, contentID, index, currentPageIndex, pageCount);
                            }

                            if (!string.IsNullOrEmpty(successTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                var pageHyperLink = new HyperLink();
                                pageHyperLink.NavigateUrl = pageUrl;
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
                                pageBuilder.Append(GetParsedContent(failureTemplateString, pageUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                isAddSpan = true;
                                if (!alwaysA)
                                {
                                    pageBuilder.Append($"{leftText}{index}{rightText}&nbsp;");
                                }
                                else
                                {
                                    pageBuilder.Append(
                                        $"<a href='javascript:void(0);'>{leftText}{index}{rightText}</a>&nbsp;");
                                }
                            }
                        }
                    }

                    //pre ellipsis
                    if (index < pageCount && !string.IsNullOrEmpty(listEllipsis))
                    {
                        if (contextType == EContextType.Channel)
                        {
                            pageUrl = URLUtility.GetUrlInChannelPage(type, pageInfo.PublishmentSystemInfo, nodeInfo, index, currentPageIndex, pageCount);
                        }
                        else
                        {
                            pageUrl = URLUtility.GetUrlInContentPage(type, pageInfo.PublishmentSystemInfo, nodeID, contentID, index, currentPageIndex, pageCount);
                        }

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, listEllipsis, pageInfo));
                        }
                        else
                        {
                            pageBuilder.Append(
                                $@"<a href=""{pageUrl}"" {TranslateUtils.ToAttributesString(attributes)}>{listEllipsis}</a>");
                        }
                    }

                    parsedContent = text + pageBuilder;
                }
                else if (type.ToLower().Equals(Type_PageSelect.ToLower()))//页跳转
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

                    var uniqueID = "PageSelect_" + pageInfo.UniqueId;
                    selectControl.ID = uniqueID;

                    string scriptHtml =
                        $"<script language=\"JavaScript\">function {uniqueID}_jumpMenu(targ,selObj,restore){"{"}eval(targ+\".location='\"+selObj.options[selObj.selectedIndex].value+\"'\");if (restore) selObj.selectedIndex=0;{"}"}</script>";
                    selectControl.Attributes.Add("onChange", $"{uniqueID}_jumpMenu('self',this,0)");

                    for (var index = 1; index <= pageCount; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            if (contextType == EContextType.Channel)
                            {
                                pageUrl = URLUtility.GetUrlInChannelPage(type, pageInfo.PublishmentSystemInfo, nodeInfo, index, currentPageIndex, pageCount);
                            }
                            else
                            {
                                pageUrl = URLUtility.GetUrlInContentPage(type, pageInfo.PublishmentSystemInfo, nodeID, contentID, index, currentPageIndex, pageCount);
                            }

                            var listitem = new ListItem(index.ToString(), pageUrl);
                            selectControl.Items.Add(listitem);
                        }
                        else
                        {
                            var listitem = new ListItem(index.ToString(), string.Empty);
                            listitem.Selected = true;
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
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return StlParserUtility.GetBackHtml(parsedContent, pageInfo);

            //return parsedContent;
        }

        public static string ParseEntity(string stlEntity, PageInfo pageInfo, int nodeID, int contentID, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent, EContextType contextType)
        {
            var parsedContent = string.Empty;
            try
            {
                var type = stlEntity.Substring(stlEntity.IndexOf(".") + 1);
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.TrimEnd('}').Trim();
                }
                var isHyperlink = false;

                //以下三个对象仅isChannelPage=true时需要
                NodeInfo nodeInfo = null;

                var pageUrl = string.Empty;

                if (contextType == EContextType.Channel)
                {
                    nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeID);
                    pageUrl = URLUtility.GetUrlInChannelPage(type, pageInfo.PublishmentSystemInfo, nodeInfo, 0, currentPageIndex, pageCount);
                }
                else
                {
                    pageUrl = URLUtility.GetUrlInContentPage(type, pageInfo.PublishmentSystemInfo, nodeID, contentID, 0, currentPageIndex, pageCount);
                }

                if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage) || StringUtils.EqualsIgnoreCase(type, Type_LastPage) || StringUtils.EqualsIgnoreCase(type, Type_PreviousPage) || StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_LastPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_PreviousPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }

                    if (isHyperlink)//当前页不为首页
                    {
                        parsedContent = pageUrl;
                    }
                    else
                    {
                        parsedContent = PageUtils.UnclickedUrl;
                    }

                }
                else if (type.ToLower().Equals(Type_CurrentPageIndex.ToLower()))//当前页索引
                {
                    parsedContent = Convert.ToString(currentPageIndex + 1);
                }
                else if (type.ToLower().Equals(Type_TotalPageNum.ToLower()))//总页数
                {
                    parsedContent = Convert.ToString(pageCount);
                }
                else if (type.ToLower().Equals(Type_TotalNum.ToLower()))//总内容数
                {
                    parsedContent = Convert.ToString(totalNum);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseElementInSearchPage(string stlElement, PageInfo pageInfo, string ajaxDivID, int nodeID, int currentPageIndex, int pageCount, int totalNum)
        {
            var parsedContent = string.Empty;
            try
            {
                var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, true);
                XmlNode node = xmlDocument.DocumentElement;
                node = node.FirstChild;
                var label = node.Name;
                if (!label.ToLower().Equals(ElementName)) return string.Empty;

                var ie = node.Attributes.GetEnumerator();
                var text = string.Empty;
                var type = string.Empty;
                var linkClass = string.Empty;
                var textClass = string.Empty;
                var listNum = 9;
                var listEllipsis = "...";
                var hasLR = true;
                //string lrStr = string.Empty;
                var lStr = string.Empty;
                var rStr = string.Empty;
                var alwaysA = true;
                var attributes = new StringDictionary();
                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Text))
                    {
                        text = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ListNum))
                    {
                        listNum = TranslateUtils.ToInt(attr.Value, 9);
                    }
                    else if (attributeName.Equals(Attribute_ListEllipsis))
                    {
                        listEllipsis = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_LinkClass))
                    {
                        linkClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_TextClass))
                    {
                        textClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_HasLR))
                    {
                        hasLR = TranslateUtils.ToBool(attr.Value);
                    }
                    //else if (attributeName.Equals(StlPageItem.Attribute_LRStr))
                    //{
                    //    lrStr = attr.Value;
                    //}
                    else if (attributeName.Equals(Attribute_LStr))
                    {
                        lStr = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_RStr))
                    {
                        rStr = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_AlwaysA))
                    {
                        alwaysA = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        attributes[attributeName] = attr.Value;
                    }
                }

                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;

                if (!string.IsNullOrEmpty(node.InnerXml))
                {
                    var stlElementList = StlParserUtility.GetStlElementList(node.InnerXml);
                    if (stlElementList.Count > 0)
                    {
                        foreach (var theStlElement in stlElementList)
                        {
                            if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                            {
                                successTemplateString = StlParserUtility.GetInnerXml(theStlElement, true);
                            }
                            else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                            {
                                failureTemplateString = StlParserUtility.GetInnerXml(theStlElement, true);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                    {
                        successTemplateString = failureTemplateString = node.InnerXml;
                    }
                }

                var clickString = URLUtility.GetClickStringInSearchPage(type, ajaxDivID, 0, currentPageIndex, pageCount);

                var isActive = false;
                var isAddSpan = false;

                if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage) || StringUtils.EqualsIgnoreCase(type, Type_LastPage) || StringUtils.EqualsIgnoreCase(type, Type_PreviousPage) || StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_LastPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_PreviousPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_NextPage))
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

                else if (type.ToLower().Equals(Type_CurrentPageIndex.ToLower()))//当前页索引
                {
                    var currentPageHtml = text + Convert.ToString(currentPageIndex + 1);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(Type_TotalPageNum.ToLower()))//总页数
                {
                    var currentPageHtml = text + Convert.ToString(pageCount);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(Type_TotalNum.ToLower()))//总内容数
                {
                    isAddSpan = true;
                    parsedContent = text + Convert.ToString(totalNum);
                }
                else if (type.ToLower().Equals(Type_PageNavigation.ToLower()))//页导航
                {
                    var leftText = "[";
                    var rightText = "]";
                    if (hasLR)
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
                    else if (!hasLR)
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
                            index = (currentPageIndex + 1) - pageHalf;
                            if (index <= 0)
                            {
                                index = 1;
                                totalPage = pageCount;
                            }
                            else
                            {
                                totalPage = (currentPageIndex + 1) + pageHalf;
                            }
                        }
                        else
                        {
                            index = (pageCount - pageLength) > 0 ? (pageCount - pageLength + 1) : 1;
                            totalPage = pageCount;
                        }
                    }
                    else
                    {
                        index = (currentPageIndex + 1) - pageHalf;
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
                        clickString = URLUtility.GetClickStringInSearchPage(type, ajaxDivID, index, currentPageIndex, pageCount);

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
                            clickString = URLUtility.GetClickStringInSearchPage(type, ajaxDivID, index, currentPageIndex, pageCount);
                            if (!string.IsNullOrEmpty(successTemplateString))
                            {
                                string pageUrl = $"javascript:{clickString}";
                                pageBuilder.Append(GetParsedContent(successTemplateString, pageUrl, index.ToString(), pageInfo));
                            }
                            else
                            {
                                var pageHyperLink = new HyperLink();
                                pageHyperLink.NavigateUrl = PageUtils.UnclickedUrl;
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
                                if (!alwaysA)
                                {
                                    pageBuilder.Append($"{leftText}{index}{rightText}&nbsp;");
                                }
                                else
                                {
                                    pageBuilder.Append(
                                        $"<a href='javascript:void(0);'>{leftText}{index}{rightText}</a>&nbsp;");
                                }
                            }
                        }
                    }

                    //pre ellipsis
                    if (index < pageCount && !string.IsNullOrEmpty(listEllipsis))
                    {
                        clickString = URLUtility.GetClickStringInSearchPage(type, ajaxDivID, index, currentPageIndex, pageCount);

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
                else if (type.ToLower().Equals(Type_PageSelect.ToLower()))//页跳转
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
                    var uniqueID = "PageSelect_" + pageInfo.UniqueId;
                    selectControl.ID = uniqueID;
                    selectControl.Attributes.Add("onChange", clickString);
                    selectControl.Attributes.Add("style", "display:none");
                    for (var index = 1; index <= pageCount; index++)
                    {
                        if (currentPageIndex + 1 != index)
                        {
                            var listitem = new ListItem(index.ToString(), $"{(index - 1)}");
                            selectControl.Items.Add(listitem);
                        }
                        else
                        {
                            var listitem = new ListItem(index.ToString(), string.Empty);
                            listitem.Selected = true;
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
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseEntityInSearchPage(string stlEntity, PageInfo pageInfo, string ajaxDivID, int nodeID, int currentPageIndex, int pageCount, int totalNum)
        {
            var parsedContent = string.Empty;
            try
            {
                var type = stlEntity.Substring(stlEntity.IndexOf(".") + 1);
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.TrimEnd('}').Trim();
                }
                var isHyperlink = false;

                var clickString = URLUtility.GetClickStringInSearchPage(type, ajaxDivID, 0, currentPageIndex, pageCount);

                if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage) || StringUtils.EqualsIgnoreCase(type, Type_LastPage) || StringUtils.EqualsIgnoreCase(type, Type_PreviousPage) || StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_LastPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_PreviousPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }

                    if (isHyperlink)//当前页不为首页
                    {
                        parsedContent = $"javascript:{clickString}";
                    }
                    else
                    {
                        parsedContent = PageUtils.UnclickedUrl;
                    }
                }
                else if (type.ToLower().Equals(Type_CurrentPageIndex.ToLower()))//当前页索引
                {
                    parsedContent = Convert.ToString(currentPageIndex + 1);
                }
                else if (type.ToLower().Equals(Type_TotalPageNum.ToLower()))//总页数
                {
                    parsedContent = Convert.ToString(pageCount);
                }
                else if (type.ToLower().Equals(Type_TotalNum.ToLower()))//总内容数
                {
                    parsedContent = Convert.ToString(totalNum);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }


        public static string ParseElementInDynamicPage(string stlElement, PageInfo pageInfo, string pageUrl, int nodeID, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivID)
        {
            var parsedContent = string.Empty;
            try
            {
                var contextInfo = new ContextInfo(pageInfo);

                var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, true);
                XmlNode node = xmlDocument.DocumentElement;
                node = node.FirstChild;
                var label = node.Name;
                if (!label.ToLower().Equals(ElementName)) return string.Empty;

                var ie = node.Attributes.GetEnumerator();
                var text = string.Empty;
                var type = string.Empty;
                var linkClass = string.Empty;
                var textClass = string.Empty;
                var listNum = 9;
                var listEllipsis = "...";
                var hasLR = true;
                //string lrStr = string.Empty;
                var lStr = string.Empty;
                var rStr = string.Empty;
                var alwaysA = true;
                var attributes = new StringDictionary();
                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Text))
                    {
                        text = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ListNum))
                    {
                        listNum = TranslateUtils.ToInt(attr.Value, 9);
                    }
                    else if (attributeName.Equals(Attribute_ListEllipsis))
                    {
                        listEllipsis = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_LinkClass))
                    {
                        linkClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_TextClass))
                    {
                        textClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_HasLR))
                    {
                        hasLR = TranslateUtils.ToBool(attr.Value);
                    }
                    //else if (attributeName.Equals(StlPageItem.Attribute_LRStr))
                    //{
                    //    lrStr = attr.Value;
                    //}
                    else if (attributeName.Equals(Attribute_LStr))
                    {
                        lStr = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_RStr))
                    {
                        rStr = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_AlwaysA))
                    {
                        alwaysA = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        attributes[attributeName] = attr.Value;
                    }
                }

                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;

                if (!string.IsNullOrEmpty(node.InnerXml))
                {
                    var stlElementList = StlParserUtility.GetStlElementList(node.InnerXml);
                    if (stlElementList.Count > 0)
                    {
                        foreach (var theStlElement in stlElementList)
                        {
                            if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                            {
                                successTemplateString = StlParserUtility.GetInnerXml(theStlElement, true);
                            }
                            else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                            {
                                failureTemplateString = StlParserUtility.GetInnerXml(theStlElement, true);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                    {
                        successTemplateString = failureTemplateString = node.InnerXml;
                    }
                }

                var jsMethod = URLUtility.GetJsMethodInDynamicPage(type, pageInfo.PublishmentSystemInfo, contextInfo.ChannelID, contextInfo.ContentID, pageUrl, 0, currentPageIndex, pageCount, isPageRefresh, ajaxDivID);

                var isActive = false;
                var isAddSpan = false;

                if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage) || StringUtils.EqualsIgnoreCase(type, Type_LastPage) || StringUtils.EqualsIgnoreCase(type, Type_PreviousPage) || StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_LastPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_PreviousPage))
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
                    else if (StringUtils.EqualsIgnoreCase(type, Type_NextPage))
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
                else if (type.ToLower().Equals(Type_CurrentPageIndex.ToLower()))//当前页索引
                {
                    var currentPageHtml = text + Convert.ToString(currentPageIndex + 1);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(Type_TotalPageNum.ToLower()))//总页数
                {
                    var currentPageHtml = text + Convert.ToString(pageCount);
                    isAddSpan = true;
                    parsedContent = currentPageHtml;
                }
                else if (type.ToLower().Equals(Type_TotalNum.ToLower()))//总内容数
                {
                    isAddSpan = true;
                    parsedContent = text + Convert.ToString(totalNum);
                }
                else if (type.ToLower().Equals(Type_PageNavigation.ToLower()))//页导航
                {
                    var leftText = "[";
                    var rightText = "]";
                    if (hasLR)
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
                    else if (!hasLR)
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
                            index = (currentPageIndex + 1) - pageHalf;
                            if (index <= 0)
                            {
                                index = 1;
                                totalPage = pageCount;
                            }
                            else
                            {
                                totalPage = (currentPageIndex + 1) + pageHalf;
                            }
                        }
                        else
                        {
                            index = (pageCount - pageLength) > 0 ? (pageCount - pageLength + 1) : 1;
                            totalPage = pageCount;
                        }
                    }
                    else
                    {
                        index = (currentPageIndex + 1) - pageHalf;
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
                        jsMethod = URLUtility.GetJsMethodInDynamicPage(type, pageInfo.PublishmentSystemInfo, contextInfo.ChannelID, contextInfo.ContentID, pageUrl, index, currentPageIndex, pageCount, isPageRefresh, ajaxDivID);

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            parsedContent = GetParsedContent(successTemplateString, $"javascript:{jsMethod}", listEllipsis, pageInfo);
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
                            jsMethod = URLUtility.GetJsMethodInDynamicPage(type, pageInfo.PublishmentSystemInfo, contextInfo.ChannelID, contextInfo.ContentID, pageUrl, index, currentPageIndex, pageCount, isPageRefresh, ajaxDivID);

                            if (!string.IsNullOrEmpty(successTemplateString))
                            {
                                pageBuilder.Append(GetParsedContent(successTemplateString,
                                    $"javascript:{jsMethod}", Convert.ToString(index), pageInfo));
                            }
                            else
                            {
                                var pageHyperLink = new HyperLink();
                                pageHyperLink.NavigateUrl = PageUtils.UnclickedUrl;
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
                                if (!alwaysA)
                                    pageBuilder.Append($"{leftText}{index}{rightText}&nbsp;");
                                else
                                    pageBuilder.Append(
                                        $"<a href='javascript:void(0);'>{leftText}{index}{rightText}</a>&nbsp;");
                            }
                        }
                    }

                    //pre ellipsis
                    if (index < pageCount && !string.IsNullOrEmpty(listEllipsis))
                    {
                        jsMethod = URLUtility.GetJsMethodInDynamicPage(type, pageInfo.PublishmentSystemInfo, contextInfo.ChannelID, contextInfo.ContentID, pageUrl, index, currentPageIndex, pageCount, isPageRefresh, ajaxDivID);

                        if (!string.IsNullOrEmpty(successTemplateString))
                        {
                            parsedContent = GetParsedContent(successTemplateString, $"javascript:{jsMethod}", Convert.ToString(currentPageIndex + 1), pageInfo);
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
                else if (type.ToLower().Equals(Type_PageSelect.ToLower()))//页跳转
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
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseEntityInDynamicPage(string stlEntity, PageInfo pageInfo, string pageUrl, int nodeID, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivID)
        {
            var parsedContent = string.Empty;
            try
            {
                var contextInfo = new ContextInfo(pageInfo);

                var type = stlEntity.Substring(stlEntity.IndexOf(".") + 1);
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.TrimEnd('}').Trim();
                }
                var isHyperlink = false;

                var jsMethod = URLUtility.GetJsMethodInDynamicPage(type, pageInfo.PublishmentSystemInfo, contextInfo.ChannelID, contextInfo.ContentID, pageUrl, 0, currentPageIndex, pageCount, isPageRefresh, ajaxDivID);

                if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage) || StringUtils.EqualsIgnoreCase(type, Type_LastPage) || StringUtils.EqualsIgnoreCase(type, Type_PreviousPage) || StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                {
                    if (StringUtils.EqualsIgnoreCase(type, Type_FirstPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_LastPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_PreviousPage))
                    {
                        if (currentPageIndex != 0)//当前页不为首页
                        {
                            isHyperlink = true;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, Type_NextPage))
                    {
                        if (currentPageIndex + 1 != pageCount)//当前页不为末页
                        {
                            isHyperlink = true;
                        }
                    }

                    if (isHyperlink)//当前页不为首页
                    {
                        parsedContent = $"javascript:{jsMethod}";
                    }
                    else
                    {
                        parsedContent = PageUtils.UnclickedUrl;
                    }
                }
                else if (type.ToLower().Equals(Type_CurrentPageIndex.ToLower()))//当前页索引
                {
                    parsedContent = Convert.ToString(currentPageIndex + 1);
                }
                else if (type.ToLower().Equals(Type_TotalPageNum.ToLower()))//总页数
                {
                    parsedContent = Convert.ToString(pageCount);
                }
                else if (type.ToLower().Equals(Type_TotalNum.ToLower()))//总内容数
                {
                    parsedContent = Convert.ToString(totalNum);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
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
