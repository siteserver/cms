using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface ILibraryImageRepository : IRepository
    {
        Task<int> InsertAsync(LibraryImage library);

        Task<bool> UpdateAsync(LibraryImage library);

        Task<bool> DeleteAsync(int libraryId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryImage>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<LibraryImage> GetAsync(int libraryId);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}
