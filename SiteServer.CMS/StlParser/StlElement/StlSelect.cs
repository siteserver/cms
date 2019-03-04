using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "下拉列表", Description = "通过 stl:select 标签在模板中显示栏目或内容下拉列表")]
    public class StlSelect
    {
        private StlSelect() { }
        public const string ElementName = "stl:select";

        [StlAttribute(Title = "是否显示栏目下拉列表")]
        private const string IsChannel = nameof(IsChannel);

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "选择的范围")]
        private const string Scope = nameof(Scope);

        [StlAttribute(Title = "指定显示的栏目组")]
        private const string GroupChannel = nameof(GroupChannel);

        [StlAttribute(Title = "指定不显示的栏目组")]
        private const string GroupChannelNot = nameof(GroupChannelNot);

        [StlAttribute(Title = "指定显示的内容组")]
        private const string GroupContent = nameof(GroupContent);

        [StlAttribute(Title = "指定不显示的内容组")]
        private const string GroupContentNot = nameof(GroupContentNot);

        [StlAttribute(Title = "指定标签")]
        private const string Tags = nameof(Tags);

        [StlAttribute(Title = "排序")]
        private const string Order = nameof(Order);

        [StlAttribute(Title = "显示数目")]
        private const string TotalNum = nameof(TotalNum);

        [StlAttribute(Title = "标题文字数量")]
        private const string TitleWordNum = nameof(TitleWordNum);

        [StlAttribute(Title = "获取下拉列表的条件判断")]
        private const string Where = nameof(Where);

        [StlAttribute(Title = "链接参数")]
        private const string QueryString = nameof(QueryString);

        [StlAttribute(Title = "仅显示置顶内容")]
        private const string IsTop = nameof(IsTop);

        [StlAttribute(Title = "仅显示推荐内容")]
        private const string IsRecommend = nameof(IsRecommend);

        [StlAttribute(Title = "仅显示热点内容")]
        private const string IsHot = nameof(IsHot);

        [StlAttribute(Title = "仅显示醒目内容")]
        private const string IsColor = nameof(IsColor);

        [StlAttribute(Title = "下拉列表提示标题")]
        private const string Title = nameof(Title);

        [StlAttribute(Title = "选择是否新窗口打开链接")]
        private const string OpenWin = nameof(OpenWin);

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

                if (StringUtils.EqualsIgnoreCase(name, IsChannel))
                {
                    isChannel = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope))
                {
                    scopeTypeString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannel))
                {
                    groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannelNot))
                {
                    groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContent))
                {
                    groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContentNot))
                {
                    groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Tags))
                {
                    tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Order))
                {
                    order = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value, totalNum);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Where))
                {
                    where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsTop))
                {
                    isTopExists = true;
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsRecommend))
                {
                    isRecommendExists = true;
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHot))
                {
                    isHotExists = true;
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsColor))
                {
                    isColorExists = true;
                    isColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TitleWordNum))
                {
                    titleWordNum = TranslateUtils.ToInt(value, titleWordNum);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Title))
                {
                    displayTitle = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OpenWin))
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
<script language=""javascript"" type=""text/javascript"">
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
                var minContentInfoList = StlDataUtility.GetMinContentInfoList(pageInfo.SiteInfo, channelId, contextInfo.ContentId, groupContent, groupContentNot, tags, false, false, false, false, false, false, false, 1, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

                if (minContentInfoList != null)
                {
                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, minContentInfo.ChannelId, minContentInfo.Id);
                        var title = StringUtils.MaxLengthText(contentInfo.Title, titleWordNum);
                        var url = PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, false);
                        if (!string.IsNullOrEmpty(queryString))
                        {
                            url = PageUtils.AddQueryString(url, queryString);
                        }
                        selectControl.Items.Add(new ListItem(title, url));
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
