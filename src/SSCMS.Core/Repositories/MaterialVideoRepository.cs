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
    public class MaterialVideoRepository : IMaterialVideoRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<MaterialVideo> _repository;

        public MaterialVideoRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<MaterialVideo>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task<int> InsertAsync(MaterialVideo video)
        {
            return await _repository.InsertAsync(video, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> UpdateAsync(MaterialVideo video)
        {
            return await _repository.UpdateAsync(video, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var video = await GetAsync(id);
            if (video != null && !string.IsNullOrEmpty(video.Url))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, video.Url);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var videos = await GetAllAsync();

            if (groupId != 0)
            {
                videos = videos.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                videos = videos.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return videos.Count;
        }

        private async Task<List<MaterialVideo>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialVideo.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<List<MaterialVideo>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var videos = await GetAllAsync();

            if (groupId != 0)
            {
                videos = videos.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                videos = videos.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return videos.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialVideo> GetAsync(int id)
        {
            var videos = await GetAllAsync();
            return videos.FirstOrDefault(x => x.Id == id);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            var video = await GetAsync(id);
            return video?.Url;
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            var videos = await GetAllAsync();
            var video = videos.FirstOrDefault(x => x.Title == title);
            return video?.Url;
        }

        public async Task<bool> IsExistsAsync(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId)) return false;

            var videos = await GetAllAsync();
            return videos.Exists(x => x.MediaId == mediaId);
        }
    }
}
