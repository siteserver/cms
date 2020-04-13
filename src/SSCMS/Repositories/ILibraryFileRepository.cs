using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface ILibraryFileRepository : IRepository
    {
        Task<int> InsertAsync(LibraryFile library);

        Task<bool> UpdateAsync(LibraryFile library);

        Task<bool> DeleteAsync(int libraryId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryFile>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<LibraryFile> GetAsync(int libraryId);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}
