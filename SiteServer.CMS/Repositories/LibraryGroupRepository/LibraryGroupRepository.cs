using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public class LibraryGroupRepository : ILibraryGroupRepository
    {
        private readonly Repository<LibraryGroup> _repository;

        public LibraryGroupRepository()
        {
            _repository = new Repository<LibraryGroup>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Type = nameof(Type);
        }

        private string CacheKey(LibraryType type) => Caching.GetListKey(TableName, type.GetValue());

        public async Task<int> InsertAsync(LibraryGroup group)
        {
            return await _repository.InsertAsync(group, Q
                .CachingRemove(CacheKey(group.Type))
            );
        }

        public async Task<bool> UpdateAsync(LibraryGroup group)
        {
            return await _repository.UpdateAsync(group, Q
                .CachingRemove(CacheKey(group.Type))
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
            var list = await _repository.GetAllAsync(Q
                .Where(Attr.Type, type.GetValue())
                .OrderByDesc(nameof(LibraryGroup.Id))
                .CachingGet(CacheKey(type))
            );
            return list.ToList();
        }

        public async Task<LibraryGroup> GetAsync(int groupId)
        {
            return await _repository.GetAsync(groupId);
        }

        public async Task<bool> IsExistsAsync(LibraryType type, string groupName)
        {
            return await _repository.ExistsAsync(Q
                .Where(Attr.Type, type.GetValue())
                .Where(nameof(LibraryGroup.GroupName), groupName)
            );
        }
    }
}
