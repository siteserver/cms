using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class LibraryAudioRepository : ILibraryAudioRepository
    {
        private readonly Repository<LibraryAudio> _repository;

        public LibraryAudioRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<LibraryAudio>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(LibraryAudio library)
        {
            return await _repository.InsertAsync(library);
        }

        public async Task<bool> UpdateAsync(LibraryAudio library)
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
                query.Where(nameof(LibraryAudio.GroupId), groupId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query.Where(q => q
                    .WhereLike(nameof(LibraryAudio.Title), $"%{keyword}%")
                    .OrWhere(nameof(LibraryAudio.FileType), keyword.ToUpper())
                );
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<LibraryAudio>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var query = Q
                .OrderByDesc(nameof(LibraryAudio.Id))
                .ForPage(page, perPage);

            if (groupId > 0)
            {
                query.Where(nameof(LibraryAudio.GroupId), groupId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.Where(q => q
                    .WhereLike(nameof(LibraryAudio.Title), $"%{keyword}%")
                    .OrWhere(nameof(LibraryAudio.FileType), keyword.ToUpper())
                );
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<LibraryAudio> GetAsync(int libraryId)
        {
            return await _repository.GetAsync(libraryId);
        }

        public async Task<string> GetUrlByIdAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryAudio.Url))
                .Where(nameof(LibraryAudio.Id), id)
            );
        }

        public async Task<string> GetUrlByTitleAsync(string title)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(LibraryAudio.Url))
                .Where(nameof(LibraryAudio.Title), title)
            );
        }
    }
}
