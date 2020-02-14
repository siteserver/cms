using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class SiteRepository : IRepository
    {
        private readonly Repository<Site> _repository;

        public SiteRepository()
        {
            _repository = new Repository<Site>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Site site)
        {
            site.Taxis = await GetMaxTaxisAsync() + 1;
            site.Id = await _repository.InsertAsync(site, Q
                .AllowIdentityInsert()
                .CachingRemove(GetListKey())
            );
            return site.Id;
        }

        public async Task DeleteAsync(int siteId)
        {
            var site = await GetAsync(siteId);
            var list = await DataProvider.ChannelRepository.GetChannelIdListAsync(siteId);
            await DataProvider.TableStyleRepository.DeleteAsync(list, site.TableName);

            await DataProvider.ContentGroupRepository.DeleteAsync(siteId);
            await DataProvider.ContentTagRepository.DeleteAsync(siteId);

            await DataProvider.ChannelRepository.DeleteAllAsync(siteId);

            await UpdateParentIdToZeroAsync(siteId);

            await _repository.DeleteAsync(siteId, Q
                .CachingRemove(GetListKey(), GetEntityKey(siteId))
            );
        }

        public async Task UpdateAsync(Site site)
        {
            var cache = await GetCacheAsync(site.Id);
            if (site.Root != cache.Root)
            {
                await UpdateAllIsRootAsync();
            }

            await _repository.UpdateAsync(site, Q
                .CachingRemove(GetListKey(), GetEntityKey(site.Id))
            );
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.TableName), tableName)
                .Where(nameof(Site.Id), siteId)
                .CachingRemove(GetListKey(), GetEntityKey(siteId))
            );
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            var cacheKeys = new List<string>
            {
                GetListKey()
            };
            var siteIds = await GetSiteIdListAsync(parentId);
            foreach (var siteId in siteIds)
            {
                cacheKeys.Add(GetEntityKey(siteId));
            }
            
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.ParentId), 0)
                .Where(nameof(Site.ParentId), parentId)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        private async Task UpdateAllIsRootAsync()
        {
            var cacheKeys = new List<string>
            {
                GetListKey()
            };
            var siteIds = await GetSiteIdListAsync();
            foreach (var siteId in siteIds)
            {
                cacheKeys.Add(GetEntityKey(siteId));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Site.Root), false)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

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

                using var connection = _repository.Database.GetConnection();
                ie = connection.ExecuteReader(sqlSelect);
            }

            return ie;
        }
    }
}
