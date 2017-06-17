using System;
using System.Collections.Generic;
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
    [Stl(Usage = "下拉列表", Description = "通过 stl:select 标签在模板中显示栏目或内容下拉列表")]
    public class StlSelect
    {
        private StlSelect() { }
        public const string ElementName = "stl:select";

        public const string AttributeIsChannel = "isChannel";
        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeScope = "scope";
        public const string AttributeGroupChannel = "groupChannel";
        public const string AttributeGroupChannelNot = "groupChannelNot";
        public const string AttributeGroupContent = "groupContent";
        public const string AttributeGroupContentNot = "groupContentNot";
        public const string AttributeTags = "tags";
        public const string AttributeOrder = "order";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeTitleWordNum = "titleWordNum";
        public const string AttributeWhere = "where";
        public const string AttributeQueryString = "queryString";
        public const string AttributeIsTop = "isTop";
        public const string AttributeIsRecommend = "isRecommend";
        public const string AttributeIsHot = "isHot";
        public const string AttributeIsColor = "isColor";
        public const string AttributeTitle = "title";
        public const string AttributeOpenWin = "openWin";
        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeIsChannel, "是否显示栏目下拉列表"},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeScope, "选择的范围"},
            {AttributeGroupChannel, "指定显示的栏目组"},
            {AttributeGroupChannelNot, "指定不显示的栏目组"},
            {AttributeGroupContent, "指定显示的内容组"},
            {AttributeGroupContentNot, "指定不显示的内容组"},
            {AttributeTags, "指定标签"},
            {AttributeOrder, "排序"},
            {AttributeTotalNum, "显示数目"},
            {AttributeTitleWordNum, "标题文字数量"},
            {AttributeWhere, "获取下拉列表的条件判断"},
            {AttributeQueryString, "链接参数"},
            {AttributeIsTop, "仅显示置顶内容"},
            {AttributeIsRecommend, "仅显示推荐内容"},
            {AttributeIsHot, "仅显示热点内容"},
            {AttributeIsColor, "仅显示醒目内容"},
            {AttributeTitle, "下拉列表提示标题"},
            {AttributeOpenWin, "选择是否新窗口打开链接"},
            {AttributeIsDynamic, "是否动态显示"}
        };


        //对“下拉列表”（stl:select）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var selectControl = new HtmlSelect();

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

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsChannel))
                        {
                            isChannel = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
                        {
                            channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
                        {
                            channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeUpLevel))
                        {
                            upLevel = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTopLevel))
                        {
                            topLevel = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeScope))
                        {
                            scopeTypeString = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannel))
                        {
                            groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannelNot))
                        {
                            groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupContent))
                        {
                            groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupContentNot))
                        {
                            groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTags))
                        {
                            tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeOrder))
                        {
                            order = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTotalNum))
                        {
                            totalNum = TranslateUtils.ToInt(attr.Value, totalNum);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWhere))
                        {
                            where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeQueryString))
                        {
                            queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsTop))
                        {
                            isTopExists = true;
                            isTop = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsRecommend))
                        {
                            isRecommendExists = true;
                            isRecommend = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsHot))
                        {
                            isHotExists = true;
                            isHot = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsColor))
                        {
                            isColorExists = true;
                            isColor = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTitleWordNum))
                        {
                            titleWordNum = TranslateUtils.ToInt(attr.Value, titleWordNum);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTitle))
                        {
                            displayTitle = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeOpenWin))
                        {
                            openWin = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            selectControl.Attributes[attr.Name] = attr.Value;
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, selectControl, isChannel, channelIndex, channelName, upLevel, topLevel, scopeTypeString, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, order, totalNum, titleWordNum, where, queryString, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, displayTitle, openWin);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlSelect selectControl, bool isChannel, string channelIndex, string channelName, int upLevel, int topLevel, string scopeTypeString, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string order, int totalNum, int titleWordNum, string where, string queryString, bool isTop, bool isTopExists, bool isRecommend, bool isRecommendExists, bool isHot, bool isHotExists, bool isColor, bool isColorExists, string displayTitle, bool openWin)
        {
            EScopeType scopeType;
            if (!string.IsNullOrEmpty(scopeTypeString))
            {
                scopeType = EScopeTypeUtils.GetEnumType(scopeTypeString);
            }
            else
            {
                scopeType = isChannel ? EScopeType.Children : EScopeType.Self;
            }

            var orderByString = isChannel ? StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, order, ETableStyle.Channel, ETaxisType.OrderByTaxis) : StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, order, ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc);

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

            var uniqueId = "Select_" + pageInfo.UniqueId;
            selectControl.ID = uniqueId;

            string scriptHtml;
            if (openWin)
            {
                scriptHtml = $@"
<script language=""JavaScript"" type=""text/JavaScript"">
<!--
function {uniqueId}_jumpMenu(targ,selObj)
{"{"} //v3.0
window.open(selObj.options[selObj.selectedIndex].value);
selObj.selectedIndex=0;
{"}"}
//-->
</script>";
                selectControl.Attributes.Add("onChange", $"{uniqueId}_jumpMenu('parent',this)");
            }
            else
            {
                scriptHtml =
                    $"<script language=\"JavaScript\">function {uniqueId}_jumpMenu(targ,selObj,restore){{eval(targ+\".location=\'\"+selObj.options[selObj.selectedIndex].value+\"\'\");if (restore) selObj.selectedIndex=0;}}</script>";
                selectControl.Attributes.Add("onChange", $"{uniqueId}_jumpMenu('self',this,0)");
            }
            if (!string.IsNullOrEmpty(displayTitle))
            {
                var listitem = new ListItem(displayTitle, PageUtils.UnclickedUrl) {Selected = true};
                selectControl.Items.Add(listitem);
            }

            if (isChannel)
            {
                var nodeIdList = StlDataUtility.GetNodeIdList(pageInfo.PublishmentSystemId, channel.NodeId, groupContent, groupContentNot, orderByString, scopeType, groupChannel, groupChannelNot, false, false, totalNum, where);

                if (nodeIdList != null && nodeIdList.Count > 0)
                {
                    foreach (int nodeIdInSelect in nodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeIdInSelect);

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
                var dataSource = StlDataUtility.GetContentsDataSource(pageInfo.PublishmentSystemInfo, channelId, contextInfo.ContentId, groupContent, groupContentNot, tags, false, false, false, false, false, false, false, false, 1, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

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

            return scriptHtml + ControlUtils.GetControlRenderHtml(selectControl);
        }
    }
}
