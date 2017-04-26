using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlSelect
    {
        private StlSelect() { }
        public const string ElementName = "stl:select";//栏目或内容下拉列表

        public const string Attribute_IsChannel = "ischannel";                  //是否显示栏目下拉列表

        public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
        public const string Attribute_ChannelName = "channelname";				//栏目名称
        public const string Attribute_UpLevel = "uplevel";					    //上级栏目的级别
        public const string Attribute_TopLevel = "toplevel";				    //从首页向下的栏目级别

        public const string Attribute_Scope = "scope";							//范围
        public const string Attribute_GroupChannel = "groupchannel";		    //指定显示的栏目组
        public const string Attribute_GroupChannelNot = "groupchannelnot";	    //指定不显示的栏目组
        public const string Attribute_GroupContent = "groupcontent";		    //指定显示的内容组
        public const string Attribute_GroupContentNot = "groupcontentnot";	    //指定不显示的内容组
        public const string Attribute_Tags = "tags";	                        //指定标签
        public const string Attribute_Order = "order";							//排序
        public const string Attribute_TotalNum = "totalnum";					//显示数目
        public const string Attribute_TitleWordNum = "titlewordnum";			//标题文字数量
        public const string Attribute_Where = "where";                          //获取下拉列表的条件判断
        public const string Attribute_QueryString = "querystring";              //链接参数

        public const string Attribute_IsTop = "istop";                       //仅显示置顶内容
        public const string Attribute_IsRecommend = "isrecommend";           //仅显示推荐内容
        public const string Attribute_IsHot = "ishot";                       //仅显示热点内容
        public const string Attribute_IsColor = "iscolor";                   //仅显示醒目内容

        public const string Attribute_Title = "title";							//下拉列表提示标题
        public const string Attribute_OpenWin = "openwin";						//选择是否新窗口打开链接
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_IsChannel, "是否显示栏目下拉列表");

                attributes.Add(Attribute_ChannelIndex, "栏目索引");
                attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_UpLevel, "上级栏目的级别");
                attributes.Add(Attribute_TopLevel, "从首页向下的栏目级别");
                attributes.Add(Attribute_Scope, "选择的范围");
                attributes.Add(Attribute_GroupChannel, "指定显示的栏目组");
                attributes.Add(Attribute_GroupChannelNot, "指定不显示的栏目组");
                attributes.Add(Attribute_GroupContent, "指定显示的内容组");
                attributes.Add(Attribute_GroupContentNot, "指定不显示的内容组");
                attributes.Add(Attribute_Tags, "指定标签");
                attributes.Add(Attribute_Order, "排序");
                attributes.Add(Attribute_TotalNum, "显示数目");
                attributes.Add(Attribute_TitleWordNum, "标题文字数量");
                attributes.Add(Attribute_Where, "获取下拉列表的条件判断");
                attributes.Add(Attribute_QueryString, "链接参数");

                attributes.Add(Attribute_IsTop, "仅显示置顶内容");
                attributes.Add(Attribute_IsRecommend, "仅显示推荐内容");
                attributes.Add(Attribute_IsHot, "仅显示热点内容");
                attributes.Add(Attribute_IsColor, "仅显示醒目内容");

                attributes.Add(Attribute_Title, "下拉列表提示标题");
                attributes.Add(Attribute_OpenWin, "选择是否新窗口打开链接");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                return attributes;
            }
        }


        //对“下拉列表”（stl:select）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var selectControl = new HtmlSelect();
                var ie = node.Attributes.GetEnumerator();

                var isChannel = true;
                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var upLevel = 0;
                var topLevel = -1;
                var scopeTypeString = string.Empty;
                var groupChannel = string.Empty;
                var groupChannelNot = string.Empty;
                var groupContent = string.Empty;
                var groupContentNot = string.Empty;
                var tags = string.Empty;
                var order = string.Empty;
                var totalNum = 0;
                var titleWordNum = 0;
                var where = string.Empty;
                var queryString = string.Empty;

                var isTop = false;
                var isTopExists = false;
                var isRecommend = false;
                var isRecommendExists = false;
                var isHot = false;
                var isHotExists = false;
                var isColor = false;
                var isColorExists = false;

                var displayTitle = string.Empty;
                var openWin = true;
                var isDynamic = false;

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_IsChannel))
                    {
                        isChannel = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_ChannelIndex))
                    {
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_ChannelName))
                    {
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_UpLevel))
                    {
                        upLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_TopLevel))
                    {
                        topLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Scope))
                    {
                        scopeTypeString = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_GroupChannel))
                    {
                        groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannelNot))
                    {
                        groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupContent))
                    {
                        groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupContentNot))
                    {
                        groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_Tags))
                    {
                        tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_Order))
                    {
                        order = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_TotalNum))
                    {
                        try
                        {
                            totalNum = int.Parse(attr.Value);
                        }
                        catch { }
                    }
                    else if (attributeName.Equals(Attribute_Where))
                    {
                        where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_QueryString))
                    {
                        queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_IsTop))
                    {
                        isTopExists = true;
                        isTop = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsRecommend))
                    {
                        isRecommendExists = true;
                        isRecommend = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsHot))
                    {
                        isHotExists = true;
                        isHot = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsColor))
                    {
                        isColorExists = true;
                        isColor = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_TitleWordNum))
                    {
                        try
                        {
                            titleWordNum = int.Parse(attr.Value);
                        }
                        catch { }
                    }
                    else if (attributeName.Equals(Attribute_Title))
                    {
                        displayTitle = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_OpenWin))
                    {
                        try
                        {
                            openWin = bool.Parse(attr.Value);
                        }
                        catch { }
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        selectControl.Attributes.Remove(attributeName);
                        selectControl.Attributes.Add(attributeName, attr.Value);
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, selectControl, isChannel, channelIndex, channelName, upLevel, topLevel, scopeTypeString, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, order, totalNum, titleWordNum, where, queryString, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, displayTitle, openWin);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlSelect selectControl, bool isChannel, string channelIndex, string channelName, int upLevel, int topLevel, string scopeTypeString, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string order, int totalNum, int titleWordNum, string where, string queryString, bool isTop, bool isTopExists, bool isRecommend, bool isRecommendExists, bool isHot, bool isHotExists, bool isColor, bool isColorExists, string displayTitle, bool openWin)
        {
            var parsedContent = string.Empty;

            EScopeType scopeType;
            if (!string.IsNullOrEmpty(scopeTypeString))
            {
                scopeType = EScopeTypeUtils.GetEnumType(scopeTypeString);
            }
            else
            {
                if (isChannel)
                {
                    scopeType = EScopeType.Children;
                }
                else
                {
                    scopeType = EScopeType.Self;
                }
            }

            string orderByString;
            if (isChannel)
            {
                orderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, order, ETableStyle.Channel, ETaxisType.OrderByTaxis);
            }
            else
            {
                orderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, order, ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc);
            }

            var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);

            channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

            var uniqueID = "Select_" + pageInfo.UniqueId;
            selectControl.ID = uniqueID;

            string scriptHtml;
            if (openWin)
            {
                scriptHtml = $@"
<script language=""JavaScript"" type=""text/JavaScript"">
<!--
function {uniqueID}_jumpMenu(targ,selObj)
{"{"} //v3.0
window.open(selObj.options[selObj.selectedIndex].value);
selObj.selectedIndex=0;
{"}"}
//-->
</script>";
                selectControl.Attributes.Add("onChange", $"{uniqueID}_jumpMenu('parent',this)");
            }
            else
            {
                scriptHtml =
                    $"<script language=\"JavaScript\">function {uniqueID}_jumpMenu(targ,selObj,restore){"{"}eval(targ+\".location='\"+selObj.options[selObj.selectedIndex].value+\"'\");if (restore) selObj.selectedIndex=0;{"}"}</script>";
                selectControl.Attributes.Add("onChange", $"{uniqueID}_jumpMenu('self',this,0)");
            }
            if (!string.IsNullOrEmpty(displayTitle))
            {
                var listitem = new ListItem(displayTitle, PageUtils.UnclickedUrl);
                listitem.Selected = true;
                selectControl.Items.Add(listitem);
            }

            if (isChannel)
            {
                var nodeIdList = StlDataUtility.GetNodeIdList(pageInfo.PublishmentSystemId, channel.NodeId, groupContent, groupContentNot, orderByString, scopeType, groupChannel, groupChannelNot, false, false, totalNum, where);

                if (nodeIdList != null && nodeIdList.Count > 0)
                {
                    foreach (int nodeIDInSelect in nodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeIDInSelect);

                        if (nodeInfo != null)
                        {
                            var title = StringUtils.MaxLengthText(nodeInfo.NodeName, titleWordNum);
                            var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, nodeInfo);
                            if (!string.IsNullOrEmpty(queryString))
                            {
                                url = PageUtils.AddQueryString(url, queryString);
                            }
                            var listitem = new ListItem(title, url);
                            selectControl.Items.Add(listitem);
                        }
                    }
                }
            }
            else
            {
                var dataSource = StlDataUtility.GetContentsDataSource(pageInfo.PublishmentSystemInfo, channelID, contextInfo.ContentID, groupContent, groupContentNot, tags, false, false, false, false, false, false, false, false, 1, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

                if (dataSource != null)
                {
                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (contentInfo != null)
                        {
                            var title = StringUtils.MaxLengthText(contentInfo.Title, titleWordNum);
                            var url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo);
                            if (!string.IsNullOrEmpty(queryString))
                            {
                                url = PageUtils.AddQueryString(url, queryString);
                            }
                            var listitem = new ListItem(title, url);
                            selectControl.Items.Add(listitem);
                        }
                    }
                }
            }

            parsedContent = scriptHtml + ControlUtils.GetControlRenderHtml(selectControl);

            return parsedContent;
        }
    }
}
