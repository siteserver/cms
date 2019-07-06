using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository : ISiteRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly Repository<SiteInfo> _repository;
        private readonly ISettingsManager _settingsManager;

        public SiteRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(SiteRepository));
            _repository = new Repository<SiteInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(SiteInfo.Id);
            public const string SiteDir = nameof(SiteInfo.SiteDir);
            public const string TableName = nameof(SiteInfo.TableName);
            public const string IsRoot = "IsRoot";
            public const string ParentId = nameof(SiteInfo.ParentId);
            public const string Taxis = nameof(SiteInfo.Taxis);
        }

        public async Task<int> InsertAsync(SiteInfo siteInfo)
        {
            siteInfo.Taxis = GetMaxTaxis() + 1;
            siteInfo.Id = await _repository.InsertAsync(siteInfo);

            await _cache.RemoveAsync(_cacheKey);

            return siteInfo.Id;
        }

        public async Task<bool> DeleteAsync(int siteId)
        {
            var siteInfo = await GetSiteInfoAsync(siteId);
            // var list = ChannelManager.GetChannelIdList(siteId);
            // DataProvider.TableStyleRepository.Delete(list, siteInfo.TableName);

            // DataProvider.TagRepository.DeleteTags(siteId);

            // DataProvider.ChannelRepository.DeleteAll(siteId);

            await UpdateParentIdToZeroAsync(siteId);

            await _repository.DeleteAsync(siteId);

            await _cache.RemoveAsync(_cacheKey);
            // ChannelManager.RemoveCacheBySiteId(siteId);
            // Permissions.ClearAllCache();

            return true;
        }

        public async Task<bool> UpdateAsync(SiteInfo siteInfo)
        {
            if (siteInfo.IsRoot)
            {
                await UpdateAllIsRootAsync();
            }

            var updated = await _repository.UpdateAsync(siteInfo);

            if (updated)
            {
                await _cache.RemoveAsync(_cacheKey);
            }

            return updated;
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.TableName, tableName)
                .Where(Attr.Id, siteId)
            );

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.ParentId, 0)
                .Where(Attr.ParentId, parentId)
            );

            await _cache.RemoveAsync(_cacheKey);
        }

        public IList<string> GetLowerSiteDirListThatNotIsRoot()
        {
            var list = _repository.GetAll<string>(Q
                .Select(Attr.SiteDir)
                .WhereNot(Attr.IsRoot, true.ToString()));

            return list.Select(x => x.ToLower()).ToList();
        }

        private async Task UpdateAllIsRootAsync()
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.IsRoot, false.ToString())
            );

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task<List<KeyValuePair<int, SiteInfo>>> GetSiteInfoKeyValuePairListToCacheAsync()
        {
            var list = new List<KeyValuePair<int, SiteInfo>>();

            var siteInfoList = await GetSiteInfoListToCacheAsync();
            foreach (var siteInfo in siteInfoList)
            {
                var entry = new KeyValuePair<int, SiteInfo>(siteInfo.Id, siteInfo);
                list.Add(entry);
            }

            return list;
        }

        private async Task<IEnumerable<SiteInfo>> GetSiteInfoListToCacheAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(Attr.Taxis, Attr.Id));
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public List<string> GetLowerSiteDirList(int parentId)
        {
            return _repository.GetAll<string>(Q
                    .Select(Attr.SiteDir)
                    .Where(Attr.ParentId, parentId))
                .Select(x => x.ToLower())
                .ToList();
        }

        public async Task<List<KeyValuePair<int, SiteInfo>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString)
        {
            var query = Q.NewQuery();

            SiteInfo siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = await GetSiteInfoBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = await GetSiteInfoByDirectoryAsync(siteDir);
            }

            if (siteInfo != null)
            {
                query.Where(Attr.ParentId, siteInfo.Id);
            }
            else
            {
                if (scopeType == ScopeType.Children)
                {
                    query.Where(Attr.ParentId, 0).Where(Attr.IsRoot, false);
                }
                else if (scopeType == ScopeType.Descendant)
                {
                    query.Where(Attr.IsRoot, false);
                }
            }

            query.OrderByDesc(Attr.IsRoot).OrderBy(Attr.ParentId).OrderByDesc(Attr.Taxis).OrderBy(Attr.Id);

            query.Offset(startNum - 1).Limit(totalNum);

            var list = new List<KeyValuePair<int, SiteInfo>>();
            var itemIndex = 0;
            var minSiteInfoList = await _repository.GetAllAsync(query);

            foreach (var minSiteInfo in minSiteInfoList)
            {
                list.Add(new KeyValuePair<int, SiteInfo>(itemIndex++, minSiteInfo));
            }

            return list;
        }

        private int GetMaxTaxis()
        {
            return _repository.Max(Attr.Taxis) ?? 0;
        }

        public int GetTableCount(string tableName)
        {
            return _repository.Count(Q.Where(Attr.TableName, tableName));
        }
    }
}