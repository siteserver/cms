using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.StlParser.Utility
{
	public static class TemplateUtility
	{
        public static async Task<string> GetContentsItemTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);
            //var contentInfo = new BackgroundContentInfo(itemContainer.ContentItem.DataItem);

            ContentItemInfo contentItemInfo = null;
            if (pageInfo.ContentItems.Count > 0)
            {
                contentItemInfo = pageInfo.ContentItems.Peek();
            }
            if (contentItemInfo == null) return string.Empty;
            var contentInfo = await DataProvider.ContentRepository.GetAsync(pageInfo.Site, contentItemInfo.ChannelId,
                contentItemInfo.ContentId);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ChannelId = contentInfo.ChannelId;
            contextInfo.ContentId = contentInfo.Id;
            contextInfo.SetContentInfo(contentInfo);

            var preSite = pageInfo.Site;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;
            if (contentInfo.SiteId != pageInfo.SiteId)
            {
                var siteInfo = await DataProvider.SiteRepository.GetAsync(contentInfo.SiteId);
                contextInfo.Site = siteInfo;
                pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, contextInfo);
            }

            var theTemplateString = string.Empty;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemTypes in selectedItems.AllKeys)
                {
                    var itemTypeArrayList = StringUtils.GetStringList(itemTypes);
                    var isTrue = true;
                    foreach (var itemType in itemTypeArrayList)
                    {
                        var (success, contentTemplateString) = await IsContentTemplateStringAsync(itemType, itemTypes,
                            selectedItems, selectedValues, pageInfo, contextInfo);
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
            await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopContentItem(pageInfo);

            if (contentInfo.SiteId != pageInfo.SiteId)
            {
                pageInfo.ChangeSite(preSite, prePageChannelId, prePageContentId, contextInfoRef);
            }

            return innerBuilder.ToString();
        }

        private static async Task<(bool Success, string TemplateString)> IsContentTemplateStringAsync(string itemType, string itemTypes, NameValueCollection selectedItems, NameValueCollection selectedValues, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var contentInfo = await contextInfo.GetContentAsync();
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
                if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.ImageUrl)))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedVideo))//带视频的内容
            {
                if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.VideoUrl)))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedFile))//带附件的内容
            {
                if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.FileUrl)))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsRecommend))//推荐的内容
            {
                if (TranslateUtils.ToBool(contentInfo.Get<string>(ContentAttribute.IsRecommend)))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsHot))//热点内容
            {
                if (TranslateUtils.ToBool(contentInfo.Get<string>(ContentAttribute.IsHot)))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsColor))//醒目内容
            {
                if (TranslateUtils.ToBool(contentInfo.Get<string>(ContentAttribute.IsColor)))
                {
                    return (true, selectedItems.Get(itemTypes));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedChannelName))//带有附件的内容
            {
                if (selectedValues.Count > 0)
                {
                    var nodeInfo = await ChannelManager.GetChannelAsync(contentInfo.SiteId, contentInfo.ChannelId);
                    if (nodeInfo != null)
                    {
                        if (selectedValues.Get(nodeInfo.ChannelName) != null)
                        {
                            return (true, selectedValues.Get(nodeInfo.ChannelName));
                        }
                    }
                }
            }
            else if (IsNumberInRange(contextInfo.ItemContainer.ContentItem.ItemIndex + 1, itemType))
            {
                return (true, selectedItems.Get(itemTypes));
            }
            return (false, string.Empty);
        }

        public static async Task<string> GetChannelsItemTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var channelId = itemContainer.ChannelItem.ChannelId;

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ChannelId = channelId;

            var nodeInfo = await ChannelManager.GetChannelAsync(pageInfo.SiteId, channelId);
            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedCurrent))//当前栏目
                    {
                        if (channelId == pageInfo.PageChannelId)
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
                            var theChannelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, pageInfo.PageChannelId, upLevel, -1);
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
                            var theChannelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, pageInfo.PageChannelId, 0, topLevel);
                            if (channelId == theChannelId)
                            {
                                templateString = selectedItems.Get(itemType);
                                break;
                            }
                        }
                    }
                    else if (IsNumberInRange(itemContainer.ChannelItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopChannelItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static async Task<string> GetSqlContentsTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopSqlItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static async Task<string> GetSitesTemplateStringAsync(string templateString, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var siteId = SqlUtils.EvalInt(itemContainer.SiteItem.DataItem, nameof(Site.Id));
            var siteInfo = await DataProvider.SiteRepository.GetAsync(siteId);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContextType = contextType;

            var preSite = pageInfo.Site;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;
            pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, contextInfo);

            var innerBuilder = new StringBuilder(templateString);
            await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopSiteItems(pageInfo);

            pageInfo.ChangeSite(preSite, prePageChannelId, prePageContentId, contextInfo);

            return innerBuilder.ToString();
        }

        public static async Task<string> GetEachsTemplateStringAsync(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var itemType in selectedItems.AllKeys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems.Get(itemType);
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopEachItem(pageInfo);

            return innerBuilder.ToString();
        }

		public static void PutListInfoToMyDataList(ParsedDataList myDataList, ListInfo listInfo)
		{
			myDataList.RepeatColumns = listInfo.Columns;
			myDataList.RepeatDirection = listInfo.Direction;
			myDataList.Height = listInfo.Height;
			myDataList.Width = listInfo.Width;
            if (!string.IsNullOrEmpty(listInfo.Align))
            {
                myDataList.HorizontalAlign = WebUtils.ToHorizontalAlign(listInfo.Align);
            }
			myDataList.ItemStyle.Height = listInfo.ItemHeight;
			myDataList.ItemStyle.Width = listInfo.ItemWidth;
            myDataList.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            if (!string.IsNullOrEmpty(listInfo.ItemAlign))
            {
                myDataList.ItemStyle.HorizontalAlign = WebUtils.ToHorizontalAlign(listInfo.ItemAlign);
            }
            myDataList.ItemStyle.VerticalAlign = VerticalAlign.Top;
            if (!string.IsNullOrEmpty(listInfo.ItemVerticalAlign))
            {
                myDataList.ItemStyle.VerticalAlign = WebUtils.ToVerticalAlign(listInfo.ItemVerticalAlign);
            }
            if (!string.IsNullOrEmpty(listInfo.ItemClass))
            {
                myDataList.ItemStyle.CssClass = listInfo.ItemClass;
            }

            if (listInfo.Layout == ELayout.Table)
			{
				myDataList.RepeatLayout = RepeatLayout.Table;
			}
            else if (listInfo.Layout == ELayout.Flow)
            {
                myDataList.RepeatLayout = RepeatLayout.Flow;
            }

            foreach (var attributeName in listInfo.Others.AllKeys)
            {
                myDataList.AddAttribute(attributeName, listInfo.Others[attributeName]);
            }
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
