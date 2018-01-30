using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlDataUtility
    {
        public static int GetChannelIdByChannelIdOrChannelIndexOrChannelName(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retval = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = Node.GetIdByIndexName(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = Node.GetIdByParentIdAndChannelName(siteId, retval, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = Node.GetIdByParentIdAndChannelName(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }

            return retval;
        }

        public static int GetChannelIdByLevel(int siteId, int channelId, int upLevel, int topLevel)
        {
            var theChannelId = channelId;
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
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
                                theChannelId = int.Parse(parentIdStr);
                            }
                        }
                    }
                    else
                    {
                        theChannelId = siteId;
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
                            theChannelId = int.Parse(parentIdStr);
                        }
                    }
                    else
                    {
                        theChannelId = siteId;
                    }
                }
            }
            return theChannelId;
        }

        public static List<int> GetChannelIdList(int siteId, int channelId, string orderByString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isImageExists, bool isImage, int totalNum, string where)
        {
            var whereString = Node.GetWhereString(siteId, groupChannel, groupChannelNot, isImageExists, isImage, where);
            var channelIdList = Node.GetIdListByScopeType(channelId, scopeType, groupChannel, groupChannelNot);
            return Node.GetIdListByTotalNum(channelIdList, totalNum, orderByString, whereString);
        }

        //public static int GetChannelIdByChannelIDOrChannelIndexOrChannelName(int siteID, int channelID, string channelIndex, string channelName)
        //{
        //    int retval = channelID;
        //    if (!string.IsNullOrEmpty(channelIndex))
        //    {
        //        int theChannelId = DataProvider.NodeDAO.GetChannelIdByNodeIndexName(siteID, channelIndex);
        //        if (theChannelId != 0)
        //        {
        //            retval = theChannelId;
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(channelName))
        //    {
        //        int theChannelId = DataProvider.NodeDAO.GetChannelIdByParentIDAndNodeName(retval, channelName, true);
        //        if (theChannelId == 0)
        //        {
        //            theChannelId = DataProvider.NodeDAO.GetChannelIdByParentIDAndNodeName(siteID, channelName, true);
        //        }
        //        if (theChannelId != 0)
        //        {
        //            retval = theChannelId;
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

        public static string GetChannelOrderByString(int siteId, string orderValue, ETaxisType defaultType)
        {
            var taxisType = defaultType;
            var orderByString = string.Empty;
            if (!string.IsNullOrEmpty(orderValue))
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
            
            return ETaxisTypeUtils.GetChannelOrderByString(taxisType, orderByString, null);
        }

        public static string GetContentOrderByString(int siteId, string orderValue, ETaxisType defaultType)
        {
            var taxisType = defaultType;
            var orderByString = string.Empty;
            if (!string.IsNullOrEmpty(orderValue))
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
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                {
                    taxisType = ETaxisType.OrderByRandom;
                }
                else
                {
                    orderByString = orderValue;
                }
            }
            
            return ETaxisTypeUtils.GetContentOrderByString(taxisType, orderByString);
        }

        public static string GetStlPageContentsSqlString(SiteInfo siteInfo, int channelId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return string.Empty;

            var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);



            var sqlWhereString = ChannelManager.IsContentModelPlugin(siteInfo, nodeInfo)
                ? Content.GetStlWhereString(siteInfo.Id, groupContent, groupContentNot,
                    tags, isTopExists, isTop, where)
                : Content.GetStlWhereString(siteInfo.Id, tableName, groupContent,
                    groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile,
                    isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor,
                    where, siteInfo.Additional.IsCreateSearchDuplicate);

            return Content.GetStlSqlStringChecked(tableName, siteInfo.Id, channelId, startNum, totalNum, orderByString, sqlWhereString, scopeType, groupChannel, groupChannelNot, isNoDup);
        }

        public static string GetPageContentsSqlStringBySearch(string tableName, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var sqlWhereString = Content.GetStlWhereStringBySearch(tableName, groupContent, groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
            var sqlString = Content.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum, orderByString, sqlWhereString, isNoDup);

            return sqlString;
        }

        public static DataSet GetContentsDataSource(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isNoDup, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, LowerNameValueCollection others)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return null;

            var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);

            if (isRelatedContents && contentId > 0)
            {
                var isTags = false;
                var tagCollection = Content.GetValue(tableName, contentId, ContentAttribute.Tags);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = Tag.GetContentIdListByTagCollection(TranslateUtils.StringCollectionToStringCollection(tagCollection), siteInfo.Id);
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

            var sqlWhereString = PluginManager.IsExists(nodeInfo.ContentModelPluginId)
                ? Content.GetStlWhereString(siteInfo.Id, groupContent, groupContentNot,
                    tags, isTopExists, isTop, where)
                : Content.GetStlWhereString(siteInfo.Id, tableName, groupContent,
                    groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile,
                    isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor,
                    where, siteInfo.Additional.IsCreateSearchDuplicate);
            var channelIdList = Node.GetIdListByScopeType(channelId, scopeType, groupChannel, groupChannelNot);
            return Content.GetStlDataSourceChecked(channelIdList, tableName, startNum, totalNum, orderByString, sqlWhereString, isNoDup, others);
        }

        public static DataSet GetChannelsDataSource(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            DataSet ie;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = Node.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                ie = Node.GetStlDataSourceBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (nodeInfo == null) return null;

                var sqlWhereString = Node.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                var channelIdList = Node.GetIdListByScopeType(nodeInfo.Id, nodeInfo.ChildrenCount, scopeType,
                    string.Empty, string.Empty);
                //ie = DataProvider.ChannelDao.GetStlDataSource(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
                ie = Node.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
            }

            return ie;
        }

        public static DataSet GetPageChannelsDataSet(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            DataSet dataSet;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = Node.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                dataSet = DataProvider.ChannelDao.GetStlDataSetBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (nodeInfo == null) return null;

                var sqlWhereString = Node.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                var channelIdList = Node.GetIdListByScopeType(nodeInfo.Id, nodeInfo.ChildrenCount, scopeType, string.Empty, string.Empty);
                dataSet = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
            }
            return dataSet;
        }

        public static IDataReader GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = TableStructure.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return DataProvider.DatabaseDao.GetDataSource(connectionString, sqlString);
        }

        public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = TableStructure.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return DataProvider.DatabaseDao.GetDataSet(connectionString, sqlString);
        }

        public static IDataReader GetSitesDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString, string since)
        {
            return DataProvider.SiteDao.GetStlDataSource(siteName, siteDir, startNum, totalNum, whereString, scopeType, orderByString, since);
        }

        public static DataSet GetDataSourceByStlElement(SiteInfo siteInfo, int templateId, string elementName, string stlElement)
        {
            var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, false);
            XmlNode node = xmlDocument.DocumentElement;
            if (node == null) return null;

            node = node.FirstChild;

            var templateInfo = TemplateManager.GetTemplateInfo(siteInfo.Id, templateId);
            var pageInfo = new PageInfo(siteInfo.Id, 0, siteInfo, templateInfo);
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
