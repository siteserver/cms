using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Repositories
{
    public partial class SiteRepository : IRepository
    {
        private readonly Repository<Site> _repository;

        public SiteRepository()
        {
            _repository = new Repository<Site>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), CacheManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Site site)
        {
            site.Taxis = await GetMaxTaxisAsync() + 1;
            site.Id = await _repository.InsertAsync(site);
            await RemoveCacheListAsync();
            return site.Id;
        }

        public async Task DeleteAsync(int siteId)
        {
            var siteEntity = await GetAsync(siteId);
            var list = await ChannelManager.GetChannelIdListAsync(siteId);
            await DataProvider.TableStyleRepository.DeleteAsync(list, siteEntity.TableName);

            await DataProvider.ContentTagRepository.DeleteTagsAsync(siteId);

            await DataProvider.ChannelRepository.DeleteAllAsync(siteId);

            await UpdateParentIdToZeroAsync(siteId);

            await _repository.DeleteAsync(siteId);

            await RemoveCacheListAsync();
            await RemoveCacheEntityAsync(siteId);
            ChannelManager.RemoveCacheBySiteId(siteId);
            PermissionsImpl.ClearAllCache();
        }

        public async Task UpdateAsync(Site site)
        {
            if (site.Root)
            {
                await UpdateAllIsRootAsync();
            }

            await _repository.UpdateAsync(site);
            await RemoveCacheListAsync();
            await RemoveCacheEntityAsync(site.Id);
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.TableName), tableName)
                .Where(nameof(Site.Id), siteId)
            );
            await RemoveCacheListAsync();
            await RemoveCacheEntityAsync(siteId);
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.ParentId), 0)
                .Where(nameof(Site.ParentId), parentId)
            );
            var siteIds = await GetSiteIdListAsync(parentId);
            foreach (var siteId in siteIds)
            {
                await RemoveCacheEntityAsync(siteId);
            }
            await RemoveCacheListAsync();
        }

        private async Task UpdateAllIsRootAsync()
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.IsRoot), false.ToString())
            );
            var siteIds = await GetSiteIdListAsync();
            foreach (var siteId in siteIds)
            {
                await RemoveCacheEntityAsync(siteId);
            }
            await RemoveCacheListAsync();
        }

        //private static string GetSiteDir(List<Site> siteEntityList, Site siteEntity)
        //{
        //    if (TranslateUtils.ToBool(siteEntity.IsRoot)) return string.Empty;
        //    if (siteEntity.ParentId <= 0) return PathUtils.GetDirectoryName(siteEntity.SiteDir, false);

        //    Site parent = null;
        //    foreach (var current in siteEntityList)
        //    {
        //        if (current.Id != siteEntity.ParentId) continue;
        //        parent = current;
        //        break;
        //    }

        //    return PathUtils.Combine(GetSiteDir(siteEntityList, parent), PathUtils.GetDirectoryName(siteEntity.SiteDir, false));
        //}

        

        

        

        public async Task<IDataReader> GetStlDataSourceAsync(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            IDataReader ie = null;

            var sqlWhereString = string.Empty;

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await GetSiteByDirectoryAsync(siteDir);
            }

            if (site != null)
            {
                sqlWhereString = $"WHERE (ParentId = {site.Id})";
            }
            else
            {
                if (scopeType == EScopeType.Children)
                {
                    sqlWhereString = "WHERE (ParentId = 0 AND IsRoot = 'False')";
                }
                else if (scopeType == EScopeType.Descendant)
                {
                    sqlWhereString = "WHERE (IsRoot = 'False')";
                }
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                sqlWhereString = string.IsNullOrEmpty(sqlWhereString) ? $"WHERE ({whereString})" : $"{sqlWhereString} AND ({whereString})";
            }

            if (string.IsNullOrEmpty(orderByString) || StringUtils.EqualsIgnoreCase(orderByString, "default"))
            {
                orderByString = "ORDER BY IsRoot DESC, ParentId, Taxis DESC, Id";

                //var sqlSelect = DataProvider.DatabaseRepository.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                var sqlSelect = DataProvider.DatabaseRepository.GetPageSqlString(TableName, SqlUtils.Asterisk, sqlWhereString, orderByString, startNum - 1, totalNum);

                using (var connection = _repository.Database.GetConnection())
                {
                    ie = connection.ExecuteReader(sqlSelect);
                }
            }

            return ie;
        }

        
    }
}
