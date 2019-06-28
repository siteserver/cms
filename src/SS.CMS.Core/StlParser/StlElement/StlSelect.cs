using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Enums;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
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

        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            var attributes = new NameValueCollection();
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
            var queryString = string.Empty;

            bool? isTop = null;
            bool? isRecommend = null;
            bool? isHot = null;
            bool? isColor = null;

            var displayTitle = string.Empty;
            var openWin = true;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, IsChannel))
                {
                    isChannel = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
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
                    groupChannel = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannelNot))
                {
                    groupChannelNot = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContent))
                {
                    groupContent = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContentNot))
                {
                    groupContentNot = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Tags))
                {
                    tags = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Order))
                {
                    order = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value, totalNum);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsTop))
                {
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsRecommend))
                {
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHot))
                {
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsColor))
                {
                    isColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TitleWordNum))
                {
                    titleWordNum = TranslateUtils.ToInt(value, titleWordNum);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Title))
                {
                    displayTitle = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OpenWin))
                {
                    openWin = TranslateUtils.ToBool(value);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseImplAsync(parseContext, attributes, isChannel, channelIndex, channelName, upLevel, topLevel, scopeTypeString, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, order, totalNum, titleWordNum, queryString, isTop, isRecommend, isHot, isColor, displayTitle, openWin);
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, NameValueCollection attributes, bool isChannel, string channelIndex, string channelName, int upLevel, int topLevel, string scopeTypeString, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string order, int totalNum, int titleWordNum, string queryString, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, string displayTitle, bool openWin)
        {
            ScopeType scopeType;
            if (!string.IsNullOrEmpty(scopeTypeString))
            {
                scopeType = ScopeType.Parse(scopeTypeString);
            }
            else
            {
                scopeType = isChannel ? ScopeType.Children : ScopeType.Self;
            }

            var orderByString = isChannel ? parseContext.GetChannelTaxisType(order, TaxisType.OrderByTaxis) : parseContext.GetContentTaxisType(parseContext.SiteId, order, TaxisType.OrderByTaxisDesc);

            var channelId = await parseContext.GetChannelIdByLevelAsync(parseContext.SiteId, parseContext.ChannelId, upLevel, topLevel);

            channelId = await parseContext.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(parseContext.SiteId, channelId, channelIndex, channelName);

            var channel = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, channelId);

            var uniqueId = "Select_" + parseContext.UniqueId;
            attributes["id"] = uniqueId;

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
                attributes["onchange"] = $"{uniqueId}_jumpMenu('parent',this)";
            }
            else
            {
                scriptHtml =
                    $"<script language=\"JavaScript\">function {uniqueId}_jumpMenu(targ,selObj,restore){{eval(targ+\".location=\'\"+selObj.options[selObj.selectedIndex].value+\"\'\");if (restore) selObj.selectedIndex=0;}}</script>";

                attributes["onchange"] = $"{uniqueId}_jumpMenu('self',this,0)";
            }

            var htmlBuilder = new StringBuilder();
            using (var htmlSelect = new Html.Select(htmlBuilder, attributes))
            {
                if (!string.IsNullOrEmpty(displayTitle))
                {
                    htmlSelect.AddOption(displayTitle, PageUtils.UnClickableUrl, true);
                }

                if (isChannel)
                {
                    var taxisType = parseContext.GetChannelTaxisType(order, TaxisType.OrderByTaxis);
                    var channelIdList = await parseContext.ChannelRepository.StlGetIdListByTotalNumAsync(parseContext.SiteId, channel.Id, taxisType, scopeType, groupChannel, groupChannelNot, null, totalNum);

                    if (channelIdList != null && channelIdList.Count > 0)
                    {
                        foreach (var channelIdInSelect in channelIdList)
                        {
                            var nodeInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, channelIdInSelect);

                            if (nodeInfo != null)
                            {
                                var title = StringUtils.MaxLengthText(nodeInfo.ChannelName, titleWordNum);
                                var url = await parseContext.UrlManager.GetChannelUrlAsync(parseContext.SiteInfo, nodeInfo, parseContext.IsLocal);
                                if (!string.IsNullOrEmpty(queryString))
                                {
                                    url = PageUtils.AddQueryString(url, queryString);
                                }

                                htmlSelect.AddOption(title, url);
                            }
                        }
                    }
                }
                else
                {
                    var minContentInfoList = await parseContext.GetMinContentInfoListAsync(parseContext.SiteInfo, channelId, parseContext.ContentId, groupContent, groupContentNot, tags, null, null, null, false, 1, totalNum, orderByString, isTop, isRecommend, isHot, isColor, scopeType, groupChannel, groupChannelNot, null);

                    if (minContentInfoList != null)
                    {
                        foreach (var minContentInfo in minContentInfoList)
                        {
                            var channelInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, minContentInfo.ChannelId);
                            var contentInfo = channelInfo.ContentRepository.GetContentInfo(parseContext.SiteInfo, channelInfo, minContentInfo.Id);
                            var title = StringUtils.MaxLengthText(contentInfo.Title, titleWordNum);
                            var url = await parseContext.UrlManager.GetContentUrlAsync(parseContext.SiteInfo, contentInfo, false);
                            if (!string.IsNullOrEmpty(queryString))
                            {
                                url = PageUtils.AddQueryString(url, queryString);
                            }

                            htmlSelect.AddOption(title, url);
                        }
                        //foreach (var dataItem in dataSource)
                        //{
                        //    var contentInfo = new BackgroundContentInfo(dataItem);
                        //    if (contentInfo != null)
                        //    {
                        //        var title = StringUtils.MaxLengthText(contentInfo.Title, titleWordNum);
                        //        var url = parseContext.UrlManager.GetContentUrl(parseContext.SiteInfo, contentInfo);
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
            }

            return scriptHtml + htmlBuilder;
        }
    }
}
