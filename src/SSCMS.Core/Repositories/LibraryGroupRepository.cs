using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class LibraryGroupRepository : ILibraryGroupRepository
    {
        private readonly Repository<LibraryGroup> _repository;
        private readonly ISiteRepository _siteRepository;

        public LibraryGroupRepository(ISettingsManager settingsManager, ISiteRepository siteRepository)
        {
            _repository = new Repository<LibraryGroup>(settingsManager.Database, settingsManager.Redis);
            _siteRepository = siteRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey(LibraryType type) => CacheUtils.GetListKey(TableName, type.GetValue());

        public async Task<int> InsertAsync(LibraryGroup group)
        {
            return await _repository.InsertAsync(group, Q
                .CachingRemove(CacheKey(group.LibraryType))
            );
        }

        public async Task<bool> UpdateAsync(LibraryGroup group)
        {
            return await _repository.UpdateAsync(group, Q
                .CachingRemove(CacheKey(group.LibraryType))
            );
        }

        public async Task<bool> DeleteAsync(LibraryType type, int groupId)
        {
            return await _repository.DeleteAsync(groupId, Q
                .CachingRemove(CacheKey(type))
            );
        }

        public async Task<List<LibraryGroup>> GetAllAsync(LibraryType type)
        {
            var groups = new List<LibraryGroup>
            {
                new LibraryGroup
                {
                    Id = 0,
                    GroupName = "全部"
                }
            };

            var sites = await _siteRepository.GetSitesAsync();
            groups.AddRange(sites.Select(site => new LibraryGroup
            {
                Id = -site.Id, 
                GroupName = site.SiteName
            }));

            var list = await _repository.GetAllAsync(Q
                .Where(nameof(LibraryGroup.LibraryType), type.GetValue())
                .OrderBy(nameof(LibraryGroup.Id))
                .CachingGet(CacheKey(type))
            );

            groups.AddRange(list);
            return groups;
        }

        public async Task<LibraryGroup> GetAsync(int groupId)
        {
            return await _repository.GetAsync(groupId);
        }

        public async Task<bool> IsExistsAsync(LibraryType type, string groupName)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(LibraryGroup.LibraryType), type.GetValue())
                .Where(nameof(LibraryGroup.GroupName), groupName)
            );
        }
    }
}
