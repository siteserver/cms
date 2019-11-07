using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Model.Mappings;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class SiteDao : DataProviderBase
    {
        private readonly Repository<SiteInfo> _repository;

        public SiteDao()
        {
            _repository = new Repository<SiteInfo>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public override string TableName => _repository.TableName;
        public override List<TableColumn> TableColumns => _repository.TableColumns;

        private static Site ToDto(SiteInfo siteInfo)
        {
            return MapperManager.MapTo<Site>(siteInfo);
        }

        private static SiteInfo ToDb(Site site)
        {
            return MapperManager.MapTo<SiteInfo>(site);
        }

        public async Task<int> InsertAsync(Site site)
        {
            var siteInfo = ToDb(site);
            siteInfo.Taxis = await GetMaxTaxisAsync() + 1;
            site.Id = await _repository.InsertAsync(siteInfo);
            SiteManager.ClearCache();
            return site.Id;
        }

        public async Task DeleteAsync(int siteId)
        {
            var siteInfo = await SiteManager.GetSiteAsync(siteId);
            var list = ChannelManager.GetChannelIdList(siteId);
            DataProvider.TableStyleDao.Delete(list, siteInfo.TableName);

            DataProvider.TagDao.DeleteTags(siteId);

            DataProvider.ChannelDao.DeleteAll(siteId);

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

            var siteInfo = ToDb(site);

            await _repository.UpdateAsync(siteInfo);
            SiteManager.ClearCache();
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(SiteInfo.TableName), tableName)
                .Where(nameof(SiteInfo.Id), siteId)
            );
            SiteManager.ClearCache();
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(SiteInfo.ParentId), 0)
                .Where(nameof(SiteInfo.ParentId), parentId)
            );
            SiteManager.ClearCache();
        }

        public async Task<IList<string>> GetLowerSiteDirListThatNotIsRootAsync()
        {
            var list = await _repository.GetAllAsync<string>(Q
                .Select(nameof(SiteInfo.SiteDir))
                .Where(nameof(SiteInfo.IsRoot), false.ToString())
            );
            return list.Select(StringUtils.ToLower).ToList();
        }

        private async Task UpdateAllIsRootAsync()
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(SiteInfo.IsRoot), false.ToString())
            );
            SiteManager.ClearCache();
        }

        public async Task<List<KeyValuePair<int, Site>>> GetSiteKeyValuePairListAsync()
        {
            var list = new List<KeyValuePair<int, Site>>();

            var siteList = await GetSiteListAsync();
            foreach (var site in siteList)
            {
                var entry = new KeyValuePair<int, Site>(site.Id, site);
                list.Add(entry);
            }

            return list;
        }

        private async Task<IEnumerable<Site>> GetSiteListAsync()
        {
            var list = await _repository.GetAllAsync(Q
                .OrderBy(nameof(SiteInfo.Taxis), nameof(SiteInfo.Id))
            );
            return list.Select(ToDto);
        }

        public async Task<bool> IsTableUsedAsync(string tableName)
        {
            var exists = await _repository.ExistsAsync(Q.Where(nameof(SiteInfo.TableName), tableName));
            if (exists) return true;

            var contentModelPluginIdList = DataProvider.ChannelDao.GetContentModelPluginIdList();
            foreach (var pluginId in contentModelPluginIdList)
            {
                var service = PluginManager.GetService(pluginId);
                if (service != null && PluginContentTableManager.IsContentTable(service) && service.ContentTableName == tableName)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<int> GetIdByIsRootAsync()
        {
            var siteId = await _repository.GetAsync<int?>(Q.Select(nameof(SiteInfo.Id))
                .Where(nameof(SiteInfo.IsRoot), true.ToString()));

            return siteId ?? 0;
        }

        public async Task<int> GetIdBySiteDirAsync(string siteDir)
        {
            var siteId = await _repository.GetAsync<int?>(Q
                .Select(nameof(SiteInfo.Id))
                .Where(nameof(SiteInfo.SiteDir), siteDir)
            );

            return siteId ?? 0;
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public async Task<IEnumerable<string>> GetLowerSiteDirListAsync(int parentId)
        {
            var list = await _repository.GetAllAsync<string>(Q
                .Select(nameof(SiteInfo.SiteDir))
                .Where(nameof(SiteInfo.ParentId), parentId)
            );
            return list.Select(StringUtils.ToLower);
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

                ie = ExecuteReader(sqlSelect);
            }

            return ie;
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            return await _repository.MaxAsync(nameof(SiteInfo.Taxis)) ?? 0;
        }
    }
}
