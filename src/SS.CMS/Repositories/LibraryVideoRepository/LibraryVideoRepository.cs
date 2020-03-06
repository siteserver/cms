using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Repositories
{
    public class LibraryVideoRepository : ILibraryVideoRepository
    {
        private readonly Repository<LibraryVideo> _repository;

        public LibraryVideoRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<LibraryVideo>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(LibraryVideo library)
        {
            return await _repository.InsertAsync(library);
        }

        public async Task<bool> UpdateAsync(LibraryVideo library)
        {
            return await _repository.UpdateAsync(library);
        }

        public async Task<bool> DeleteAsync(int libraryId)
        {
            return await _repository.DeleteAsync(libraryId);
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var query = Q.NewQuery();

            if (groupId > 0)
            {
                query.Where(nameof(LibraryVideo.GroupId), groupId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryVideo.Title), $"%{keyword}%");
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<LibraryVideo>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var query = Q
                .OrderByDesc(nameof(LibraryVideo.Id))
                .ForPage(page, perPage);

            if (groupId > 0)
            {
                query.Where(nameof(LibraryVideo.GroupId), groupId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryVideo.Title), $"%{keyword}%");
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<LibraryVideo> GetAsync(int libraryId)
        {
            return await _repository.GetAsync(libraryId);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryVideo.Url))
                .Where(nameof(LibraryVideo.Id), id)
            );
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryVideo.Url))
                .Where(nameof(LibraryVideo.Title), title)
            );
        }
    }
}
