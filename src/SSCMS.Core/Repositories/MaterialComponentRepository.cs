using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class MaterialComponentRepository : IMaterialComponentRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<MaterialComponent> _repository;

        public MaterialComponentRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<MaterialComponent>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task<int> InsertAsync(MaterialComponent component)
        {
            return await _repository.InsertAsync(component, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> UpdateAsync(MaterialComponent component)
        {
            return await _repository.UpdateAsync(component, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var component = await GetAsync(id);
            if (component != null && !string.IsNullOrEmpty(component.ImageUrl))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, component.ImageUrl);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> IsExistsAsync(string title)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(MaterialComponent.Title), title)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var components = await GetAllAsync();

            if (groupId != 0)
            {
                components = components.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                components = components.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return components.Count;
        }

        private async Task<List<MaterialComponent>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialComponent.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<List<MaterialComponent>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var components = await GetAllAsync();

            if (groupId != 0)
            {
                components = components.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                components = components.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return components.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialComponent> GetAsync(int id)
        {
            var components = await GetAllAsync();
            return components.FirstOrDefault(x => x.Id == id);
        }

        public async Task<string> GetImageUrlByIdAsync(int id)
        {
            var component = await GetAsync(id);
            return component?.ImageUrl;
        }

        public async Task<string> GetImageUrlByTitleAsync(string title)
        {
            var components = await GetAllAsync();
            var component = components.FirstOrDefault(x => x.Title == title);
            return component?.ImageUrl;
        }
    }
}
