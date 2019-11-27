using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.Plugin;

namespace SiteServer.CMS.StlParser.Utility
{
    public static class StlDataUtility
    {
        public static async Task<int> GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retVal = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await ChannelManager.GetChannelIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await ChannelManager.GetChannelIdByParentIdAndChannelNameAsync(siteId, retVal, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await ChannelManager.GetChannelIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }

            return retVal;
        }

        public static async Task<int> GetChannelIdByLevelAsync(int siteId, int channelId, int upLevel, int topLevel)
        {
            var theChannelId = channelId;
            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
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

        public static async Task<IEnumerable<int>> GetChannelIdListAsync(int siteId, int channelId, string orderByString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isImageExists, bool isImage, int totalNum, string where)
        {
            var whereString = StlChannelCache.GetWhereString(siteId, groupChannel, groupChannelNot, isImageExists, isImage, where);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return await StlChannelCache.GetIdListByTotalNumAsync(channelIdList, totalNum, orderByString, whereString);
        }

        //public static int GetChannelIdByChannelIDOrChannelIndexOrChannelName(int siteID, int channelID, string channelIndex, string channelName)
        //{
        //    int retVal = channelID;
        //    if (!string.IsNullOrEmpty(channelIndex))
        //    {
        //        int theChannelId = DataProvider.NodeDAO.GetChannelIdByNodeIndexName(siteID, channelIndex);
        //        if (theChannelId != 0)
        //        {
        //            retVal = theChannelId;
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(channelName))
        //    {
        //        int theChannelId = DataProvider.NodeDAO.GetChannelIdByParentIDAndNodeName(retVal, channelName, true);
        //        if (theChannelId == 0)
        //        {
        //            theChannelId = DataProvider.NodeDAO.GetChannelIdByParentIDAndNodeName(siteID, channelName, true);
        //        }
        //        if (theChannelId != 0)
        //        {
        //            retVal = theChannelId;
        //        }
        //    }
        //    return retVal;
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

        public static async Task<string> GetStlPageContentsSqlStringAsync(Site site, int channelId, ListInfo listInfo)
        {
            if (!await ChannelManager.IsExistsAsync(site.Id, channelId)) return string.Empty;

            var nodeInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, nodeInfo);

            var sqlWhereString = await ChannelManager.IsContentModelPluginAsync(site, nodeInfo)
                ? await DataProvider.ContentDao.GetStlWhereStringAsync(site.Id, listInfo.GroupContent, listInfo.GroupContentNot,
                    listInfo.Tags, listInfo.IsTopExists, listInfo.IsTop, listInfo.Where)
                : await DataProvider.ContentDao.GetStlWhereStringAsync(site.Id, listInfo.GroupContent,
                    listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile,
                    listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor,
                    listInfo.Where);

            return await DataProvider.ContentDao.GetStlSqlStringCheckedAsync(tableName, site.Id, channelId, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, sqlWhereString, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot);
        }

        public static string GetPageContentsSqlStringBySearch(string tableName, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var sqlWhereString = DataProvider.ContentDao.GetStlWhereStringBySearch(groupContent, groupContentNot, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
            var sqlString = DataProvider.ContentDao.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum, orderByString, sqlWhereString);

            return sqlString;
        }

        public static async Task<DataSet> GetContentsDataSourceAsync(Site site, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!await ChannelManager.IsExistsAsync(site.Id, channelId)) return null;

            var nodeInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, nodeInfo);

            if (isRelatedContents && contentId > 0)
            {
                var tagCollection = DataProvider.ContentDao.GetValue(tableName, contentId, ContentAttribute.Tags);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = await StlTagCache.GetContentIdListByTagCollectionAsync(TranslateUtils.StringCollectionToStringList(tagCollection), site.Id);
                    if (contentIdList.Count > 0)
                    {
                        contentIdList.Remove(contentId);
                        
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
                else
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

            var sqlWhereString = await PluginManager.IsExistsAsync(nodeInfo.ContentModelPluginId)
                ? await DataProvider.ContentDao.GetStlWhereStringAsync(site.Id, groupContent, groupContentNot,
                    tags, isTopExists, isTop, where)
                : await DataProvider.ContentDao.GetStlWhereStringAsync(site.Id, groupContent,
                    groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile,
                    isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor,
                    where);

            var channelIdList = await ChannelManager.GetChannelIdListAsync(nodeInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return DataProvider.ContentDao.GetStlDataSourceChecked(channelIdList, tableName, startNum, totalNum, orderByString, sqlWhereString, others);
        }

        public static async Task<List<MinContentInfo>> GetMinContentInfoListAsync(Site site, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            var dataSource = await GetContentsDataSourceAsync(site, channelId, contentId, groupContent, groupContentNot, tags,
                isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isRelatedContents, startNum,
                totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot,
                isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, others);

            var list = new List<MinContentInfo>();

            foreach (DataRow dataItem in dataSource.Tables[0].Rows)
            {
                var minContentInfo = new MinContentInfo
                {
                    Id = (int) dataItem[ContentAttribute.Id],
                    ChannelId = (int) dataItem[ContentAttribute.ChannelId]
                };
                list.Add(minContentInfo);
            }

            return list;
        }

        public static async Task<DataSet> GetChannelsDataSourceAsync(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            DataSet ie;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                ie = StlChannelCache.GetStlDataSourceBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (nodeInfo == null) return null;

                var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                var channelIdList = await ChannelManager.GetChannelIdListAsync(nodeInfo, scopeType, string.Empty, string.Empty, string.Empty);
                //ie = DataProvider.ChannelDao.GetStlDataSource(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
                ie = StlChannelCache.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
            }

            return ie;
        }

        public static async Task<DataSet> GetPageChannelsDataSetAsync(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            DataSet dataSet;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                dataSet = DataProvider.ChannelDao.GetStlDataSetBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (nodeInfo == null) return null;

                var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
                var channelIdList = await ChannelManager.GetChannelIdListAsync(nodeInfo, scopeType, string.Empty, string.Empty, string.Empty);
                dataSet = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
            }
            return dataSet;
        }

        public static DataSet GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return StlDatabaseCache.GetDataSet(connectionString, sqlString);
        }

        public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return DataProvider.DatabaseDao.GetDataSet(connectionString, sqlString);
        }

        public static async Task<IDataReader> GetSitesDataSourceAsync(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            return await DataProvider.SiteDao.GetStlDataSourceAsync(siteName, siteDir, startNum, totalNum, whereString, scopeType, orderByString);
        }
    }
}
