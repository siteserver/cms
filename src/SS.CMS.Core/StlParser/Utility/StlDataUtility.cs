using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.Utility
{
    public static class StlDataUtility
    {
        public static int GetChannelIdByChannelIdOrChannelIndexOrChannelName(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retval = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = ChannelManager.GetChannelIdByIndexName(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = ChannelManager.GetChannelIdByParentIdAndChannelName(siteId, retval, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = ChannelManager.GetChannelIdByParentIdAndChannelName(siteId, siteId, channelName, true);
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
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (channelInfo != null)
            {
                if (topLevel >= 0)
                {
                    if (topLevel > 0)
                    {
                        if (topLevel < channelInfo.ParentsCount)
                        {
                            var parentIdStrList = TranslateUtils.StringCollectionToStringList(channelInfo.ParentsPath);
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
                    if (upLevel < channelInfo.ParentsCount)
                    {
                        var parentIdStrList = TranslateUtils.StringCollectionToStringList(channelInfo.ParentsPath);
                        if (parentIdStrList[upLevel] != null)
                        {
                            var parentIdStr = parentIdStrList[channelInfo.ParentsCount - upLevel];
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

        public static TaxisType GetChannelTaxisType(string orderValue, TaxisType defaultType)
        {
            var taxisType = defaultType;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderDefault))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderBack))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDate))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDateBack))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHits))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderRandom))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
            }

            return taxisType;
        }

        public static TaxisType GetContentTaxisType(int siteId, string orderValue, TaxisType defaultType)
        {
            var taxisType = defaultType;
            var order = string.Empty;
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
            }

            return taxisType;
        }

        public static List<ContentInfo> GetStlPageContentsSqlString(IPluginManager pluginManager, SiteInfo siteInfo, int channelId, ListInfo listInfo)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return new List<ContentInfo>();

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var query = ChannelManager.IsContentModelPlugin(pluginManager, siteInfo, channelInfo)
                ? channelInfo.ContentRepository.StlGetStlWhereString(siteInfo.Id, listInfo.GroupContent, listInfo.GroupContentNot,
                    listInfo.Tags, listInfo.IsTop, channelInfo, false, 0)
                : channelInfo.ContentRepository.StlGetStlWhereString(siteInfo.Id, listInfo.GroupContent,
                    listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImage, listInfo.IsVideo, listInfo.IsFile, listInfo.IsTop, listInfo.IsRecommend, listInfo.IsHot, listInfo.IsColor, channelInfo, false, 0);

            return channelInfo.ContentRepository.StlGetStlSqlStringChecked(siteInfo.Id, channelInfo, listInfo.StartNum, listInfo.TotalNum, listInfo.Order, query, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot);
        }

        public static List<ContentInfo> GetPageContentsSqlStringBySearch(ChannelInfo channelInfo, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, int startNum, int totalNum, string order, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor)
        {
            var query = channelInfo.ContentRepository.StlGetStlWhereStringBySearch(channelInfo, groupContent, groupContentNot, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor);
            var sqlString = channelInfo.ContentRepository.StlGetStlSqlStringCheckedBySearch(channelInfo, startNum, totalNum, order, query);

            return sqlString;
        }

        public static List<ContentInfo> GetContentsDataSource(IPluginManager pluginManager, SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool isRelatedContents, int startNum, int totalNum, TaxisType taxisType, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return null;

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var sqlWhereString = pluginManager.IsExists(channelInfo.ContentModelPluginId)
                ? channelInfo.ContentRepository.StlGetStlWhereString(siteInfo.Id, groupContent, groupContentNot,
                    tags, isTop, channelInfo, isRelatedContents, contentId)
                : channelInfo.ContentRepository.StlGetStlWhereString(siteInfo.Id, groupContent,
                    groupContentNot, tags, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor,
                    channelInfo, isRelatedContents, contentId);

            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return channelInfo.ContentRepository.StlGetStlDataSourceChecked(channelIdList, channelInfo, startNum, totalNum, taxisType, sqlWhereString, others);
        }

        public static List<KeyValuePair<int, ContentInfo>> GetContainerContentList(IPluginManager pluginManager, SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool isRelatedContents, int startNum, int totalNum, string order, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return null;

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var sqlWhereString = pluginManager.IsExists(channelInfo.ContentModelPluginId)
                ? siteInfo.ContentRepository.StlGetStlWhereString(siteInfo.Id, groupContent, groupContentNot,
                    tags, isTop, channelInfo, isRelatedContents, contentId)
                : siteInfo.ContentRepository.StlGetStlWhereString(siteInfo.Id, groupContent,
                    groupContentNot, tags, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor,
                    channelInfo, isRelatedContents, contentId);

            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return siteInfo.ContentRepository.StlGetContainerContentListChecked(channelIdList, channelInfo, startNum, totalNum, order, sqlWhereString, others);
        }

        public static List<ContentInfo> GetMinContentInfoList(IPluginManager pluginManager, SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool isRelatedContents, int startNum, int totalNum, TaxisType taxisType, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            var dataSource = GetContentsDataSource(pluginManager, siteInfo, channelId, contentId, groupContent, groupContentNot, tags,
                isImage, isVideo, isFile, isRelatedContents, startNum,
                totalNum, taxisType, isTop, isRecommend, isHot, isColor, scopeType, groupChannel, groupChannelNot, others);

            return dataSource;
        }

        // public static DataSet GetChannelsDataSource(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, string order, EScopeType scopeType, bool isTotal, string where)
        // {
        //     DataSet ie;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         ie = StlChannelCache.GetStlDataSourceBySiteId(siteId, startNum, totalNum, sqlWhereString, order);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         //ie = DataProvider.ChannelDao.GetStlDataSource(channelIdList, startNum, totalNum, sqlWhereString, order);
        //         ie = StlChannelCache.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, order);
        //     }

        //     return ie;
        // }

        // public static DataSet GetPageChannelsDataSet(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, string order, EScopeType scopeType, bool isTotal, string where)
        // {
        //     DataSet dataSet;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         dataSet = DataProvider.ChannelDao.GetStlDataSetBySiteId(siteId, startNum, totalNum, sqlWhereString, order);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         dataSet = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, order);
        //     }
        //     return dataSet;
        // }

        // public static List<Container.Channel> GetPageContainerChannelList(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, string order, EScopeType scopeType, bool isTotal)
        // {
        //     List<Container.Channel> list;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = DataProvider.ChannelDao.GetWhereString(group, groupNot, isImageExists, isImage);
        //         list = DataProvider.ChannelDao.GetContainerChannelListBySiteId(siteId, startNum, totalNum, sqlWhereString, order);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = DataProvider.ChannelDao.GetWhereString(group, groupNot, isImageExists, isImage);
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         list = DataProvider.ChannelDao.GetContainerChannelList(channelIdList, startNum, totalNum, sqlWhereString, order);
        //     }
        //     return list;
        // }

        public static List<Container.Sql> GetContainerSqlList(ISettingsManager settingsManager, string connectionString, string queryString, int startNum, int totalNum, string order)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
            var db = new Db(settingsManager.DatabaseType, connectionString);
            return StlDatabaseCache.GetContainerSqlList(db, sqlString);
        }

        public static List<Container.Sql> GetPageContainerSqlList(ISettingsManager settingsManager, string connectionString, string pageSqlString)
        {
            var db = new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString);
            return StlDatabaseCache.GetContainerSqlList(db, pageSqlString);
        }

        public static DataSet GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string order)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
            return StlDatabaseCache.GetDataSet(connectionString, sqlString);
        }

        // public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string order)
        // {
        //     var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
        //     return DatabaseUtils.GetDataSet(connectionString, sqlString);
        // }

        // public static IDataReader GetSitesDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string order)
        // {
        //     return DataProvider.SiteDao.GetStlDataSource(siteName, siteDir, startNum, totalNum, whereString, scopeType, order);
        // }

        public static List<KeyValuePair<int, SiteInfo>> GetContainerSiteList(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string order)
        {
            return DataProvider.SiteRepository.GetContainerSiteList(siteName, siteDir, startNum, totalNum, scopeType, order);
        }
    }
}
