using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.Plugin;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.StlParser.Utility
{
    public class StlDataUtility
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


        public static ETaxisType GetChannelTaxisType(string orderValue, ETaxisType defaultType)
        {
            var taxisType = defaultType;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderDefault))
                {
                    taxisType = ETaxisType.OrderByTaxis;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderBack))
                {
                    taxisType = ETaxisType.OrderByTaxisDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDate))
                {
                    taxisType = ETaxisType.OrderByAddDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDateBack))
                {
                    taxisType = ETaxisType.OrderByAddDate;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHits))
                {
                    taxisType = ETaxisType.OrderByHits;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderRandom))
                {
                    taxisType = ETaxisType.OrderByRandom;
                }
            }

            return taxisType;
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

        public static string GetStlPageContentsSqlString(SiteInfo siteInfo, int channelId, ListInfo listInfo)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return string.Empty;

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var sqlWhereString = ChannelManager.IsContentModelPlugin(siteInfo, channelInfo)
                ? StlContentCache.GetStlWhereString(siteInfo.Id, listInfo.GroupContent, listInfo.GroupContentNot,
                    listInfo.Tags, listInfo.IsTopExists, listInfo.IsTop, channelInfo, false, 0)
                : StlContentCache.GetStlWhereString(siteInfo.Id, listInfo.GroupContent,
                    listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile,
                    listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, channelInfo, false, 0);

            return StlContentCache.GetStlSqlStringChecked(siteInfo.Id, channelInfo, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, sqlWhereString, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot);
        }

        public static string GetPageContentsSqlStringBySearch(ChannelInfo channelInfo, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var sqlWhereString = StlContentCache.GetStlWhereStringBySearch(channelInfo, groupContent, groupContentNot, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
            var sqlString = StlContentCache.GetStlSqlStringCheckedBySearch(channelInfo, startNum, totalNum, orderByString, sqlWhereString);

            return sqlString;
        }

        public static DataSet GetContentsDataSource(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return null;

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var sqlWhereString = PluginManager.IsExists(channelInfo.ContentModelPluginId)
                ? StlContentCache.GetStlWhereString(siteInfo.Id, groupContent, groupContentNot,
                    tags, isTopExists, isTop, channelInfo, isRelatedContents, contentId)
                : StlContentCache.GetStlWhereString(siteInfo.Id, groupContent,
                    groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile,
                    isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor,
                    channelInfo, isRelatedContents, contentId);

            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return StlContentCache.GetStlDataSourceChecked(channelIdList, channelInfo, startNum, totalNum, orderByString, sqlWhereString, others);
        }

        public static List<Container.Content> GetContainerContentList(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!ChannelManager.IsExists(siteInfo.Id, channelId)) return null;

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var sqlWhereString = PluginManager.IsExists(channelInfo.ContentModelPluginId)
                ? StlContentCache.GetStlWhereString(siteInfo.Id, groupContent, groupContentNot,
                    tags, isTopExists, isTop, channelInfo, isRelatedContents, contentId)
                : StlContentCache.GetStlWhereString(siteInfo.Id, groupContent,
                    groupContentNot, tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile,
                    isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor,
                    channelInfo, isRelatedContents, contentId);

            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return StlContentCache.GetContainerContentListChecked(channelIdList, channelInfo, startNum, totalNum, orderByString, sqlWhereString, others);
        }

        public static List<MinContentInfo> GetMinContentInfoList(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum, int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, EScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            var dataSource = GetContentsDataSource(siteInfo, channelId, contentId, groupContent, groupContentNot, tags,
                isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isRelatedContents, startNum,
                totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot,
                isColorExists, isColor, scopeType, groupChannel, groupChannelNot, others);

            var list = new List<MinContentInfo>();

            foreach (DataRow dataItem in dataSource.Tables[0].Rows)
            {
                var minContentInfo = new MinContentInfo
                {
                    Id = (int)dataItem[ContentAttribute.Id],
                    ChannelId = (int)dataItem[ContentAttribute.ChannelId]
                };
                list.Add(minContentInfo);
            }

            return list;
        }

        // public static DataSet GetChannelsDataSource(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        // {
        //     DataSet ie;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         ie = StlChannelCache.GetStlDataSourceBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         //ie = DataProvider.ChannelDao.GetStlDataSource(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
        //         ie = StlChannelCache.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
        //     }

        //     return ie;
        // }

        // public static DataSet GetPageChannelsDataSet(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal, string where)
        // {
        //     DataSet dataSet;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         dataSet = DataProvider.ChannelDao.GetStlDataSetBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         dataSet = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
        //     }
        //     return dataSet;
        // }

        // public static List<Container.Channel> GetPageContainerChannelList(int siteId, int channelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, string orderByString, EScopeType scopeType, bool isTotal)
        // {
        //     List<Container.Channel> list;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = DataProvider.ChannelDao.GetWhereString(group, groupNot, isImageExists, isImage);
        //         list = DataProvider.ChannelDao.GetContainerChannelListBySiteId(siteId, startNum, totalNum, sqlWhereString, orderByString);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = DataProvider.ChannelDao.GetWhereString(group, groupNot, isImageExists, isImage);
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         list = DataProvider.ChannelDao.GetContainerChannelList(channelIdList, startNum, totalNum, sqlWhereString, orderByString);
        //     }
        //     return list;
        // }

        public static List<Container.Sql> GetContainerSqlList(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            var dbContext = new DbContext(AppSettings.DbContext.DatabaseType, connectionString);
            return StlDatabaseCache.GetContainerSqlList(dbContext, sqlString);
        }

        public static List<Container.Sql> GetPageContainerSqlList(string connectionString, string pageSqlString)
        {
            var dbContext = new DbContext(AppSettings.DbContext.DatabaseType, connectionString);
            return StlDatabaseCache.GetContainerSqlList(dbContext, pageSqlString);
        }

        public static DataSet GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return StlDatabaseCache.GetDataSet(connectionString, sqlString);
        }

        public static DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            var sqlString = StlSqlContentsCache.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            return DatabaseUtils.GetDataSet(connectionString, sqlString);
        }

        // public static IDataReader GetSitesDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        // {
        //     return DataProvider.SiteDao.GetStlDataSource(siteName, siteDir, startNum, totalNum, whereString, scopeType, orderByString);
        // }

        public static List<Container.Site> GetContainerSiteList(string siteName, string siteDir, int startNum, int totalNum, EScopeType scopeType, string orderByString)
        {
            return DataProvider.SiteDao.GetContainerSiteList(siteName, siteDir, startNum, totalNum, scopeType, orderByString);
        }
    }
}
