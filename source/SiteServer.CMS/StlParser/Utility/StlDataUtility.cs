using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using BaiRong.Core.Model;
using SiteServer.CMS.StlParser.Cache;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlDataUtility
    {
        public static int GetNodeIdByLevel(int publishmentSystemId, int nodeId, int upLevel, int topLevel)
        {
            var theNodeId = nodeId;
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                if (topLevel >= 0)
                {
                    if (topLevel > 0)
                    {
                        if (topLevel < nodeInfo.ParentsCount)
                        {
                            var parentIdStrList = TranslateUtils.StringCollectionToStringList(nodeInfo.ParentsPath);
                            if (parentIdStrList[topLevel] != null)
                            {
                                var parentIdStr = parentIdStrList[topLevel];
                                theNodeId = int.Parse(parentIdStr);
                            }
                        }
                    }
                    else
                    {
                        theNodeId = publishmentSystemId;
                    }
                }
                else if (upLevel > 0)
                {
                    if (upLevel < nodeInfo.ParentsCount)
                    {
                        var parentIdStrList = TranslateUtils.StringCollectionToStringList(nodeInfo.ParentsPath);
                        if (parentIdStrList[upLevel] != null)
                        {
                            var parentIdStr = parentIdStrList[nodeInfo.ParentsCount - upLevel];
                            theNodeId = int.Parse(parentIdStr);
                        }
                    }
                    else
                    {
                        theNodeId = publishmentSystemId;
                    }
                }
            }
            return theNodeId;
        }

        public static List<int> GetNodeIdList(int publishmentSystemId, int channelId, string groupContent, string groupContentNot, string orderByString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isImageExists, bool isImage, int totalNum, string where, string guid)
        {
            var whereString = Node.GetWhereString(publishmentSystemId, groupContent, groupContentNot, isImageExists, isImage, where, guid);
            return Node.GetNodeIdList(channelId, totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot, guid);
        }

        //public static int GetNodeIDByChannelIDOrChannelIndexOrChannelName(int publishmentSystemID, int channelID, string channelIndex, string channelName)
        //{
        //    int retval = channelID;
        //    if (!string.IsNullOrEmpty(channelIndex))
        //    {
        //        int theNodeID = DataProvider.NodeDAO.GetNodeIDByNodeIndexName(publishmentSystemID, channelIndex);
        //        if (theNodeID != 0)
        //        {
        //            retval = theNodeID;
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(channelName))
        //    {
        //        int theNodeID = DataProvider.NodeDAO.GetNodeIDByParentIDAndNodeName(retval, channelName, true);
        //        if (theNodeID == 0)
        //        {
        //            theNodeID = DataProvider.NodeDAO.GetNodeIDByParentIDAndNodeName(publishmentSystemID, channelName, true);
        //        }
        //        if (theNodeID != 0)
        //        {
        //            retval = theNodeID;
        //        }
        //    }
        //    return retval;
        //}

        public static ETaxisType GetETaxisTypeByOrder(string order, bool isChannel, ETaxisType defaultType)
        {
            var taxisType = defaultType;
            if (!string.IsNullOrEmpty(order))
            {
                if (isChannel)
                {
                    if (order.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxis;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxisDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDateDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHits;
                    }
                }
                else
                {
                    if (order.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxisDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxis;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDateDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderLastEditDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByLastEditDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByLastEditDateDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHits;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHitsByDay.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHitsByDay;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHitsByWeek.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHitsByWeek;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHitsByMonth.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHitsByMonth;
                    }
                }
            }
            return taxisType;
        }

        public static string GetOrderByString(int publishmentSystemId, string orderValue, ETableStyle tableStyle, ETaxisType defaultType, string guid)
        {
            var taxisType = defaultType;
            var orderByString = string.Empty;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (tableStyle == ETableStyle.Channel)
                {
                    if (orderValue.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxis;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxisDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDateDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDate;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHits;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByRandom;
                    }
                    else
                    {
                        orderByString = orderValue;
                    }
                }
                else if (tableStyle == ETableStyle.BackgroundContent)
                {
                    if (orderValue.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxisDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxis;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDateDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDate;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderLastEditDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByLastEditDateDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderLastEditDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByLastEditDate;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHits;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderHitsByDay.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHitsByDay;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderHitsByWeek.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHitsByWeek;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderHitsByMonth.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByHitsByMonth;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderStars.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByStars;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderDigg.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByDigg;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderComments.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByComments;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByRandom;
                    }
                    else
                    {
                        orderByString = orderValue;
                    }
                }
                else if (tableStyle == ETableStyle.InputContent)
                {
                    if (orderValue.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxisDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByTaxis;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDateDesc;
                    }
                    else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = ETaxisType.OrderByAddDate;
                    }
                    else
                    {
                        orderByString = orderValue;
                    }
                }
            }
            List<int> orderedContentIdList = null;
            if (taxisType == ETaxisType.OrderByHits)
            {
                if (tableStyle == ETableStyle.Channel)
                {
                    orderedContentIdList = Tracking.GetPageNodeIdListByAccessNum(publishmentSystemId, guid);
                }
            }
            else if (taxisType == ETaxisType.OrderByStars)
            {
                orderedContentIdList = Star.GetContentIdListByPoint(publishmentSystemId, guid);
            }
            else if (taxisType == ETaxisType.OrderByDigg)
            {
                orderedContentIdList = Digg.GetRelatedIdentityListByTotal(publishmentSystemId, guid);
            }
            else if (taxisType == ETaxisType.OrderByComments)
            {
                orderedContentIdList = Comment.GetContentIdListByCount(publishmentSystemId, guid);
            }
            return ETaxisTypeUtils.GetOrderByString(tableStyle, taxisType, orderByString, orderedContentIdList);
        }

        public static string GetStlPageContentsSqlString(PublishmentSystemInfo publishmentSystemInfo, int channelId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, string guid)
        {
            if (!NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, channelId)) return string.Empty;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var sqlWhereString = tableStyle == ETableStyle.BackgroundContent ? Content.GetStlWhereString(publishmentSystemInfo.PublishmentSystemId, tableName, groupContent, groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, publishmentSystemInfo.Additional.IsCreateSearchDuplicate, guid) : Content.GetStlWhereString(publishmentSystemInfo.PublishmentSystemId, groupContent, groupContentNot, tags, isTopExists, isTop, where, guid);

            return Content.GetStlSqlStringChecked(tableName, publishmentSystemInfo.PublishmentSystemId, channelId, startNum, totalNum, orderByString, sqlWhereString, scopeType, groupChannel, groupChannelNot, isNoDup, guid);
        }

        public static string GetPageContentsSqlStringBySearch(string tableName, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, string guid)
        {
            var sqlWhereString = Content.GetStlWhereStringBySearch(tableName, groupContent, groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, guid);
            var sqlString = Content.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum, orderByString, sqlWhereString, isNoDup, guid);

            return sqlString;
        }

        public static IEnumerable GetContentsDataSource(PublishmentSystemInfo publishmentSystemInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, LowerNameValueCollection others, string guid)
        {
            if (!NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, channelId)) return null;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            if (isRelatedContents && contentId > 0)
            {
                var isTags = false;
                var tagCollection = Content.GetValue(tableName, contentId, ContentAttribute.Tags, guid);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = Tag.GetContentIdListByTagCollection(TranslateUtils.StringCollectionToStringCollection(tagCollection), publishmentSystemInfo.PublishmentSystemId, guid);
                    if (contentIdList.Count > 0)
                    {
                        contentIdList.Remove(contentId);
                        isTags = true;
                        if (string.IsNullOrEmpty(where))
                        {
                            where =
                                $"ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                        }
                        else
                        {
                            where +=
                                $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)}))";
                        }
                    }
                }

                if (!isTags)
                {
                    if (string.IsNullOrEmpty(where))
                    {
                        where = $"ID <> {contentId}";
                    }
                    else
                    {
                        where += $" AND (ID <> {contentId})";
                    }
                }
            }

            string sqlWhereString;
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovPublicContent)
            {
                sqlWhereString = Content.GetStlWhereString(publishmentSystemInfo.PublishmentSystemId, tableName, groupContent, groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, publishmentSystemInfo.Additional.IsCreateSearchDuplicate, guid);
            }
            else
            {
                sqlWhereString = Content.GetStlWhereString(publishmentSystemInfo.PublishmentSystemId, groupContent, groupContentNot, tags, isTopExists, isTop, where, guid);
            }
            return DataProvider.ContentDao.GetStlDataSourceChecked(tableName, channelId, startNum, totalNum, orderByString, sqlWhereString, scopeType, groupChannel, groupChannelNot, isNoDup, others);
        }

        public static IEnumerable GetCommentsDataSource(int publishmentSystemId, int channelId, int contentId, DbItemContainer itemContainer, int startNum, int totalNum, bool isRecommend, string orderByString, string where, string guid)
        {
            var sqlString = Comment.GetSelectSqlStringWithChecked(publishmentSystemId, channelId, contentId, startNum, totalNum, isRecommend, where, orderByString, guid);
            return BaiRongDataProvider.DatabaseDao.GetDataSource(sqlString);
        }

        public static DataSet GetPageCommentsDataSet(int publishmentSystemId, int channelId, int contentId, DbItemContainer itemContainer, int startNum, int totalNum, bool isRecommend, string orderByString, string where, string guid)
        {
            var sqlString = Comment.GetSelectSqlStringWithChecked(publishmentSystemId, channelId, contentId, startNum, totalNum, isRecommend, where, orderByString, guid);
            return BaiRongDataProvider.DatabaseDao.GetDataSet(sqlString);
        }

        public static IEnumerable GetInputContentsDataSource(int publishmentSystemId, int inputId, ListInfo listInfo, string guid)
        {
            var isReplyExists = listInfo.Others.Get(StlInputContents.AttributeIsReply) != null;
            var isReply = TranslateUtils.ToBool(listInfo.Others.Get(StlInputContents.AttributeIsReply));
            var sqlString = InputContent.GetSelectSqlStringWithChecked(publishmentSystemId, inputId, isReplyExists, isReply, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.OrderByString, listInfo.Others, guid);
            return BaiRongDataProvider.DatabaseDao.GetDataSource(sqlString);
        }

        public static DataSet GetPageInputContentsDataSet(int publishmentSystemId, int inputId, ListInfo listInfo, string guid)
        {
            var isReplyExists = listInfo.Others.Get(StlInputContents.AttributeIsReply) != null;
            var isReply = TranslateUtils.ToBool(listInfo.Others.Get(StlInputContents.AttributeIsReply));
            var sqlString = InputContent.GetSelectSqlStringWithChecked(publishmentSystemId, inputId, isReplyExists, isReply, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.OrderByString, listInfo.Others, guid);
            return BaiRongDataProvider.DatabaseDao.GetDataSet(sqlString);
        }

        public static IEnumerable GetChannelsDataSource(int publishmentSystemId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where, string guid)
        {
            IEnumerable ie;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = Node.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where, guid);
                ie = DataProvider.NodeDao.GetStlDataSourceByPublishmentSystemId(publishmentSystemId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                if (nodeInfo == null) return null;

                var sqlWhereString = Node.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where, guid);
                ie = DataProvider.NodeDao.GetStlDataSource(nodeInfo.NodeId, nodeInfo.ChildrenCount, startNum, totalNum, sqlWhereString, scopeType, orderByString);
            }

            return ie;
        }

        public static DataSet GetPageChannelsDataSet(int publishmentSystemId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where, string guid)
        {
            DataSet dataSet;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = Node.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where, guid);
                dataSet = DataProvider.NodeDao.GetStlDataSetByPublishmentSystemId(publishmentSystemId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                if (nodeInfo == null) return null;

                var sqlWhereString = Node.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where, guid);
                dataSet = DataProvider.NodeDao.GetStlDataSet(nodeInfo.NodeId, nodeInfo.ChildrenCount, startNum, totalNum, sqlWhereString, scopeType, orderByString);
            }
            return dataSet;
        }

        public static IEnumerable GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string orderByString, string guid)
        {
            var sqlString = TableStructure.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString, guid);
            return BaiRongDataProvider.DatabaseDao.GetDataSource(connectionString, sqlString);
        }

        public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string orderByString, string guid)
        {
            var sqlString = TableStructure.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString, guid);
            return BaiRongDataProvider.DatabaseDao.GetDataSet(connectionString, sqlString);
        }

        public static IEnumerable GetSitesDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString, string since, string guid)
        {
            return DataProvider.PublishmentSystemDao.GetStlDataSource(siteName, siteDir, startNum, totalNum, whereString, scopeType, orderByString, since);
        }

        public static IEnumerable GetPhotosDataSource(PublishmentSystemInfo publishmentSystemInfo, int contentId, int startNum, int totalNum, string guid)
        {
            return DataProvider.PhotoDao.GetStlDataSource(publishmentSystemInfo.PublishmentSystemId, contentId, startNum, totalNum);
        }

        public static IEnumerable GetDataSourceByStlElement(PublishmentSystemInfo publishmentSystemInfo, int templateId, string elementName, string stlElement, string guid)
        {
            var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, false);
            XmlNode node = xmlDocument.DocumentElement;
            if (node == null) return null;

            node = node.FirstChild;

            var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemInfo.PublishmentSystemId, templateId);
            var pageInfo = new PageInfo(guid, publishmentSystemInfo.PublishmentSystemId, 0, publishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);

            if (node?.Name == null) return null;

            if (elementName == StlChannels.ElementName)
            {
                var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.Channel);

                return StlChannels.GetDataSource(pageInfo, contextInfo, listInfo);
            }
            if (elementName == StlContents.ElementName)
            {
                var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.Content);

                return StlContents.GetDataSource(pageInfo, contextInfo, listInfo);
            }

            return null;
        }
    }
}
