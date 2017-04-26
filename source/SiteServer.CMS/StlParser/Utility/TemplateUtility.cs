using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement.Inner;

namespace SiteServer.CMS.StlParser.Utility
{
	public class TemplateUtility
	{
		private TemplateUtility()
		{
		}

        public static string GetContentsItemTemplateString(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);
            var contentInfo = new BackgroundContentInfo(itemContainer.ContentItem.DataItem);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ChannelID = contentInfo.NodeId;
            contextInfo.ContentID = contentInfo.Id;
            contextInfo.ContentInfo = contentInfo;

            var theTemplateString = string.Empty;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (string itemTypes in selectedItems.Keys)
                {
                    var itemTypeArrayList = TranslateUtils.StringCollectionToStringList(itemTypes);
                    var isTrue = true;
                    foreach (string itemType in itemTypeArrayList)
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
                    else
                    {
                        theTemplateString = string.Empty;
                    }
                }
            }

            if (string.IsNullOrEmpty(theTemplateString))
            {
                theTemplateString = templateString;
            }

            var innerBuilder = new StringBuilder(theTemplateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopContentItem(pageInfo);

            return innerBuilder.ToString();
        }

        private static bool IsContentTemplateString(string itemType, string itemTypes, ref string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (itemType == StlItem.ContentsItem.Selected_Current)//当前内容
            {
                if (contextInfo.ContentInfo.Id == pageInfo.PageContentId)
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_IsTop)//置顶内容
            {
                if (contextInfo.ContentInfo.IsTop)
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_Image)//带图片的内容
            {
                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl)))
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_Video)//带视频的内容
            {
                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl)))
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_File)//带附件的内容
            {
                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl)))
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_IsRecommend)//推荐的内容
            {
                if (TranslateUtils.ToBool(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.IsRecommend)))
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_IsHot)//热点内容
            {
                if (TranslateUtils.ToBool(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.IsHot)))
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_IsColor)//醒目内容
            {
                if (TranslateUtils.ToBool(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.IsColor)))
                {
                    templateString = selectedItems[itemTypes];
                    return true;
                }
            }
            else if (itemType == StlItem.ContentsItem.Selected_ChannelName)//带有附件的内容
            {
                if (selectedValues.Count > 0)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(contextInfo.ContentInfo.PublishmentSystemId, contextInfo.ContentInfo.NodeId);
                    if (nodeInfo != null)
                    {
                        if (selectedValues[nodeInfo.NodeName] != null)
                        {
                            templateString = selectedValues[nodeInfo.NodeName];
                            return true;
                        }
                    }
                }
            }
            else if (IsNumberInRange(contextInfo.ItemContainer.ContentItem.ItemIndex + 1, itemType))
            {
                templateString = selectedItems[itemTypes];
                return true;
            }
            return false;
        }

        public static string GetChannelsItemTemplateString(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var nodeID = SqlUtils.EvalInt(itemContainer.ChannelItem.DataItem, NodeAttribute.NodeId);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ChannelID = nodeID;

            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeID);
            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (string itemType in selectedItems.Keys)
                {
                    if (itemType == StlItem.ChannelsItem.Selected_Current)//当前栏目
                    {
                        if (nodeID == pageInfo.PageNodeId)
                        {
                            templateString = selectedItems[itemType];
                            break;
                        }
                    }
                    else if (itemType == StlItem.ChannelsItem.Selected_Image)//带有图片的栏目
                    {
                        if (!string.IsNullOrEmpty(nodeInfo.ImageUrl))
                        {
                            templateString = selectedItems[itemType];
                            break;
                        }
                    }
                    else if (itemType.StartsWith(StlItem.ChannelsItem.Selected_Up))//当前栏目的上级栏目
                    {
                        var upLevel = 1;
                        if (itemType == StlItem.ChannelsItem.Selected_Up)
                        {
                            upLevel = 1;
                        }
                        else
                        {
                            upLevel = TranslateUtils.ToInt(itemType.Substring(2));
                        }
                        if (upLevel > 0)
                        {
                            var theNodeID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, pageInfo.PageNodeId, upLevel, -1);
                            if (nodeID == theNodeID)
                            {
                                templateString = selectedItems[itemType];
                                break;
                            }
                        }
                    }
                    else if (itemType.StartsWith(StlItem.ChannelsItem.Selected_Top))//当前栏目从首页向下的上级栏目栏目
                    {
                        var topLevel = 1;
                        if (itemType == StlItem.ChannelsItem.Selected_Top)
                        {
                            topLevel = 1;
                        }
                        else
                        {
                            topLevel = TranslateUtils.ToInt(itemType.Substring(3));
                        }
                        if (topLevel >= 0)
                        {
                            var theNodeID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, pageInfo.PageNodeId, 0, topLevel);
                            if (nodeID == theNodeID)
                            {
                                templateString = selectedItems[itemType];
                                break;
                            }
                        }
                    }
                    else if (IsNumberInRange(itemContainer.ChannelItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems[itemType];
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopChannelItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetCommentsTemplateString(string templateString, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = EContextType.Comment;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContainerClientID = containerClientID;

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopCommentItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetInputContentsTemplateString(string templateString, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContextType = contextType;

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopInputItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetSqlContentsTemplateString(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (string itemType in selectedItems.Keys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems[itemType];
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopSqlItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetSitesTemplateString(string templateString, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var publishmentSystemID = SqlUtils.EvalInt(itemContainer.SiteItem.DataItem, PublishmentSystemAttribute.PublishmentSystemId);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ItemContainer = itemContainer;
            contextInfo.ContextType = contextType;

            var prePublishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            var prePageNodeID = pageInfo.PageNodeId;
            var prePageContentID = pageInfo.PageContentId;
            pageInfo.ChangeSite(publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId, 0, contextInfo);

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopInputItem(pageInfo);

            pageInfo.ChangeSite(prePublishmentSystemInfo, prePageNodeID, prePageContentID, contextInfo);

            return innerBuilder.ToString();
        }

        public static string GetPhotosTemplateString(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (string itemType in selectedItems.Keys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems[itemType];
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopPhotoItem(pageInfo);

            return innerBuilder.ToString();
        }

        public static string GetEachsTemplateString(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string containerClientID, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfoRef)
        {
            var itemContainer = DbItemContainer.GetItemContainer(pageInfo);

            var contextInfo = contextInfoRef.Clone();
            contextInfo.ContextType = contextType;
            contextInfo.ContainerClientID = containerClientID;
            contextInfo.ItemContainer = itemContainer;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (string itemType in selectedItems.Keys)
                {
                    if (IsNumberInRange(itemContainer.SqlItem.ItemIndex + 1, itemType))
                    {
                        templateString = selectedItems[itemType];
                        break;
                    }
                }
            }

            var innerBuilder = new StringBuilder(templateString);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            DbItemContainer.PopEachItem(pageInfo);

            return innerBuilder.ToString();
        }

		public static void PutContentsDisplayInfoToMyDataList(ParsedDataList myDataList, ContentsDisplayInfo displayInfo)
		{
			myDataList.RepeatColumns = displayInfo.Columns;
			myDataList.RepeatDirection = displayInfo.Direction;
			myDataList.Height = displayInfo.Height;
			myDataList.Width = displayInfo.Width;
            if (!string.IsNullOrEmpty(displayInfo.Align))
            {
                myDataList.HorizontalAlign = Converter.ToHorizontalAlign(displayInfo.Align);
            }
			myDataList.ItemStyle.Height = displayInfo.ItemHeight;
			myDataList.ItemStyle.Width = displayInfo.ItemWidth;
            myDataList.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            if (!string.IsNullOrEmpty(displayInfo.ItemAlign))
            {
                myDataList.ItemStyle.HorizontalAlign = Converter.ToHorizontalAlign(displayInfo.ItemAlign);
            }
            myDataList.ItemStyle.VerticalAlign = VerticalAlign.Top;
            if (!string.IsNullOrEmpty(displayInfo.ItemVerticalAlign))
            {
                myDataList.ItemStyle.VerticalAlign = Converter.ToVerticalAlign(displayInfo.ItemVerticalAlign);
            }
            if (!string.IsNullOrEmpty(displayInfo.ItemClass))
            {
                myDataList.ItemStyle.CssClass = displayInfo.ItemClass;
            }

            if (displayInfo.Layout == ELayout.Table)
			{
				myDataList.RepeatLayout = RepeatLayout.Table;
			}
            else if (displayInfo.Layout == ELayout.Flow)
            {
                myDataList.RepeatLayout = RepeatLayout.Flow;
            }

            foreach (string attributeName in displayInfo.OtherAttributes)
            {
                myDataList.AddAttribute(attributeName, displayInfo.OtherAttributes[attributeName]);
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
