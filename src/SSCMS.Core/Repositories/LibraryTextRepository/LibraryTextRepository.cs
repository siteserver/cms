using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;

namespace SSCMS.Core.Repositories.LibraryTextRepository
{
    public class LibraryTextRepository : ILibraryTextRepository
    {
        private readonly Repository<LibraryText> _repository;

        public LibraryTextRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<LibraryText>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(LibraryText library)
        {
            return await _repository.InsertAsync(library);
        }

        public async Task<bool> UpdateAsync(LibraryText library)
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
                query.Where(nameof(LibraryText.GroupId), groupId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryText.Title), $"%{keyword}%");
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<LibraryText>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var query = Q
                .OrderByDesc(nameof(LibraryText.Id))
                .ForPage(page, perPage);

            if (groupId > 0)
            {
                query.Where(nameof(LibraryText.GroupId), groupId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryText.Title), $"%{keyword}%");
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<LibraryText> GetAsync(int libraryId)
        {
            return await _repository.GetAsync(libraryId);
        }

        public async Task<string> GetContentByIdAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryText.Content))
                .Where(nameof(LibraryText.Id), id)
            );
        }

        public async Task<string> GetContentByTitleAsync(string title)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryText.Content))
                .Where(nameof(LibraryText.Title), title)
            );
        }
    }
}
