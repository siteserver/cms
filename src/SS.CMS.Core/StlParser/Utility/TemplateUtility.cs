using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.Utility
{
    public static class TemplateUtility
    {
        public static async Task<string> GetContentsItemTemplateStringAsync(ParseContext parseContext, string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId)
        {
            var container = Container.GetContainer(parseContext.PageInfo);
            //var contentInfo = new BackgroundContentInfo(container.ContentItem.DataItem);

            if (parseContext.PageInfo.ContentItems.Count == 0) return string.Empty;

            var contentItemInfo = parseContext.PageInfo.ContentItems.Peek();

            var channelInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(contentItemInfo.Value.ChannelId);
            var contentInfo = await channelInfo.ContentRepository.GetContentInfoAsync(contentItemInfo.Value.Id);

            var context = parseContext.Clone(EContextType.Content);
            context.Container = container;
            context.ContainerClientId = containerClientId;
            context.ChannelId = contentInfo.ChannelId;
            context.ContentId = contentInfo.Id;
            context.ContentInfo = contentInfo;

            var preSiteInfo = context.SiteInfo;
            var prePageChannelId = context.PageChannelId;
            var prePageContentId = context.PageContentId;
            if (contentInfo.SiteId != context.SiteId)
            {
                var siteInfo = await parseContext.SiteRepository.GetSiteInfoAsync(contentInfo.SiteId);
                context.PageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, context);
            }

            var theTemplateString = string.Empty;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemTypes in selectedItems.AllKeys)
                {
                    var itemTypeArrayList = TranslateUtils.StringCollectionToStringList(itemTypes);
                    var isTrue = true;
                    foreach (var itemType in itemTypeArrayList)
                    {
                        var (isContentTemplateString, _) = await IsContentTemplateStringAsync(itemType, itemTypes,
                            theTemplateString, selectedItems, selectedValues, context);
                        if (!isContentTemplateString)
                        {
                            isTrue = false;
                        }
                    }
                    if (isTrue)
                    {
                        break;
                    }
                    theTemplateString = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(theTemplateString))
            {
                theTemplateString = templateString;
            }

            var innerBuilder = new StringBuilder(theTemplateString);
            await context.ParseInnerContentAsync(innerBuilder);

            Container.PopContentItem(context.PageInfo);

            if (contentInfo.SiteId != context.SiteId)
            {
                context.PageInfo.ChangeSite(preSiteInfo, prePageChannelId, prePageContentId, parseContext);
            }

            return innerBuilder.ToString();
        }

        private static async Task<(bool IsContentTemplateString, string TemplateString)> IsContentTemplateStringAsync(string itemType, string itemTypes, string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, ParseContext parseContext)
        {
            var contentInfo = await parseContext.GetContentInfoAsync();
            if (contentInfo == null) return (false, null);

            if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedCurrent))//当前内容
            {
                if (contentInfo.Id == parseContext.PageContentId)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsTop))//置顶内容
            {
                if (contentInfo.IsTop)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedImage))//带图片的内容
            {
                if (!string.IsNullOrEmpty(contentInfo.ImageUrl))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedVideo))//带视频的内容
            {
                if (!string.IsNullOrEmpty(contentInfo.VideoUrl))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedFile))//带附件的内容
            {
                if (!string.IsNullOrEmpty(contentInfo.FileUrl))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsRecommend))//推荐的内容
            {
                if (contentInfo.IsRecommend)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsHot))//热点内容
            {
                if (contentInfo.IsHot)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsColor))//醒目内容
            {
                if (contentInfo.IsColor)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedChannelName))//带有附件的内容
            {
                if (selectedValues.Count > 0)
                {
                    var nodeInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(contentInfo.ChannelId);
                    if (nodeInfo != null)
                    {
                        if (selectedValues.Get(nodeInfo.ChannelName) != null)
                        {
                            return (true, selectedValues.Get(nodeInfo.ChannelName));
                        }
                    }
                }
            }
            else if (IsNumberInRange(parseContext.Container.ContentItem.Key + 1, itemType))
            {
                return (true, selectedItems.Get(itemTypes));
            }
            return (false, null);
        }

        public static async Task<string> GetChannelsItemTemplateStringAsync(ParseContext parseContext, string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId)
        {
            var container = Container.GetContainer(parseContext.PageInfo);

            var channelId = container.ChannelItem.Value.Id;

            var context = parseContext.Clone(EContextType.Channel);
            context.Container = container;
            context.ContainerClientId = containerClientId;
            context.ChannelId = channelId;

            var nodeInfo = await parseContext.ChannelRepository.GetChannelInfoAsync(channelId);
            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedCurrent))//当前栏目
                    {
                        if (channelId == context.PageChannelId)
                        {
                            templateString = selectedItems.Get(itemType);
                            break;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedImage))//带有图片的栏目
                    {
                        if (!string.IsNullOrEmpty(nodeInfo.ImageUrl))
                        {
                            templateString = selectedItems.Get(itemType);
                            break;
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(itemType, StlItemTemplate.SelectedUp))//当前栏目的上级栏目
                    {
                        var upLevel = StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedUp) ? 1 : TranslateUtils.ToInt(itemType.Substring(2));
                        if (upLevel > 0)
                        {
                            var theChannelId = await parseContext.GetChannelIdByLevelAsync(context.SiteId, context.PageChannelId, upLevel, -1);
                            if (channelId == theChannelId)
                            {
                                templateString = selectedItems.Get(itemType);
                                break;
                            }
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(itemType, StlItemTemplate.SelectedTop))//当前栏目从首页向下的上级栏目栏目
                    {
                        var topLevel = StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedTop) ? 1 : TranslateUtils.ToInt(itemType.Substring(3));
                        if (topLevel >= 0)
                        {
                            var theChannelId = await parseContext.GetChannelIdByLevelAsync(context.SiteId, context.PageChannelId, 0, topLevel);
                            if (channelId == theChannelId)
                            {
                                templateString = selectedItems.Get(itemType);
                                break;
                            }
                        }
                    }
                    else if (IsNumberInRange(container.ChannelItem.Key + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await context.ParseInnerContentAsync(innerBuilder);

            Container.PopChannelItem(context.PageInfo);

            return innerBuilder.ToString();
        }

        public static async Task<string> GetSqlContentsTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, ParseContext parseContext)
        {
            var container = Container.GetContainer(parseContext.PageInfo);

            var context = parseContext.Clone(EContextType.SqlContent);
            context.ContainerClientId = containerClientId;
            context.Container = container;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (IsNumberInRange(container.SqlItem.Key + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await context.ParseInnerContentAsync(innerBuilder);

            Container.PopSqlItem(context.PageInfo);

            return innerBuilder.ToString();
        }

        public static async Task<string> GetSitesTemplateStringAsync(string templateString, string containerClientId, ParseContext parseContext)
        {
            var container = Container.GetContainer(parseContext.PageInfo);
            var siteInfo = await parseContext.SiteRepository.GetSiteInfoAsync(container.SiteItem.Value.Id);

            var context = parseContext.Clone(EContextType.Site);
            context.ContainerClientId = containerClientId;
            context.Container = container;

            var preSiteInfo = context.SiteInfo;
            var prePageChannelId = context.PageChannelId;
            var prePageContentId = context.PageContentId;
            context.PageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, context);

            var innerBuilder = new StringBuilder(templateString);
            await context.ParseInnerContentAsync(innerBuilder);

            Container.PopSiteItems(context.PageInfo);

            context.PageInfo.ChangeSite(preSiteInfo, prePageChannelId, prePageContentId, context);

            return innerBuilder.ToString();
        }

        public static async Task<string> GetEachsTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, ParseContext parseContext)
        {
            var container = Container.GetContainer(parseContext.PageInfo);

            var context = parseContext.Clone(EContextType.Each);
            context.ContainerClientId = containerClientId;
            context.Container = container;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (IsNumberInRange(container.SqlItem.Key + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await context.ParseInnerContentAsync(innerBuilder);

            Container.PopEachItem(context.PageInfo);

            return innerBuilder.ToString();
        }

        // public static void PutListInfoToMyDataList(ParsedDataList myDataList, ListInfo listInfo)
        // {
        //     myDataList.RepeatColumns = listInfo.Columns;
        //     // myDataList.RepeatDirection = listInfo.Direction;
        //     // myDataList.Height = listInfo.Height;
        //     // myDataList.Width = listInfo.Width;
        //     // if (!string.IsNullOrEmpty(listInfo.Align))
        //     // {
        //     //     myDataList.HorizontalAlign = TranslateUtilsEx.ToHorizontalAlign(listInfo.Align);
        //     // }
        //     // myDataList.ItemStyle.Height = listInfo.ItemHeight;
        //     // myDataList.ItemStyle.Width = listInfo.ItemWidth;
        //     // myDataList.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        //     // if (!string.IsNullOrEmpty(listInfo.ItemAlign))
        //     // {
        //     //     myDataList.ItemStyle.HorizontalAlign = TranslateUtilsEx.ToHorizontalAlign(listInfo.ItemAlign);
        //     // }
        //     // myDataList.ItemStyle.VerticalAlign = VerticalAlign.Top;
        //     // if (!string.IsNullOrEmpty(listInfo.ItemVerticalAlign))
        //     // {
        //     //     myDataList.ItemStyle.VerticalAlign = TranslateUtilsEx.ToVerticalAlign(listInfo.ItemVerticalAlign);
        //     // }
        //     // if (!string.IsNullOrEmpty(listInfo.ItemClass))
        //     // {
        //     //     myDataList.ItemStyle.CssClass = listInfo.ItemClass;
        //     // }

        //     if (listInfo.Layout == ELayout.Table)
        //     {
        //         myDataList.RepeatLayout = RepeatLayout.Table;
        //     }
        //     else if (listInfo.Layout == ELayout.Flow)
        //     {
        //         myDataList.RepeatLayout = RepeatLayout.Flow;
        //     }

        //     foreach (var attributeName in listInfo.Others.AllKeys)
        //     {
        //         myDataList.AddAttribute(attributeName, listInfo.Others[attributeName]);
        //     }
        // }

        private static bool IsNumberInRange(int number, string range)
        {
            if (!string.IsNullOrEmpty(range))
            {
                if (range.IndexOf(',') != -1)
                {
                    var intArr = range.Split(',');
                    foreach (var intStr in intArr)
                    {
                        if (TranslateUtils.ToInt(intStr.Trim()) == number)
                        {
                            return true;
                        }
                    }
                }
                else if (range.IndexOf('_') != -1)
                {
                    var startVal = TranslateUtils.ToInt(range.Split('_')[0].Trim());
                    var endVal = TranslateUtils.ToInt(range.Split('_')[1].Trim());
                    if (number >= startVal && number <= endVal)
                    {
                        return true;
                    }
                }
                else if (range.IndexOf('-') != -1)
                {
                    var startVal = TranslateUtils.ToInt(range.Split('_')[0].Trim());
                    var endVal = TranslateUtils.ToInt(range.Split('_')[1].Trim());
                    if (number >= startVal && number <= endVal)
                    {
                        return true;
                    }
                }
                else if (TranslateUtils.ToInt(range.Trim()) == number)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
