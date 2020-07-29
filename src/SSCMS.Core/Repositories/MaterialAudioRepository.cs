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
    public class MaterialAudioRepository : IMaterialAudioRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<MaterialAudio> _repository;

        public MaterialAudioRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<MaterialAudio>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task<int> InsertAsync(MaterialAudio audio)
        {
            return await _repository.InsertAsync(audio, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> UpdateAsync(MaterialAudio audio)
        {
            return await _repository.UpdateAsync(audio, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var audio = await GetAsync(id);
            if (audio != null && !string.IsNullOrEmpty(audio.Url))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, audio.Url);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var audios = await GetAllAsync();

            if (groupId != 0)
            {
                audios = audios.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                audios = audios.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return audios.Count;
        }

        private async Task<List<MaterialAudio>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialAudio.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<List<MaterialAudio>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var audios = await GetAllAsync();

            if (groupId != 0)
            {
                audios = audios.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                audios = audios.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return audios.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialAudio> GetAsync(int id)
        {
            var audios = await GetAllAsync();
            return audios.FirstOrDefault(x => x.Id == id);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            var audio = await GetAsync(id);
            return audio?.Url;
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            var audios = await GetAllAsync();
            var audio = audios.FirstOrDefault(x => x.Title == title);
            return audio?.Url;
        }

        public async Task<bool> IsExistsAsync(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId)) return false;

            var audios = await GetAllAsync();
            return audios.Exists(x => x.MediaId == mediaId);
        }
    }
}
