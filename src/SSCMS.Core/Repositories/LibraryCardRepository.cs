using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class LibraryCardRepository : ILibraryCardRepository
    {
        private readonly Repository<LibraryCard> _repository;

        public LibraryCardRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<LibraryCard>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(LibraryCard library)
        {
            return await _repository.InsertAsync(library);
        }

        public async Task<bool> UpdateAsync(LibraryCard library)
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
                query.Where(nameof(LibraryCard.GroupId), groupId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryCard.Title), $"%{keyword}%");
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<LibraryCard>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var query = Q
                .OrderByDesc(nameof(LibraryCard.Id))
                .ForPage(page, perPage);

            if (groupId > 0)
            {
                query.Where(nameof(LibraryCard.GroupId), groupId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(LibraryCard.Title), $"%{keyword}%");
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<LibraryCard> GetAsync(int libraryId)
        {
            return await _repository.GetAsync(libraryId);
        }

        public async Task<string> GetBodyByIdAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryCard.Body))
                .Where(nameof(LibraryCard.Id), id)
            );
        }

        public async Task<string> GetBodyByTitleAsync(string title)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryCard.Body))
                .Where(nameof(LibraryCard.Title), title)
            );
        }
    }
}
