using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;

namespace SSCMS.Core.Repositories.LibraryImageRepository
{
    public class LibraryImageRepository : ILibraryImageRepository
    {
        private readonly Repository<LibraryImage> _repository;

        public LibraryImageRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<LibraryImage>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(LibraryImage library)
        {
            return await _repository.InsertAsync(library);
        }

        public async Task<bool> UpdateAsync(LibraryImage library)
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
                query.Where(nameof(LibraryImage.GroupId), groupId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryImage.Title), $"%{keyword}%");
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<LibraryImage>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var query = Q
                .OrderByDesc(nameof(LibraryImage.Id))
                .ForPage(page, perPage);

            if (groupId > 0)
            {
                query.Where(nameof(LibraryImage.GroupId), groupId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryImage.Title), $"%{keyword}%");
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<LibraryImage> GetAsync(int libraryId)
        {
            return await _repository.GetAsync(libraryId);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryImage.Url))
                .Where(nameof(LibraryImage.Id), id)
            );
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryImage.Url))
                .Where(nameof(LibraryImage.Title), title)
            );
        }
    }
}
