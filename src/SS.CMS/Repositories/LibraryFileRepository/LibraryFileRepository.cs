using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Repositories
{
    public class LibraryFileRepository : ILibraryFileRepository
    {
        private readonly Repository<LibraryFile> _repository;

        public LibraryFileRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<LibraryFile>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(LibraryFile library)
        {
            return await _repository.InsertAsync(library);
        }

        public async Task<bool> UpdateAsync(LibraryFile library)
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
                query.Where(nameof(LibraryFile.GroupId), groupId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query.Where(q => q
                    .WhereLike(nameof(LibraryFile.Title), $"%{keyword}%")
                    .OrWhere(nameof(LibraryFile.Type), keyword.ToUpper())
                );
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<LibraryFile>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var query = Q
                .OrderByDesc(nameof(LibraryFile.Id))
                .ForPage(page, perPage);

            if (groupId > 0)
            {
                query.Where(nameof(LibraryFile.GroupId), groupId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.Where(q => q
                    .WhereLike(nameof(LibraryFile.Title), $"%{keyword}%")
                    .OrWhere(nameof(LibraryFile.Type), keyword.ToUpper())
                );
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<LibraryFile> GetAsync(int libraryId)
        {
            return await _repository.GetAsync(libraryId);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryFile.Url))
                .Where(nameof(LibraryFile.Id), id)
            );
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryFile.Url))
                .Where(nameof(LibraryFile.Title), title)
            );
        }
    }
}
