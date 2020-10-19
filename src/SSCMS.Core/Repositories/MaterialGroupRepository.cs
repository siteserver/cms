using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class MaterialGroupRepository : IMaterialGroupRepository
    {
        private readonly Repository<MaterialGroup> _repository;
        private readonly ISiteRepository _siteRepository;

        public MaterialGroupRepository(ISettingsManager settingsManager, ISiteRepository siteRepository)
        {
            _repository = new Repository<MaterialGroup>(settingsManager.Database, settingsManager.Redis);
            _siteRepository = siteRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey(MaterialType type) => CacheUtils.GetListKey(TableName, type.GetValue());

        public async Task<int> InsertAsync(MaterialGroup group)
        {
            return await _repository.InsertAsync(group, Q
                .CachingRemove(CacheKey(group.MaterialType))
            );
        }

        public async Task<bool> UpdateAsync(MaterialGroup group)
        {
            return await _repository.UpdateAsync(group, Q
                .CachingRemove(CacheKey(group.MaterialType))
            );
        }

        public async Task<bool> DeleteAsync(MaterialType type, int groupId)
        {
            return await _repository.DeleteAsync(groupId, Q
                .CachingRemove(CacheKey(type))
            );
        }

        public async Task<List<MaterialGroup>> GetAllAsync(MaterialType type)
        {
            var groups = new List<MaterialGroup>
            {
                new MaterialGroup
                {
                    Id = 0,
                    MaterialType = type,
                    GroupName = "全部"
                }
            };

            var sites = await _siteRepository.GetSitesAsync();
            groups.AddRange(sites.Select(site => new MaterialGroup
            {
                Id = -site.Id,
                MaterialType = type,
                GroupName = site.SiteName
            }));

            var list = await _repository.GetAllAsync(Q
                .Where(nameof(MaterialGroup.MaterialType), type.GetValue())
                .OrderBy(nameof(MaterialGroup.Id))
                .CachingGet(CacheKey(type))
            );

            groups.AddRange(list);
            return groups;
        }

        public async Task<MaterialGroup> GetAsync(int groupId)
        {
            return await _repository.GetAsync(groupId);
        }

        public async Task<bool> IsExistsAsync(MaterialType type, string groupName)
        {
            var groups = await GetAllAsync(type);
            return groups.Exists(x => StringUtils.EqualsIgnoreCase(groupName, x.GroupName));
        }
    }
}
