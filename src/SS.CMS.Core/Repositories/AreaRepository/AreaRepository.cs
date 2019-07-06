using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class AreaRepository : IAreaRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly Repository<AreaInfo> _repository;

        public AreaRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(AreaRepository));
            _repository = new Repository<AreaInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(AreaInfo.Id);
            public const string ParentId = nameof(AreaInfo.ParentId);
            public const string ParentsPath = nameof(AreaInfo.ParentsPath);
            public const string ChildrenCount = nameof(AreaInfo.ChildrenCount);
            public const string Taxis = nameof(AreaInfo.Taxis);
            public const string IsLastNode = nameof(AreaInfo.IsLastNode);
        }

        private async Task<int> InsertAsync(AreaInfo parentInfo, AreaInfo areaInfo)
        {
            if (parentInfo != null)
            {
                areaInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.Id;
                areaInfo.ParentsCount = parentInfo.ParentsCount + 1;

                var maxTaxis = await GetMaxTaxisByParentPathAsync(areaInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentInfo.Taxis;
                }
                areaInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                areaInfo.ParentsPath = "0";
                areaInfo.ParentsCount = 0;
                var maxTaxis = await GetMaxTaxisByParentPathAsync("0");
                areaInfo.Taxis = maxTaxis + 1;
            }
            areaInfo.ChildrenCount = 0;
            areaInfo.IsLastNode = true;

            await _repository.IncrementAsync(Attr.Taxis, Q
                    .Where(Attr.Taxis, ">=", areaInfo.Taxis)
                    );

            await _repository.InsertAsync(areaInfo);

            if (!string.IsNullOrEmpty(areaInfo.ParentsPath) && areaInfo.ParentsPath != "0")
            {
                await _repository.IncrementAsync(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(areaInfo.ParentsPath)));
            }

            await _repository.UpdateAsync(Q
                .Set(Attr.IsLastNode, false)
                .Where(Attr.ParentId, areaInfo.ParentId)
            );

            var topId = await _repository.GetAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, areaInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                await _repository.UpdateAsync(Q
                    .Set(Attr.IsLastNode, true)
                    .Where(nameof(Attr.Id), topId)
                );
            }

            await _cache.RemoveAsync(_cacheKey);

            return areaInfo.Id;
        }

        private async Task UpdateSubtractChildrenCountAsync(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            await _repository.DecrementAsync(Attr.ChildrenCount, Q
                .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)), subtractNum);

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task TaxisSubtractAsync(int selectedId)
        {
            var areaInfo = await _repository.GetAsync(selectedId);
            if (areaInfo == null) return;

            var dataInfo = await _repository.GetAsync(Q
                .Where(Attr.ParentId, areaInfo.ParentId)
                .WhereNot(Attr.Id, areaInfo.Id)
                .Where(Attr.Taxis, "<", areaInfo.Taxis)
                .OrderByDesc(Attr.Taxis));

            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerChildrenCount = dataInfo.ChildrenCount;
            var lowerParentsPath = dataInfo.ParentsPath;

            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

            await SetTaxisSubtractAsync(selectedId, selectedNodePath, lowerChildrenCount + 1);
            await SetTaxisAddAsync(lowerId, lowerNodePath, areaInfo.ChildrenCount + 1);

            await UpdateIsLastNodeAsync(areaInfo.ParentId);
        }

        private async Task TaxisAddAsync(int selectedId)
        {
            var areaInfo = await _repository.GetAsync(selectedId);
            if (areaInfo == null) return;

            var dataInfo = await _repository.GetAsync(Q
                .Where(Attr.ParentId, areaInfo.ParentId)
                .WhereNot(Attr.Id, areaInfo.Id)
                .Where(Attr.Taxis, ">", areaInfo.Taxis)
                .OrderBy(Attr.Taxis));

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherChildrenCount = dataInfo.ChildrenCount;
            var higherParentsPath = dataInfo.ParentsPath;

            var higherNodePath = string.Concat(higherParentsPath, ",", higherId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

            await SetTaxisAddAsync(selectedId, selectedNodePath, higherChildrenCount + 1);
            await SetTaxisSubtractAsync(higherId, higherNodePath, areaInfo.ChildrenCount + 1);

            await UpdateIsLastNodeAsync(areaInfo.ParentId);
        }

        private async Task SetTaxisAddAsync(int areaId, string parentsPath, int addNum)
        {
            await _repository.IncrementAsync(Attr.Taxis, Q
                .Where(Attr.Id, areaId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, parentsPath + ","), addNum);

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task SetTaxisSubtractAsync(int areaId, string parentsPath, int subtractNum)
        {
            await _repository.DecrementAsync(Attr.Taxis, Q
                .Where(Attr.Id, areaId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, parentsPath + ","), subtractNum);

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task UpdateIsLastNodeAsync(int parentId)
        {
            if (parentId <= 0) return;

            await _repository.UpdateAsync(Q
                .Set(Attr.IsLastNode, false)
                .Where(Attr.ParentId, parentId)
            );

            var topId = await _repository.GetAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                await _repository.UpdateAsync(Q
                    .Set(Attr.IsLastNode, true)
                    .Where(nameof(Attr.Id), topId)
                );
            }
        }

        private async Task<int> GetMaxTaxisByParentPathAsync(string parentPath)
        {
            return await _repository.MaxAsync(Attr.Taxis, Q
                .Where(Attr.ParentsPath, parentPath)
                .OrWhereStarts(Attr.ParentsPath, parentPath + ",")) ?? 0;
        }

        public async Task<int> InsertAsync(AreaInfo areaInfo)
        {
            var parentAreaInfo = await _repository.GetAsync(areaInfo.ParentId);

            areaInfo.Id = await InsertAsync(parentAreaInfo, areaInfo);
            await _cache.RemoveAsync(_cacheKey);

            return areaInfo.Id;
        }

        public async Task<bool> UpdateAsync(AreaInfo areaInfo)
        {
            var updated = await _repository.UpdateAsync(areaInfo);
            if (updated)
            {
                await _cache.RemoveAsync(_cacheKey);
            }

            return updated;
        }

        public async Task UpdateTaxisAsync(int selectedId, bool isSubtract)
        {
            if (isSubtract)
            {
                await TaxisSubtractAsync(selectedId);
            }
            else
            {
                await TaxisAddAsync(selectedId);
            }
        }

        public async Task<bool> DeleteAsync(int areaId)
        {
            var areaInfo = await _repository.GetAsync(areaId);
            if (areaInfo != null)
            {
                var areaIdList = new List<int>();
                if (areaInfo.ChildrenCount > 0)
                {
                    areaIdList.AddRange(await GetIdListForDescendantAsync(areaId));
                }
                areaIdList.Add(areaId);

                var deletedNum = await _repository.DeleteAsync(Q
                    .WhereIn(Attr.Id, areaIdList));

                if (deletedNum > 0)
                {
                    await _repository.DecrementAsync(Attr.Taxis, Q
                        .Where(Attr.Taxis, ">", areaInfo.Taxis), deletedNum);
                }

                await UpdateIsLastNodeAsync(areaInfo.ParentId);
                await UpdateSubtractChildrenCountAsync(areaInfo.ParentsPath, deletedNum);
            }

            await _cache.RemoveAsync(_cacheKey);

            return true;
        }

        private async Task<IEnumerable<AreaInfo>> GetAreaInfoListAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderBy(Attr.Taxis));
        }

        public async Task<IEnumerable<int>> GetIdListByParentIdAsync(int parentId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
        }

        private async Task<IEnumerable<int>> GetIdListForDescendantAsync(int areaId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .WhereStarts(Attr.ParentsPath, $"{areaId},")
                .OrWhereContains(Attr.ParentsPath, $",{areaId},")
                .OrWhereEnds(Attr.ParentsPath, $",{areaId}"));
        }

        private async Task<List<KeyValuePair<int, AreaInfo>>> GetAreaInfoPairListToCacheAsync()
        {
            var areaInfoList = await GetAreaInfoListAsync();

            return areaInfoList.Select(areaInfo => new KeyValuePair<int, AreaInfo>(areaInfo.Id, areaInfo)).ToList();
        }
    }
}
