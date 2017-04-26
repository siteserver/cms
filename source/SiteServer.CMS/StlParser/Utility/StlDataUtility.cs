using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

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

        public static List<int> GetNodeIdList(int publishmentSystemId, int channelId, string groupContent, string groupContentNot, string orderByString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isImageExists, bool isImage, int totalNum, string where)
        {
            var whereString = DataProvider.NodeDao.GetWhereString(publishmentSystemId, groupContent, groupContentNot, isImageExists, isImage, where);
            return DataProvider.NodeDao.GetNodeIdList(channelId, totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot);
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

        public static IEnumerable GetPropertysDataSource(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, EContextType contextType, string property, int startNum, int totalNum)
        {
            var arrayList = new ArrayList();
            var propertyList = new List<string>();
            if (contextType == EContextType.Content)
            {
                //第一条
                propertyList.Add(contentInfo.GetExtendedAttribute(property));
                //第n条
                var extentAttributeName = ContentAttribute.GetExtendAttributeName(property);
                propertyList.AddRange(TranslateUtils.StringCollectionToStringList(contentInfo.GetExtendedAttribute(extentAttributeName)));

            }
            if (startNum > propertyList.Count)
            {
                startNum = propertyList.Count;
                totalNum = 1;
            }
            if (startNum + totalNum > propertyList.Count)
            {
                totalNum = propertyList.Count - startNum + 1;
            }
            if (totalNum == 0)
            {
                totalNum = propertyList.Count - startNum + 1;
            }
            for (var i = startNum; i < startNum + totalNum; i++)
            {
                contentInfo.SetExtendedAttribute(property, propertyList[i - 1]);
                var item = contentInfo.Copy() as ContentInfo;
                if (item != null)
                {
                    arrayList.Add(item);
                }
            }
            return arrayList;
        }

        public static string GetOrderByString(int publishmentSystemId, string orderValue, ETableStyle tableStyle, ETaxisType defaultType)
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
            ArrayList orderedContentIdArrayList = null;
            if (taxisType == ETaxisType.OrderByHits)
            {
                if (tableStyle == ETableStyle.Channel)
                {
                    orderedContentIdArrayList = DataProvider.TrackingDao.GetPageNodeIdArrayListByAccessNum(publishmentSystemId);
                }
            }
            else if (taxisType == ETaxisType.OrderByStars)
            {
                orderedContentIdArrayList = DataProvider.StarDao.GetContentIdArrayListByPoint(publishmentSystemId);
            }
            else if (taxisType == ETaxisType.OrderByDigg)
            {
                orderedContentIdArrayList = BaiRongDataProvider.DiggDao.GetRelatedIdentityArrayListByTotal(publishmentSystemId);
            }
            else if (taxisType == ETaxisType.OrderByComments)
            {
                orderedContentIdArrayList = DataProvider.CommentDao.GetContentIdArrayListByCount(publishmentSystemId);
            }
            return ETaxisTypeUtils.GetOrderByString(tableStyle, taxisType, orderByString, orderedContentIdArrayList);
        }

        public static string GetPageContentsSqlString(PublishmentSystemInfo publishmentSystemInfo, int channelId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var sqlString = string.Empty;

            if (DataProvider.NodeDao.IsExists(channelId))
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

                string sqlWhereString;
                if (tableStyle == ETableStyle.BackgroundContent)
                {
                    sqlWhereString = DataProvider.BackgroundContentDao.GetStlWhereString(publishmentSystemInfo, tableName, groupContent, groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                }
                else
                {
                    sqlWhereString = BaiRongDataProvider.ContentDao.GetStlWhereString(publishmentSystemInfo.PublishmentSystemId, groupContent, groupContentNot, tags, isTopExists, isTop, where);
                }
                sqlString = DataProvider.ContentDao.GetStlSqlStringChecked(tableName, publishmentSystemInfo.PublishmentSystemId, channelId, startNum, totalNum, orderByString, sqlWhereString, scopeType, groupChannel, groupChannelNot, isNoDup);
            }

            return sqlString;
        }

        public static IEnumerable GetContentsDataSource(PublishmentSystemInfo publishmentSystemInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection otherAttributes)
        {
            IEnumerable ie = null;

            if (DataProvider.NodeDao.IsExists(channelId))
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

                if (isRelatedContents && contentId > 0)
                {
                    var isTags = false;
                    var tagCollection = BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, ContentAttribute.Tags);
                    if (!string.IsNullOrEmpty(tagCollection))
                    {
                        var contentIdArrayList = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(TranslateUtils.StringCollectionToStringCollection(tagCollection), publishmentSystemInfo.PublishmentSystemId);
                        contentIdArrayList.Remove(contentId);
                        if (contentIdArrayList.Count > 0)
                        {
                            isTags = true;
                            if (string.IsNullOrEmpty(where))
                            {
                                where =
                                    $"ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList)})";
                            }
                            else
                            {
                                where +=
                                    $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList)}))";
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
                    sqlWhereString = DataProvider.BackgroundContentDao.GetStlWhereString(publishmentSystemInfo, tableName, groupContent, groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                }
                else
                {
                    sqlWhereString = BaiRongDataProvider.ContentDao.GetStlWhereString(publishmentSystemInfo.PublishmentSystemId, groupContent, groupContentNot, tags, isTopExists, isTop, where);
                }
                ie = DataProvider.ContentDao.GetStlDataSourceChecked(tableName, channelId, startNum, totalNum, orderByString, sqlWhereString, scopeType, groupChannel, groupChannelNot, isNoDup, otherAttributes);
            }

            return ie;
        }

        public static IEnumerable GetCommentsDataSource(int publishmentSystemId, int channelId, int contentId, DbItemContainer itemContainer, int startNum, int totalNum, bool isRecommend, string orderByString, string where)
        {
            var sqlString = DataProvider.CommentDao.GetSelectSqlStringWithChecked(publishmentSystemId, channelId, contentId, startNum, totalNum, isRecommend, where, orderByString);
            var ie = BaiRongDataProvider.DatabaseDao.GetDataSource(sqlString);

            return ie;
        }

        public static DataSet GetPageCommentsDataSet(int publishmentSystemId, int channelId, int contentId, DbItemContainer itemContainer, int startNum, int totalNum, bool isRecommend, string orderByString, string where)
        {
            var sqlString = DataProvider.CommentDao.GetSelectSqlStringWithChecked(publishmentSystemId, channelId, contentId, startNum, totalNum, isRecommend, where, orderByString);
            var dataSet = BaiRongDataProvider.DatabaseDao.GetDataSet(sqlString);

            return dataSet;
        }

        public static IEnumerable GetInputContentsDataSource(int publishmentSystemId, int inputId, ContentsDisplayInfo displayInfo)
        {
            var isReplyExists = displayInfo.OtherAttributes[StlInputContents.Attribute_IsReply] != null;
            var isReply = TranslateUtils.ToBool(displayInfo.OtherAttributes[StlInputContents.Attribute_IsReply]);
            var sqlString = DataProvider.InputContentDao.GetSelectSqlStringWithChecked(publishmentSystemId, inputId, isReplyExists, isReply, displayInfo.StartNum, displayInfo.TotalNum, displayInfo.Where, displayInfo.OrderByString, displayInfo.OtherAttributes);
            return BaiRongDataProvider.DatabaseDao.GetDataSource(sqlString);
        }

        public static DataSet GetPageInputContentsDataSet(int publishmentSystemId, int inputId, ContentsDisplayInfo displayInfo)
        {
            var isReplyExists = displayInfo.OtherAttributes[StlInputContents.Attribute_IsReply] != null;
            var isReply = TranslateUtils.ToBool(displayInfo.OtherAttributes[StlInputContents.Attribute_IsReply]);
            var sqlString = DataProvider.InputContentDao.GetSelectSqlStringWithChecked(publishmentSystemId, inputId, isReplyExists, isReply, displayInfo.StartNum, displayInfo.TotalNum, displayInfo.Where, displayInfo.OrderByString, displayInfo.OtherAttributes);
            return BaiRongDataProvider.DatabaseDao.GetDataSet(sqlString);
        }

        public static IEnumerable GetChannelsDataSource(int publishmentSystemId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            IEnumerable ie = null;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = DataProvider.NodeDao.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where);
                ie = DataProvider.NodeDao.GetStlDataSourceByPublishmentSystemId(publishmentSystemId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                if (nodeInfo != null)
                {
                    var sqlWhereString = DataProvider.NodeDao.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where);
                    ie = DataProvider.NodeDao.GetStlDataSource(nodeInfo, startNum, totalNum, sqlWhereString, scopeType, orderByString);
                }
            }

            return ie;
        }

        public static DataSet GetPageChannelsDataSet(int publishmentSystemId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            DataSet dataSet = null;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = DataProvider.NodeDao.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where);
                dataSet = DataProvider.NodeDao.GetStlDataSetByPublishmentSystemId(publishmentSystemId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                if (nodeInfo != null)
                {
                    var sqlWhereString = DataProvider.NodeDao.GetWhereString(publishmentSystemId, group, groupNot, isImageExists, isImage, where);
                    dataSet = DataProvider.NodeDao.GetStlDataSet(nodeInfo, startNum, totalNum, sqlWhereString, scopeType, orderByString);
                }
            }
            return dataSet;
        }

        public static IEnumerable GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return BaiRongDataProvider.DatabaseDao.GetDataSource(connectionString, sqlString);
        }

        public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return BaiRongDataProvider.DatabaseDao.GetDataSet(connectionString, sqlString);
        }

        public static IEnumerable GetSitesDataSource(string siteName, string directory, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString, string since)
        {
            return DataProvider.PublishmentSystemDao.GetStlDataSource(siteName, directory, startNum, totalNum, whereString, scopeType, orderByString, since);
        }

        public static IEnumerable GetPhotosDataSource(PublishmentSystemInfo publishmentSystemInfo, int contentId, int startNum, int totalNum)
        {
            return DataProvider.PhotoDao.GetStlDataSource(publishmentSystemInfo.PublishmentSystemId, contentId, startNum, totalNum);
        }

        public static IEnumerable GetDataSourceByStlElement(PublishmentSystemInfo publishmentSystemInfo, int templateId, string elementName, string stlElement)
        {
            var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, false);
            XmlNode node = xmlDocument.DocumentElement;
            if (node != null)
            {
                node = node.FirstChild;

                var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemInfo.PublishmentSystemId, templateId);
                var pageInfo = new PageInfo(publishmentSystemInfo.PublishmentSystemId, 0, publishmentSystemInfo, templateInfo, null);
                var contextInfo = new ContextInfo(pageInfo);

                if (node?.Name != null)
                {
                    if (elementName == StlChannels.ElementName)
                    {
                        var displayInfo = ContentsDisplayInfo.GetContentsDisplayInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Channel);

                        return StlChannels.GetDataSource(pageInfo, contextInfo, displayInfo);
                    }
                    else if (elementName == StlContents.ElementName)
                    {
                        var displayInfo = ContentsDisplayInfo.GetContentsDisplayInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Content);

                        return StlContents.GetDataSource(pageInfo, contextInfo, displayInfo);
                    }
                }
            }

            return null;
        }
    }
}
