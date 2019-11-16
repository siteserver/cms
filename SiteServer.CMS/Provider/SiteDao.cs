using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Provider
{
    public class SiteDao : IRepository
    {
        private readonly Repository<Site> _repository;

        public SiteDao()
        {
            _repository = new Repository<Site>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Site site)
        {
            site.Taxis = await GetMaxTaxisAsync() + 1;
            site.Id = await _repository.InsertAsync(site);
            SiteManager.ClearCache();
            return site.Id;
        }

        public async Task DeleteAsync(int siteId)
        {
            var siteEntity = await SiteManager.GetSiteAsync(siteId);
            var list = await ChannelManager.GetChannelIdListAsync(siteId);
            await DataProvider.TableStyleDao.DeleteAsync(list, siteEntity.TableName);

            await DataProvider.ContentTagDao.DeleteTagsAsync(siteId);

            await DataProvider.ChannelDao.DeleteAllAsync(siteId);

            await UpdateParentIdToZeroAsync(siteId);

            await _repository.DeleteAsync(siteId);

            SiteManager.ClearCache();
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
            SiteManager.ClearCache();
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.TableName), tableName)
                .Where(nameof(Site.Id), siteId)
            );
            SiteManager.ClearCache();
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.ParentId), 0)
                .Where(nameof(Site.ParentId), parentId)
            );
            SiteManager.ClearCache();
        }

        private async Task UpdateAllIsRootAsync()
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.IsRoot), false.ToString())
            );
            SiteManager.ClearCache();
        }

        public async Task<List<KeyValuePair<int, Site>>> GetSiteKeyValuePairListAsync()
        {
            var list = new List<KeyValuePair<int, Site>>();

            var siteEntityList = (await _repository.GetAllAsync(Q
                .OrderBy(nameof(Site.Taxis), nameof(Site.Id))
            )).ToList();

            foreach (var site in siteEntityList)
            {
                site.SiteDir = GetSiteDir(siteEntityList, site);

                var entry = new KeyValuePair<int, Site>(site.Id, site);
                list.Add(entry);
            }

            foreach (var pair in list)
            {
                var site = pair.Value;
                if (site == null) continue;

                site.Children = list.Where(x => x.Value.ParentId == site.Id).Select(x => x.Value).ToList();
            }

            return list;
        }

        private static string GetSiteDir(List<Site> siteEntityList, Site siteEntity)
        {
            if (TranslateUtils.ToBool(siteEntity.IsRoot)) return string.Empty;
            if (siteEntity.ParentId <= 0) return PathUtils.GetDirectoryName(siteEntity.SiteDir, false);

            Site parent = null;
            foreach (var current in siteEntityList)
            {
                if (current.Id != siteEntity.ParentId) continue;
                parent = current;
                break;
            }

            return PathUtils.Combine(GetSiteDir(siteEntityList, parent), PathUtils.GetDirectoryName(siteEntity.SiteDir, false));
        }

        public async Task<bool> IsTableUsedAsync(string tableName)
        {
            var exists = await _repository.ExistsAsync(Q.Where(nameof(Site.TableName), tableName));
            if (exists) return true;

            var contentModelPluginIdList = await DataProvider.ChannelDao.GetContentModelPluginIdListAsync();
            foreach (var pluginId in contentModelPluginIdList)
            {
                var service = await PluginManager.GetServiceAsync(pluginId);
                if (service != null && PluginContentTableManager.IsContentTable(service) && service.ContentTableName == tableName)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<int> GetIdByIsRootAsync()
        {
            var siteId = await _repository.GetAsync<int?>(Q.Select(nameof(Site.Id))
                .Where(nameof(Site.IsRoot), true.ToString()));

            return siteId ?? 0;
        }

        public async Task<int> GetIdBySiteDirAsync(string siteDir)
        {
            var siteId = await _repository.GetAsync<int?>(Q
                .Select(nameof(Site.Id))
                .Where(nameof(Site.SiteDir), siteDir)
            );

            return siteId ?? 0;
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public async Task<IEnumerable<string>> GetLowerSiteDirListAsync(int parentId)
        {
            var list = await _repository.GetAllAsync<string>(Q
                .Select(nameof(Site.SiteDir))
                .Where(nameof(Site.ParentId), parentId)
            );
            return list.Select(StringUtils.ToLower);
        }

        public async Task<IList<string>> GetSiteDirListAsync(int parentId)
        {
            var list = await _repository.GetAllAsync<string>(Q
                .Select(nameof(Site.SiteDir))
                .Where(nameof(Site.ParentId), parentId)
                .Where(nameof(Site.IsRoot), false.ToString())
            );
            return list.ToList();
        }

        public async Task<IDataReader> GetStlDataSourceAsync(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            IDataReader ie = null;

            var sqlWhereString = string.Empty;

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await SiteManager.GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await SiteManager.GetSiteByDirectoryAsync(siteDir);
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

                //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlUtils.Asterisk, sqlWhereString, orderByString, startNum - 1, totalNum);

                using (var connection = _repository.Database.GetConnection())
                {
                    ie = connection.ExecuteReader(sqlSelect);
                }
            }

            return ie;
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            return await _repository.MaxAsync(nameof(Site.Taxis)) ?? 0;
        }
    }
}
