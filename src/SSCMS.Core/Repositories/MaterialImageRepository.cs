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
    public class MaterialImageRepository : IMaterialImageRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<MaterialImage> _repository;

        public MaterialImageRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<MaterialImage>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task<int> InsertAsync(MaterialImage image)
        {
            return await _repository.InsertAsync(image, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> UpdateAsync(MaterialImage image)
        {
            return await _repository.UpdateAsync(image, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await GetAsync(id);
            if (image != null && !string.IsNullOrEmpty(image.Url))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, image.Url);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var images = await GetAllAsync();

            if (groupId != 0)
            {
                images = images.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                images = images.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return images.Count;
        }

        private async Task<List<MaterialImage>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialImage.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<List<MaterialImage>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var images = await GetAllAsync();

            if (groupId != 0)
            {
                images = images.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                images = images.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return images.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialImage> GetAsync(int id)
        {
            var images = await GetAllAsync();
            return images.FirstOrDefault(x => x.Id == id);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            var image = await GetAsync(id);
            return image?.Url;
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            var images = await GetAllAsync();
            var image = images.FirstOrDefault(x => x.Title == title);
            return image?.Url;
        }

        public async Task<bool> IsExistsAsync(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId)) return false;

            var images = await GetAllAsync();
            return images.Exists(x => x.MediaId == mediaId);
        }
    }
}
