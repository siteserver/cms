using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Datory.Utils;
using SSCMS;
using SSCMS.Parse;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Utility
{
	public static class TemplateUtility
	{
        public static async Task<string> GetContentsItemTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, IParseManager parseManager, ParseType contextType)
        {
            var context = parseManager.ContextInfo;

            var pageInfo = parseManager.PageInfo;
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);
            var content = itemContainer.ContentItem.Value;

            parseManager.ContextInfo = parseManager.ContextInfo.Clone();
            parseManager.ContextInfo.ContextType = contextType;
            parseManager.ContextInfo.ItemContainer = itemContainer;
            parseManager.ContextInfo.ContainerClientId = containerClientId;
            parseManager.ContextInfo.ChannelId = content.ChannelId;
            parseManager.ContextInfo.ContentId = content.Id;
            parseManager.ContextInfo.SetContent(content);

            var preSite = pageInfo.Site;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;
            if (content.SiteId != pageInfo.SiteId)
            {
                var siteInfo = await parseManager.DatabaseManager.SiteRepository.GetAsync(content.SiteId);
                parseManager.ContextInfo.Site = siteInfo;
                pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, parseManager.ContextInfo);
            }

            var theTemplateString = string.Empty;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemTypes in selectedItems.AllKeys)
                {
                    var itemTypeArrayList = Utilities.GetStringList(itemTypes);
                    var isTrue = true;
                    foreach (var itemType in itemTypeArrayList)
                    {
                        var (success, contentTemplateString) = await IsContentTemplateStringAsync(itemType, itemTypes,
                            selectedItems, selectedValues, parseManager);
                        if (!success)
                        {
                            isTrue = false;
                        }
                        else
                        {
                            theTemplateString = contentTemplateString;
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
            await parseManager.ParseInnerContentAsync(innerBuilder);

            DbItemContainer.PopContentItem(pageInfo);

            if (content.SiteId != pageInfo.SiteId)
            {
                pageInfo.ChangeSite(preSite, prePageChannelId, prePageContentId, parseManager.ContextInfo);
            }

            parseManager.ContextInfo = context;

            return innerBuilder.ToString();
        }

        private static async Task<(bool Success, string TemplateString)> IsContentTemplateStringAsync(string itemType, string itemTypes, NameValueCollection selectedItems, NameValueCollection selectedValues, IParseManager parseManager)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var contentInfo = await parseManager.GetContentAsync();
            if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedCurrent))//当前内容
            {
                if (contentInfo.Id == pageInfo.PageContentId)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsTop))//置顶内容
            {
                if (contentInfo.Top)
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
                if (contentInfo.Recommend)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsHot))//热点内容
            {
                if (contentInfo.Hot)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsColor))//醒目内容
            {
                if (contentInfo.Color)
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedChannelName))//带有附件的内容
            {
                if (selectedValues.Count > 0)
                {
                    var nodeInfo = await parseManager.DatabaseManager.ChannelRepository.GetAsync(contentInfo.ChannelId);
                    if (nodeInfo != null)
                    {
                        if (selectedValues.Get(nodeInfo.ChannelName) != null)
                        {
                            return (true, selectedValues.Get(nodeInfo.ChannelName));
                        }
                    }
                }
            }
            else if (IsNumberInRange(contextInfo.ItemContainer.ContentItem.Key + 1, itemType))
            {
                return (true, selectedItems.Get(itemTypes));
            }
            return (false, string.Empty);
        }

        public static async Task<string> GetChannelsItemTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, IParseManager parseManager, ParseType contextType)
        {
            var context = parseManager.ContextInfo;

            var pageInfo = parseManager.PageInfo;
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var channel = itemContainer.ChannelItem.Value;

            parseManager.ContextInfo = parseManager.ContextInfo.Clone();
            parseManager.ContextInfo.ContextType = contextType;
            parseManager.ContextInfo.ItemContainer = itemContainer;
            parseManager.ContextInfo.ContainerClientId = containerClientId;
            parseManager.ContextInfo.ChannelId = channel.Id;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedCurrent))//当前栏目
                    {
                        if (channel.Id == pageInfo.PageChannelId)
                        {
                            templateString = selectedItems.Get(itemType);
                            break;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedImage))//带有图片的栏目
                    {
                        if (!string.IsNullOrEmpty(channel.ImageUrl))
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
                            var dataManager = new StlDataManager(parseManager.DatabaseManager);
                            var theChannelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, pageInfo.PageChannelId, upLevel, -1);
                            if (channel.Id == theChannelId)
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
                            var dataManager = new StlDataManager(parseManager.DatabaseManager);
                            var theChannelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, pageInfo.PageChannelId, 0, topLevel);
                            if (channel.Id == theChannelId)
                            {
                                templateString = selectedItems.Get(itemType);
                                break;
                            }
                        }
                    }
                    else if (IsNumberInRange(itemContainer.ChannelItem.Key + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await parseManager.ParseInnerContentAsync(innerBuilder);

            DbItemContainer.PopChannelItem(pageInfo);

            parseManager.ContextInfo = context;

            return innerBuilder.ToString();
        }

        public static async Task<string> GetSqlContentsTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, IParseManager parseManager, ParseType contextType)
        {
            var context = parseManager.ContextInfo;

            var pageInfo = parseManager.PageInfo;
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            parseManager.ContextInfo = parseManager.ContextInfo.Clone();
            parseManager.ContextInfo.ContextType = contextType;
            parseManager.ContextInfo.ContainerClientId = containerClientId;
            parseManager.ContextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.Key + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await parseManager.ParseInnerContentAsync(innerBuilder);

            DbItemContainer.PopSqlItem(pageInfo);

            parseManager.ContextInfo = context;

            return innerBuilder.ToString();
        }

        public static async Task<string> GetSitesTemplateStringAsync(string templateString, string containerClientId, IParseManager parseManager, ParseType contextType)
        {
            var context = parseManager.ContextInfo;

            var pageInfo = parseManager.PageInfo;
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var siteInfo = itemContainer.SiteItem.Value;

            parseManager.ContextInfo = parseManager.ContextInfo.Clone();
            parseManager.ContextInfo.ContainerClientId = containerClientId;
            parseManager.ContextInfo.ItemContainer = itemContainer;
            parseManager.ContextInfo.ContextType = contextType;

            var preSite = pageInfo.Site;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;
            pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, parseManager.ContextInfo);

            var innerBuilder = new StringBuilder(templateString);
            await parseManager.ParseInnerContentAsync(innerBuilder);

            DbItemContainer.PopSiteItems(pageInfo);

            pageInfo.ChangeSite(preSite, prePageChannelId, prePageContentId, parseManager.ContextInfo);

            parseManager.ContextInfo = context;

            return innerBuilder.ToString();
        }

        public static async Task<string> GetEachsTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, IParseManager parseManager, ParseType contextType)
        {
            var context = parseManager.ContextInfo;

            var pageInfo = parseManager.PageInfo;
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            parseManager.ContextInfo = parseManager.ContextInfo.Clone();
            parseManager.ContextInfo.ContextType = contextType;
            parseManager.ContextInfo.ContainerClientId = containerClientId;
            parseManager.ContextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.Key + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await parseManager.ParseInnerContentAsync(innerBuilder);

            DbItemContainer.PopEachItem(pageInfo);

            parseManager.ContextInfo = context;

            return innerBuilder.ToString();
        }

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
