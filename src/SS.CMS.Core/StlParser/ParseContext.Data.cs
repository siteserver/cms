using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using SS.CMS.Core.Services;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser
{
    public partial class ParseContext
    {
        public async Task<string> GetStlCurrentUrlAsync(SiteInfo siteInfo, int channelId, int contentId, ContentInfo contentInfo, TemplateType templateType, int templateId, bool isLocal)
        {
            var currentUrl = string.Empty;
            if (templateType == TemplateType.IndexPageTemplate)
            {
                currentUrl = UrlManager.GetWebUrl(siteInfo);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                if (contentInfo == null)
                {
                    var nodeInfo = await ChannelRepository.GetChannelInfoAsync(channelId);
                    currentUrl = await UrlManager.GetContentUrlAsync(siteInfo, nodeInfo, contentId, isLocal);
                }
                else
                {
                    currentUrl = await UrlManager.GetContentUrlAsync(siteInfo, contentInfo, isLocal);
                }
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                currentUrl = await UrlManager.GetChannelUrlAsync(siteInfo, await ChannelRepository.GetChannelInfoAsync(channelId), isLocal);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                currentUrl = await UrlManager.GetFileUrlAsync(siteInfo, templateId, isLocal);
            }
            //currentUrl是当前页面的地址，前后台分离的时候，不允许带上protocol
            //return PageUtils.AddProtocolToUrl(currentUrl);
            return currentUrl;
        }

        public async Task<int> GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retval = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await ChannelRepository.GetChannelIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await ChannelRepository.GetChannelIdByParentIdAndChannelNameAsync(siteId, retval, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await ChannelRepository.GetChannelIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }

            return retval;
        }

        public async Task<int> GetChannelIdByLevelAsync(int siteId, int channelId, int upLevel, int topLevel)
        {
            var theChannelId = channelId;
            var channelInfo = await ChannelRepository.GetChannelInfoAsync(channelId);
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

        //public int GetChannelIdByChannelIDOrChannelIndexOrChannelName(int siteID, int channelID, string channelIndex, string channelName)
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

        public TaxisType GetETaxisTypeByOrder(string order, bool isChannel, TaxisType defaultType)
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
                        taxisType = TaxisType.OrderByLastModifiedDate;
                    }
                    else if (order.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                    {
                        taxisType = TaxisType.OrderByLastModifiedDateDesc;
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

        public TaxisType GetChannelTaxisType(string orderValue, TaxisType defaultType)
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

        public TaxisType GetContentTaxisType(int siteId, string orderValue, TaxisType defaultType)
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
                    taxisType = TaxisType.OrderByLastModifiedDateDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderLastEditDateBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByLastModifiedDate;
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

        public async Task<List<ContentInfo>> GetStlPageContentsSqlStringAsync(SiteInfo siteInfo, int channelId, ListInfo listInfo)
        {
            if (!await ChannelRepository.IsExistsAsync(channelId)) return new List<ContentInfo>();

            var channelInfo = await ChannelRepository.GetChannelInfoAsync(channelId);

            var query = await ChannelRepository.IsContentModelPluginAsync(PluginManager, siteInfo, channelInfo)
                ? await channelInfo.ContentRepository.GetStlWhereStringAsync(siteInfo.Id, channelInfo, listInfo.GroupContent, listInfo.GroupContentNot,
                    listInfo.Tags, listInfo.IsTop, false, 0)
                : await channelInfo.ContentRepository.GetStlWhereStringAsync(siteInfo.Id, channelInfo, listInfo.GroupContent,
                    listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImage, listInfo.IsVideo, listInfo.IsFile, listInfo.IsTop, listInfo.IsRecommend, listInfo.IsHot, listInfo.IsColor, false, 0);

            // return await channelInfo.ContentRepository.GetStlSqlStringCheckedAsync(siteInfo.Id, channelInfo, listInfo.StartNum, listInfo.TotalNum, listInfo.Order, query, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot);
            return null;
        }

        public List<ContentInfo> GetPageContentsSqlStringBySearch(ChannelInfo channelInfo, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, int startNum, int totalNum, string order, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor)
        {
            var query = channelInfo.ContentRepository.GetStlWhereStringBySearch(groupContent, groupContentNot, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor);
            var sqlString = channelInfo.ContentRepository.GetStlSqlStringCheckedBySearch(startNum, totalNum, order, query);

            return sqlString;
        }

        public async Task<List<ContentInfo>> GetContentsDataSourceAsync(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool isRelatedContents, int startNum, int totalNum, TaxisType taxisType, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!await ChannelRepository.IsExistsAsync(channelId)) return null;

            var channelInfo = await ChannelRepository.GetChannelInfoAsync(channelId);

            var sqlWhereString = await PluginManager.IsExistsAsync(channelInfo.ContentModelPluginId)
                ? await channelInfo.ContentRepository.GetStlWhereStringAsync(siteInfo.Id, channelInfo, groupContent, groupContentNot,
                    tags, isTop, isRelatedContents, contentId)
                : await channelInfo.ContentRepository.GetStlWhereStringAsync(siteInfo.Id, channelInfo, groupContent,
                    groupContentNot, tags, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor,
                    isRelatedContents, contentId);

            var channelIdList = await ChannelRepository.GetChannelIdListAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return await channelInfo.ContentRepository.GetStlDataSourceCheckedAsync(channelIdList, startNum, totalNum, taxisType, sqlWhereString, others);
        }

        public async Task<List<KeyValuePair<int, ContentInfo>>> GetContainerContentListAsync(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool isRelatedContents, int startNum, int totalNum, string order, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            if (!await ChannelRepository.IsExistsAsync(channelId)) return null;

            var channelInfo = await ChannelRepository.GetChannelInfoAsync(channelId);

            var sqlWhereString = await PluginManager.IsExistsAsync(channelInfo.ContentModelPluginId)
                ? await siteInfo.ContentRepository.GetStlWhereStringAsync(siteInfo.Id, channelInfo, groupContent, groupContentNot,
                    tags, isTop, isRelatedContents, contentId)
                : await siteInfo.ContentRepository.GetStlWhereStringAsync(siteInfo.Id, channelInfo, groupContent,
                    groupContentNot, tags, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor,
              isRelatedContents, contentId);

            var channelIdList = await ChannelRepository.GetChannelIdListAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return await siteInfo.ContentRepository.GetContainerContentListCheckedAsync(channelIdList, startNum, totalNum, order, sqlWhereString, others);
        }

        public async Task<List<ContentInfo>> GetMinContentInfoListAsync(SiteInfo siteInfo, int channelId, int contentId, string groupContent, string groupContentNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool isRelatedContents, int startNum, int totalNum, TaxisType taxisType, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ScopeType scopeType, string groupChannel, string groupChannelNot, NameValueCollection others)
        {
            var dataSource = await GetContentsDataSourceAsync(siteInfo, channelId, contentId, groupContent, groupContentNot, tags,
                isImage, isVideo, isFile, isRelatedContents, startNum,
                totalNum, taxisType, isTop, isRecommend, isHot, isColor, scopeType, groupChannel, groupChannelNot, others);

            return dataSource;
        }

        // public DataSet GetChannelsDataSource(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, string order, EScopeType scopeType, bool isTotal, string where)
        // {
        //     DataSet ie;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         ie = StlChannelCache.GetStlDataSourceBySiteId(siteId, startNum, totalNum, sqlWhereString, order);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelRepository.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         var channelIdList = ChannelRepository.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         //ie = DataProvider.ChannelDao.GetStlDataSource(channelIdList, startNum, totalNum, sqlWhereString, order);
        //         ie = StlChannelCache.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, order);
        //     }

        //     return ie;
        // }

        // public DataSet GetPageChannelsDataSet(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, string order, EScopeType scopeType, bool isTotal, string where)
        // {
        //     DataSet dataSet;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         dataSet = DataProvider.ChannelDao.GetStlDataSetBySiteId(siteId, startNum, totalNum, sqlWhereString, order);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelRepository.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = StlChannelCache.GetWhereString(siteId, group, groupNot, isImageExists, isImage, where);
        //         var channelIdList = ChannelRepository.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         dataSet = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, sqlWhereString, order);
        //     }
        //     return dataSet;
        // }

        // public List<Container.Channel> GetPageContainerChannelList(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, string order, EScopeType scopeType, bool isTotal)
        // {
        //     List<Container.Channel> list;

        //     if (isTotal)//从所有栏目中选择
        //     {
        //         var sqlWhereString = DataProvider.ChannelDao.GetWhereString(group, groupNot, isImageExists, isImage);
        //         list = DataProvider.ChannelDao.GetContainerChannelListBySiteId(siteId, startNum, totalNum, sqlWhereString, order);
        //     }
        //     else
        //     {
        //         var channelInfo = ChannelRepository.GetChannelInfo(siteId, channelId);
        //         if (channelInfo == null) return null;

        //         var sqlWhereString = DataProvider.ChannelDao.GetWhereString(group, groupNot, isImageExists, isImage);
        //         var channelIdList = ChannelRepository.GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        //         list = DataProvider.ChannelDao.GetContainerChannelList(channelIdList, startNum, totalNum, sqlWhereString, order);
        //     }
        //     return list;
        // }

        public List<KeyValuePair<int, Dictionary<string, object>>> GetContainerSqlList(string connectionString, string queryString, int startNum, int totalNum, string order)
        {
            var sqlString = CacheManager.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
            var db = new Database(SettingsManager.DatabaseType, connectionString);
            return CacheManager.GetContainerSqlList(db, sqlString);
        }

        public List<KeyValuePair<int, Dictionary<string, object>>> GetPageContainerSqlList(string connectionString, string pageSqlString)
        {
            var db = new Database(SettingsManager.DatabaseType, SettingsManager.DatabaseConnectionString);
            return CacheManager.GetContainerSqlList(db, pageSqlString);
        }

        public DataSet GetSqlContentsDataSource(string connectionString, string queryString, int startNum, int totalNum, string order)
        {
            var sqlString = CacheManager.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
            return CacheManager.GetDataSet(connectionString, sqlString);
        }

        // public DataSet GetPageSqlContentsDataSet(string connectionString, string queryString, int startNum, int totalNum, string order)
        // {
        //     var sqlString = CacheManager.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
        //     return DatabaseUtils.GetDataSet(connectionString, sqlString);
        // }

        // public IDataReader GetSitesDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string order)
        // {
        //     return DataProvider.SiteDao.GetStlDataSource(siteName, siteDir, startNum, totalNum, whereString, scopeType, order);
        // }

        public async Task<List<KeyValuePair<int, SiteInfo>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string order)
        {
            return await SiteRepository.GetContainerSiteListAsync(siteName, siteDir, startNum, totalNum, scopeType, order);
        }
    }
}
