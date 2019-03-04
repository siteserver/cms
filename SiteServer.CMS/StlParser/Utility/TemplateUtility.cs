using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
	public static class TemplateUtility
	{
        public static string GetContentsItemTemplateString(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);
            //var contentInfo = new BackgroundContentInfo(itemContainer.ContentItem.DataItem);

            ContentItemInfo contentItemInfo = null;
            if (pageInfo.ContentItems.Count > 0)
            {
                contentItemInfo = pageInfo.ContentItems.Peek();
            }
            if (contentItemInfo == null) return string.Empty;
            var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, contentItemInfo.ChannelId,
                contentItemInfo.ContentId);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ChannelId = contentInfo.ChannelId;
            contextInfo.ContentId = contentInfo.Id;
            contextInfo.ContentInfo = contentInfo;

            var preSiteInfo = pageInfo.SiteInfo;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;
            if (contentInfo.SiteId != pageInfo.SiteId)
            {
                var siteInfo = SiteManager.GetSiteInfo(contentInfo.SiteId);
                contextInfo.SiteInfo = siteInfo;
                pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, contextInfo);
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
                        if (!IsContentTemplateString(itemType, itemTypes, ref theTemplateString, selectedItems, selectedValues, pageInfo, contextInfo))
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
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopContentItem(pageInfo);

            if (contentInfo.SiteId != pageInfo.SiteId)
            {
                pageInfo.ChangeSite(preSiteInfo, prePageChannelId, prePageContentId, contextInfoRef);
            }

            return innerBuilder.ToString();
        }

        private static bool IsContentTemplateString(string itemType, string itemTypes, ref string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedCurrent))//当前内容
            {
                if (contextInfo.ContentInfo.Id == pageInfo.PageContentId)
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsTop))//置顶内容
            {
                if (contextInfo.ContentInfo.IsTop)
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedImage))//带图片的内容
            {
                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetString(BackgroundContentAttribute.ImageUrl)))
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedVideo))//带视频的内容
            {
                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetString(BackgroundContentAttribute.VideoUrl)))
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedFile))//带附件的内容
            {
                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetString(BackgroundContentAttribute.FileUrl)))
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsRecommend))//推荐的内容
            {
                if (TranslateUtils.ToBool(contextInfo.ContentInfo.GetString(ContentAttribute.IsRecommend)))
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsHot))//热点内容
            {
                if (TranslateUtils.ToBool(contextInfo.ContentInfo.GetString(ContentAttribute.IsHot)))
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedIsColor))//醒目内容
            {
                if (TranslateUtils.ToBool(contextInfo.ContentInfo.GetString(ContentAttribute.IsColor)))
                {
                    templateString = selectedItems.Get(itemTypes);
                    return true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(itemType, StlItemTemplate.SelectedChannelName))//带有附件的内容
            {
                if (selectedValues.Count > 0)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(contextInfo.ContentInfo.SiteId, contextInfo.ContentInfo.ChannelId);
                    if (nodeInfo != null)
                    {
                        if (selectedValues.Get(nodeInfo.ChannelName) != null)
                        {
                            templateString = selectedValues.Get(nodeInfo.ChannelName);
                            return true;
                        }
                    }
                }
            }
            else if (IsNumberInRange(contextInfo.ItemContainer.ContentItem.ItemIndex + 1, itemType))
            {
                templateString = selectedItems.Get(itemTypes);
                return true;
            }
            return false;
        }

        public static string GetChannelsItemTemplateString(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var channelId = itemContainer.ChannelItem.ChannelId;

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ChannelId = channelId;

            var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
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
                            var theChannelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, pageInfo.PageChannelId, upLevel, -1);
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
                            var theChannelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, pageInfo.PageChannelId, 0, topLevel);
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
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopChannelItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetSqlContentsTemplateString(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
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
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopSqlItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetSitesTemplateString(string templateString, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var siteId = SqlUtils.EvalInt(itemContainer.SiteItem.DataItem, SiteAttribute.Id);
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContainerClientId = containerClientId;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContextType = contextType;

            var preSiteInfo = pageInfo.SiteInfo;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;
            pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, contextInfo);

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopSiteItems(pageInfo);

            pageInfo.ChangeSite(preSiteInfo, prePageChannelId, prePageContentId, contextInfo);

            return innerBuilder.ToString();
        }

        public static string GetEachsTemplateString(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string containerClientId, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
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
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

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
                myDataList.HorizontalAlign = TranslateUtils.ToHorizontalAlign(listInfo.Align);
            }
			myDataList.ItemStyle.Height = listInfo.ItemHeight;
			myDataList.ItemStyle.Width = listInfo.ItemWidth;
            myDataList.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            if (!string.IsNullOrEmpty(listInfo.ItemAlign))
            {
                myDataList.ItemStyle.HorizontalAlign = TranslateUtils.ToHorizontalAlign(listInfo.ItemAlign);
            }
            myDataList.ItemStyle.VerticalAlign = VerticalAlign.Top;
            if (!string.IsNullOrEmpty(listInfo.ItemVerticalAlign))
            {
                myDataList.ItemStyle.VerticalAlign = TranslateUtils.ToVerticalAlign(listInfo.ItemVerticalAlign);
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
