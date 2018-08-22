using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "下拉列表", Description = "通过 stl:select 标签在模板中显示栏目或内容下拉列表")]
    public class StlSelect
    {
        private StlSelect() { }
        public const string ElementName = "stl:select";

        private static readonly Attr IsChannel = new Attr("isChannel", "是否显示栏目下拉列表");
        private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
        private static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
        private static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");
        private static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");
        private static readonly Attr Scope = new Attr("scope", "选择的范围");
        private static readonly Attr GroupChannel = new Attr("groupChannel", "指定显示的栏目组");
        private static readonly Attr GroupChannelNot = new Attr("groupChannelNot", "指定不显示的栏目组");
        private static readonly Attr GroupContent = new Attr("groupContent", "指定显示的内容组");
        private static readonly Attr GroupContentNot = new Attr("groupContentNot", "指定不显示的内容组");
        private static readonly Attr Tags = new Attr("tags", "指定标签");
        private static readonly Attr Order = new Attr("order", "排序");
        private static readonly Attr TotalNum = new Attr("totalNum", "显示数目");
        private static readonly Attr TitleWordNum = new Attr("titleWordNum", "标题文字数量");
        private static readonly Attr Where = new Attr("where", "获取下拉列表的条件判断");
        private static readonly Attr QueryString = new Attr("queryString", "链接参数");
        private static readonly Attr IsTop = new Attr("isTop", "仅显示置顶内容");
        private static readonly Attr IsRecommend = new Attr("isRecommend", "仅显示推荐内容");
        private static readonly Attr IsHot = new Attr("isHot", "仅显示热点内容");
        private static readonly Attr IsColor = new Attr("isColor", "仅显示醒目内容");
        private static readonly Attr Title = new Attr("title", "下拉列表提示标题");
        private static readonly Attr OpenWin = new Attr("openWin", "选择是否新窗口打开链接");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
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

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, IsChannel.Name))
                {
                    isChannel = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel.Name))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel.Name))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope.Name))
                {
                    scopeTypeString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannel.Name))
                {
                    groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannelNot.Name))
                {
                    groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContent.Name))
                {
                    groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContentNot.Name))
                {
                    groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Tags.Name))
                {
                    tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Order.Name))
                {
                    order = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum.Name))
                {
                    totalNum = TranslateUtils.ToInt(value, totalNum);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Where.Name))
                {
                    where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString.Name))
                {
                    queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsTop.Name))
                {
                    isTopExists = true;
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsRecommend.Name))
                {
                    isRecommendExists = true;
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHot.Name))
                {
                    isHotExists = true;
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsColor.Name))
                {
                    isColorExists = true;
                    isColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TitleWordNum.Name))
                {
                    titleWordNum = TranslateUtils.ToInt(value, titleWordNum);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Title.Name))
                {
                    displayTitle = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OpenWin.Name))
                {
                    openWin = TranslateUtils.ToBool(value);
                }
                else
                {
                    selectControl.Attributes[name] = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, selectControl, isChannel, channelIndex, channelName, upLevel, topLevel, scopeTypeString, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, order, totalNum, titleWordNum, where, queryString, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, displayTitle, openWin);
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

            var orderByString = isChannel ? StlDataUtility.GetChannelOrderByString(pageInfo.SiteId, order, ETaxisType.OrderByTaxis) : StlDataUtility.GetContentOrderByString(pageInfo.SiteId, order, ETaxisType.OrderByTaxisDesc);

            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, channelIndex, channelName);

            var channel = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);

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
                var channelIdList = StlDataUtility.GetChannelIdList(pageInfo.SiteId, channel.Id, orderByString, scopeType, groupChannel, groupChannelNot, false, false, totalNum, where);

                if (channelIdList != null && channelIdList.Count > 0)
                {
                    foreach (var channelIdInSelect in channelIdList)
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelIdInSelect);

                        if (nodeInfo != null)
                        {
                            var title = StringUtils.MaxLengthText(nodeInfo.ChannelName, titleWordNum);
                            var url = PageUtility.GetChannelUrl(pageInfo.SiteInfo, nodeInfo, pageInfo.IsLocal);
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
                var dataSource = StlDataUtility.GetContentsDataSource(pageInfo.SiteInfo, channelId, contextInfo.ContentId, groupContent, groupContentNot, tags, false, false, false, false, false, false, false, 1, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

                if (dataSource != null)
                {
                    foreach (DataRow dataItem in dataSource.Tables[0].Rows)
                    {
                        var contentInfo = new ContentInfo(dataItem);
                        if (contentInfo != null)
                        {
                            var title = StringUtils.MaxLengthText(contentInfo.Title, titleWordNum);
                            var url = PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal);
                            if (!string.IsNullOrEmpty(queryString))
                            {
                                url = PageUtils.AddQueryString(url, queryString);
                            }
                            var listitem = new ListItem(title, url);
                            selectControl.Items.Add(listitem);
                        }
                    }
                    //foreach (var dataItem in dataSource)
                    //{
                    //    var contentInfo = new BackgroundContentInfo(dataItem);
                    //    if (contentInfo != null)
                    //    {
                    //        var title = StringUtils.MaxLengthText(contentInfo.Title, titleWordNum);
                    //        var url = PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo);
                    //        if (!string.IsNullOrEmpty(queryString))
                    //        {
                    //            url = PageUtils.AddQueryString(url, queryString);
                    //        }
                    //        var listitem = new ListItem(title, url);
                    //        selectControl.Items.Add(listitem);
                    //    }
                    //}
                }
            }

            return scriptHtml + ControlUtils.GetControlRenderHtml(selectControl);
        }
    }
}
