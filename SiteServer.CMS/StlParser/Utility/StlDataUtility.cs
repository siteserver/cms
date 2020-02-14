using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.StlParser.Utility
{
    public static class StlDataUtility
    {
        public static async Task<int> GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retVal = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await DataProvider.ChannelRepository.GetChannelIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await DataProvider.ChannelRepository.GetChannelIdByParentIdAndChannelNameAsync(siteId, retVal, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await DataProvider.ChannelRepository.GetChannelIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
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
            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (nodeInfo != null)
            {
                if (topLevel >= 0)
                {
                    if (topLevel > 0)
                    {
                        if (topLevel < nodeInfo.ParentsCount)
                        {
                            var parentIdStrList = Utilities.GetStringList(nodeInfo.ParentsPath);
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
                        var parentIdStrList = Utilities.GetStringList(nodeInfo.ParentsPath);
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

        public static async Task<List<int>> GetChannelIdListAsync(int siteId, int channelId, string orderByString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isImageExists, bool isImage, int totalNum, string where)
        {
            var whereString = DataProvider.ChannelRepository.GetWhereString(groupChannel, groupChannelNot, isImageExists, isImage, where);
            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return await DataProvider.ChannelRepository.GetIdListByTotalNumAsync(channelIdList, totalNum, orderByString, whereString);
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

        public static TaxisType GetETaxisTypeByOrder(string order, bool isChannel, TaxisType defaultType)
        {
            var taxisType = defaultType;
            if (!string.IsNullOrEmpty(order))
            {
                if (isChannel)
                {
                    if (order.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = TaxisType.OrderByTaxis;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = TaxisType.OrderByTaxisDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = TaxisType.OrderByAddDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = TaxisType.OrderByAddDateDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                    {
                        taxisType = TaxisType.OrderByHits;
                    }
                }
                else
                {
                    if (order.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                    {
                        taxisType = TaxisType.OrderByTaxisDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                    {
                        taxisType = TaxisType.OrderByTaxis;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                    {
                        taxisType = TaxisType.OrderByAddDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = TaxisType.OrderByAddDateDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderLastEditDate.ToLower()))
                    {
                        taxisType = TaxisType.OrderByLastEditDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = TaxisType.OrderByLastEditDateDesc;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                    {
                        taxisType = TaxisType.OrderByHits;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHitsByDay.ToLower()))
                    {
                        taxisType = TaxisType.OrderByHitsByDay;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHitsByWeek.ToLower()))
                    {
                        taxisType = TaxisType.OrderByHitsByWeek;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderHitsByMonth.ToLower()))
                    {
                        taxisType = TaxisType.OrderByHitsByMonth;
                    }
                }
            }
            return taxisType;
        }

        public static string GetChannelOrderByString(int siteId, string orderValue, TaxisType defaultType)
        {
            var taxisType = defaultType;
            var orderByString = string.Empty;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (orderValue.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
                else
                {
                    orderByString = orderValue;
                }
            }
            
            return ETaxisTypeUtils.GetChannelOrderByString(taxisType, orderByString, null);
        }

        public static string GetContentOrderByString(int siteId, string orderValue, TaxisType defaultType)
        {
            var taxisType = defaultType;
            var orderByString = string.Empty;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (orderValue.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderLastEditDate.ToLower()))
                {
                    taxisType = TaxisType.OrderByLastEditDateDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderLastEditDateBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByLastEditDate;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderHitsByDay.ToLower()))
                {
                    taxisType = TaxisType.OrderByHitsByDay;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderHitsByWeek.ToLower()))
                {
                    taxisType = TaxisType.OrderByHitsByWeek;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderHitsByMonth.ToLower()))
                {
                    taxisType = TaxisType.OrderByHitsByMonth;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                {
                    taxisType = TaxisType.OrderByRandom;
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
            if (!await DataProvider.ChannelRepository.IsExistsAsync(channelId)) return string.Empty;

            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, nodeInfo);

            var sqlWhereString = await DataProvider.ChannelRepository.IsContentModelPluginAsync(site, nodeInfo)
                ? await DataProvider.ContentRepository.GetStlWhereStringAsync(site.Id, listInfo.GroupContent, listInfo.GroupContentNot,
                    listInfo.Tags, listInfo.IsTopExists, listInfo.IsTop, listInfo.Where)
                : await DataProvider.ContentRepository.GetStlWhereStringAsync(site.Id, listInfo.GroupContent,
                    listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile,
                    listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor,
                    listInfo.Where);

            return await DataProvider.ContentRepository.GetStlSqlStringCheckedAsync(tableName, site.Id, channelId, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, sqlWhereString, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot);
        }

        public static string GetPageContentsSqlStringBySearch(string tableName, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var sqlWhereString = DataProvider.ContentRepository.GetStlWhereStringBySearch(groupContent, groupContentNot, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
            var sqlString = DataProvider.ContentRepository.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum, orderByString, sqlWhereString);

            return sqlString;
        }

        public static async Task<DataSet> GetContentsDataSourceAsync(Site site, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!await DataProvider.ChannelRepository.IsExistsAsync(channelId)) return null;

            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, nodeInfo);

            if (isRelatedContents && contentId > 0)
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

            var sqlWhereString = await PluginManager.IsExistsAsync(nodeInfo.ContentModelPluginId)
                ? await DataProvider.ContentRepository.GetStlWhereStringAsync(site.Id, groupContent, groupContentNot,
                    tags, isTopExists, isTop, where)
                : await DataProvider.ContentRepository.GetStlWhereStringAsync(site.Id, groupContent,
                    groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile,
                    isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor,
                    where);

            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(nodeInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return await DataProvider.ContentRepository.GetStlDataSourceCheckedAsync(channelIdList, tableName, startNum, totalNum, orderByString, sqlWhereString, others);
        }

        public static async Task<List<ContentSummary>> GetMinContentInfoListAsync(Site site, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            var dataSource = await GetContentsDataSourceAsync(site, channelId, contentId, groupContent, groupContentNot, tags,
                isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isRelatedContents, startNum,
                totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot,
                isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, others);

            var list = new List<ContentSummary>();

            foreach (DataRow dataItem in dataSource.Tables[0].Rows)
            {
                var minContentInfo = new ContentSummary
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
                var sqlWhereString = DataProvider.ChannelRepository.GetWhereString(group, groupNot, isImageExists, isImage, where);
                ie = DataProvider.ChannelRepository.GetStlDataSourceBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var sqlWhereString = DataProvider.ChannelRepository.GetWhereString(group, groupNot, isImageExists, isImage, where);
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(siteId, channelId, scopeType);
                //ie = DataProvider.ChannelRepository.GetStlDataSource(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
                ie = DataProvider.ChannelRepository.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
            }

            return ie;
        }

        public static async Task<DataSet> GetPageChannelsDataSetAsync(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        {
            DataSet dataSet;

            if (isTotal)//从所有栏目中选择
            {
                var sqlWhereString = DataProvider.ChannelRepository.GetWhereString(group, groupNot, isImageExists, isImage, where);
                dataSet = DataProvider.ChannelRepository.GetStlDataSetBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
            }
            else
            {
                var sqlWhereString = DataProvider.ChannelRepository.GetWhereString(group, groupNot, isImageExists, isImage, where);
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(siteId, channelId, scopeType);
                dataSet = DataProvider.ChannelRepository.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
            }
            return dataSet;
        }

        public static DataSet GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = DataProvider.DatabaseRepository.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return DataProvider.DatabaseRepository.GetDataSet(connectionString, sqlString);
        }

        public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = DataProvider.DatabaseRepository.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return DataProvider.DatabaseRepository.GetDataSet(connectionString, sqlString);
        }

        public static async Task<IDataReader> GetSitesDataSourceAsync(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            return await DataProvider.SiteRepository.GetStlDataSourceAsync(siteName, siteDir, startNum, totalNum, whereString, scopeType, orderByString);
        }
    }
}
