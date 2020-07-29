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
    public class MaterialArticleRepository : IMaterialArticleRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<MaterialArticle> _repository;

        public MaterialArticleRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<MaterialArticle>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task<int> InsertAsync(MaterialArticle article)
        {
            return await _repository.InsertAsync(article, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> UpdateAsync(MaterialArticle article)
        {
            return await _repository.UpdateAsync(article, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var article = await GetAsync(id);
            if (article != null && !string.IsNullOrEmpty(article.ThumbUrl))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, article.ThumbUrl);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword, List<int> articleIds = null)
        {
            var articles = await GetAllAsync();

            if (articleIds != null && articleIds.Count > 0)
            {
                articles = articles.Where(x => !articleIds.Contains(x.Id)).ToList();
            }

            if (groupId != 0)
            {
                articles = articles.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                articles = articles.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return articles.Count;
        }

        private async Task<List<MaterialArticle>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialArticle.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<List<MaterialArticle>> GetAllAsync(int groupId, string keyword, int page, int perPage, List<int> articleIds = null)
        {
            var articles = await GetAllAsync();

            if (articleIds != null && articleIds.Count > 0)
            {
                articles = articles.Where(x => !articleIds.Contains(x.Id)).ToList();
            }

            if (groupId != 0)
            {
                articles = articles.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                articles = articles.Where(x => StringUtils.ContainsIgnoreCase(x.Title, keyword)).ToList();
            }

            return articles.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialArticle> GetAsync(int id)
        {
            var articles = await GetAllAsync();
            return articles.FirstOrDefault(x => x.Id == id);
        }

        public async Task<string> GetBodyByIdAsync(int id)
        {
            var article = await GetAsync(id);
            return article?.Content;
        }

        public async Task<string> GetBodyByTitleAsync(string title)
        {
            var articles = await GetAllAsync();
            var article = articles.FirstOrDefault(x => x.Title == title);
            return article?.Content;
        }
    }
}
