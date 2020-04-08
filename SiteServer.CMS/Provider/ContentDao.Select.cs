using System;
using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public partial class ContentDao
    {
        private string GetCountKey(string tableName, int siteId, int channelId)
        {
            return Caching.GetCountKey(tableName, siteId, channelId);
        }

        private string GetEntityKey(string tableName, int contentId)
        {
            return Caching.GetEntityKey(tableName, contentId);
        }

        private string GetListKey(string tableName, int siteId, int channelId)
        {
            return Caching.GetListKey(tableName, siteId.ToString(), (Math.Abs(channelId)).ToString());
        }

        public void RemoveListCache(SiteInfo siteInfo, ChannelInfo channelInfo, string tableName)
        {
            var listKey = GetListKey(tableName, siteInfo.Id, channelInfo.Id);
            Caching.CacheManager.Remove(listKey);

            var countKey = GetCountKey(tableName, siteInfo.Id, channelInfo.Id);
            Caching.CacheManager.Remove(countKey);

            StlContentCache.ClearCache();
        }

        public void RemoveEntityCache(string tableName, int contentId)
        {
            var cacheKey = GetEntityKey(tableName, contentId);
            Caching.CacheManager.Remove(cacheKey);
            StlContentCache.ClearCache();
        }

        public void CacheAllListAndCount(SiteInfo site, List<ChannelSummary> summaries)
        {
            var cacheManager = Caching.CacheManager;
            foreach (var summary in summaries)
            {
                var tableName = ChannelManager.GetTableName(site, summary);
                var listKey = GetListKey(tableName, site.Id, summary.Id);
                var countKey = GetCountKey(tableName, site.Id, summary.Id);

                if (!cacheManager.Exists(listKey) && !cacheManager.Exists(countKey))
                {
                    var repository = new Repository(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName);

                    var allSummaries = repository.GetAll<ContentSummary>(Q
                        .Select(nameof(ContentInfo.Id), nameof(ContentInfo.ChannelId))
                        .Where(nameof(ContentInfo.SiteId), site.Id)
                        .Where(nameof(ContentInfo.ChannelId), summary.Id)
                        .WhereNot(nameof(ContentInfo.SourceId), SourceManager.Preview)
                        .OrderByDesc(nameof(ContentInfo.Taxis), nameof(ContentInfo.Id))
                    );

                    if (allSummaries != null)
                    {
                        var contentSummaries = allSummaries.ToList();
                        cacheManager.Put(listKey, contentSummaries);

                        var count = contentSummaries.Count;
                        cacheManager.Put(countKey, count);
                    }
                    else
                    {
                        cacheManager.Put(listKey, new List<ContentSummary>());
                        cacheManager.Put(countKey, 0);
                    }
                }
            }
        }

        public void CacheAllEntity(SiteInfo site, List<ChannelSummary> channelSummaries)
        {
            var cacheManager = Caching.CacheManager;
            foreach (var channelSummary in channelSummaries)
            {
                var contentSummaries = GetSummaries(site, channelSummary);
                var summary = contentSummaries.FirstOrDefault();
                if (summary != null)
                {
                    var tableName = ChannelManager.GetTableName(site, channelSummary);
                    if (!cacheManager.Exists(GetEntityKey(tableName, summary.Id)))
                    {
                        var contentInfoList = new List<ContentInfo>();

                        var sqlWhere = $"WHERE {ContentAttribute.SiteId} = {site.Id} AND {ContentAttribute.ChannelId} = {channelSummary.Id} AND {ContentAttribute.SourceId} != {SourceManager.Preview}";
                        var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, site.Additional.PageSize, SqlUtils.Asterisk, sqlWhere, "ORDER BY Taxis DESC, Id DESC");

                        using (var rdr = ExecuteReader(sqlSelect))
                        {
                            while (rdr.Read())
                            {
                                var contentInfo = new ContentInfo(rdr);
                                contentInfoList.Add(contentInfo);
                            }
                            rdr.Close();
                        }

                        foreach (var contentInfo in contentInfoList)
                        {
                            cacheManager.Put(GetEntityKey(tableName, contentInfo.Id), contentInfo);
                        }
                    }
                }
            }
        }

        public int GetCount(SiteInfo site, IChannelSummary channel)
        {
            var tableName = ChannelManager.GetTableName(site, channel);
            var countKey = GetCountKey(tableName, site.Id, channel.Id);

            var cacheManager = Caching.CacheManager;
            var count = cacheManager.GetOrCreate(countKey, () =>
            {
                var sqlString = $"SELECT COUNT(*) FROM {tableName} WHERE ChannelId = {channel.Id}";

                return DataProvider.DatabaseDao.GetIntResult(sqlString);
            });

            return count;
        }

        //public ContentInfo GetCacheContentInfo(string tableName, int channelId, int contentId)
        //{
        //    if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

        //    ContentInfo contentInfo = null;

        //    var sqlWhere = $"WHERE ({ContentAttribute.ChannelId} = {channelId} OR {ContentAttribute.ChannelId} = {-channelId}) AND {ContentAttribute.Id} = {contentId}";
        //    var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

        //    using (var rdr = ExecuteReader(sqlSelect))
        //    {
        //        if (rdr.Read())
        //        {
        //            contentInfo = new ContentInfo(rdr);
        //        }
        //        rdr.Close();
        //    }

        //    return contentInfo;
        //}

        public ContentInfo Get(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            return Get(tableName, channelInfo.Id, contentId);
        }

        public ContentInfo Get(SiteInfo site, int channelId, int contentId)
        {
            var tableName = ChannelManager.GetTableName(site, channelId);
            return Get(tableName, channelId, contentId);
        }

        public ContentInfo Get(string tableName, int channelId, int contentId)
        {
            var entityKey = GetEntityKey(tableName, contentId);

            var cacheManager = Caching.CacheManager;
            var entity = cacheManager.GetOrCreate(entityKey, () =>
            {
                ContentInfo contentInfo = null;

                var sqlWhere = $"WHERE ({ContentAttribute.ChannelId} = {channelId} OR {ContentAttribute.ChannelId} = {-channelId}) AND {ContentAttribute.Id} = {contentId}";
                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

                using (var rdr = ExecuteReader(sqlSelect))
                {
                    if (rdr.Read())
                    {
                        contentInfo = new ContentInfo(rdr);
                    }
                    rdr.Close();
                }

                return contentInfo;
            });

            return entity;
        }

        public IList<int> GetContentIds(SiteInfo site, ChannelInfo channel)
        {
            var summaries = GetSummaries(site, channel, false);
            return summaries.Select(x => x.Id).ToList();
        }

        public IList<ContentSummary> GetSummaries(SiteInfo site, ChannelInfo channel, bool isAllContents)
        {
            if (isAllContents)
            {
                var list = new List<ContentSummary>();
                var channelIds = ChannelManager.GetChannelIdList(channel, EScopeType.All);
                foreach (var channelId in channelIds)
                {
                    var child = ChannelManager.GetChannelInfo(site.Id, channelId);
                    list.AddRange(GetSummaries(site, child));
                }

                return list;
            }

            return GetSummaries(site, channel);
        }

        public IList<ContentSummary> GetSummariesByKeyword(SiteInfo site, ChannelInfo channel, bool isAllContents, string type, string keyword)
        {
            var tableName = ChannelManager.GetTableName(site, channel);

            var query = Q
                    .Select(nameof(ContentInfo.Id), nameof(ContentInfo.ChannelId))
                    .Where(nameof(ContentInfo.SiteId), site.Id)
                    .WhereNot(nameof(ContentInfo.SourceId), SourceManager.Preview)
                    .WhereLike(type, $"%{keyword}%")
                    .OrderByDesc(nameof(ContentInfo.Taxis), nameof(ContentInfo.Id));

            if (isAllContents)
            {
                var channelIds = ChannelManager.GetChannelIdList(channel, EScopeType.All);
                query.WhereIn(nameof(ContentInfo.ChannelId), channelIds);
            }
            else
            {
                query.Where(nameof(ContentInfo.ChannelId), channel.Id);
            }

            var repository = new Repository(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName);

            return repository.GetAll<ContentSummary>(query);
        }

        private IList<ContentSummary> GetSummaries(SiteInfo site, IChannelSummary channelSummary)
        {
            //var repository = await GetRepositoryAsync(site, channel);
            //var query = Q.Select(nameof(ContentInfo.ChannelId), nameof(ContentInfo.Id));

            //await QueryWhereAsync(query, site, channel, false);

            //query.OrderByDesc(nameof(ContentInfo.Taxis), nameof(ContentInfo.Id));

            //query.CachingGet(GetListKey(repository.TableName, site.Id, channel.Id));

            //return await repository.GetAllAsync<ContentSummary>(query);

            var tableName = ChannelManager.GetTableName(site, channelSummary);
            var listKey = GetListKey(tableName, site.Id, channelSummary.Id);

            var repository = new Repository(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, site.TableName);

            var cacheManager = Caching.CacheManager;
            var list = cacheManager.GetOrCreate(listKey, () => repository.GetAll<ContentSummary>(Q
                .Select(nameof(ContentInfo.Id), nameof(ContentInfo.ChannelId))
                .Where(nameof(ContentInfo.SiteId), site.Id)
                .Where(nameof(ContentInfo.ChannelId), channelSummary.Id)
                .WhereNot(nameof(ContentInfo.SourceId), SourceManager.Preview)
                .OrderByDesc(nameof(ContentInfo.Taxis), nameof(ContentInfo.Id))
            ));

            return list;
        }
    }
}
