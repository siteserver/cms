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
    public class MaterialFileRepository : IMaterialFileRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<MaterialFile> _repository;

        public MaterialFileRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<MaterialFile>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task<int> InsertAsync(MaterialFile file)
        {
            return await _repository.InsertAsync(file, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> UpdateAsync(MaterialFile file)
        {
            return await _repository.UpdateAsync(file, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var file = await GetAsync(id);
            if (file != null && !string.IsNullOrEmpty(file.Url))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, file.Url);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var files = await GetAllAsync();

            if (groupId != 0)
            {
                files = files.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                files = files.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return files.Count;
        }

        private async Task<List<MaterialFile>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialFile.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<List<MaterialFile>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var files = await GetAllAsync();

            if (groupId != 0)
            {
                files = files.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                files = files.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return files.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialFile> GetAsync(int id)
        {
            var files = await GetAllAsync();
            return files.FirstOrDefault(x => x.Id == id);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            var file = await GetAsync(id);
            return file?.Url;
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            var files = await GetAllAsync();
            var file = files.FirstOrDefault(x => x.Title == title);
            return file?.Url;
        }

        public async Task<bool> IsExistsAsync(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId)) return false;

            var files = await GetAllAsync();
            return files.Exists(x => x.MediaId == mediaId);
        }
    }
}
