using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public interface ILibraryVideoRepository : IRepository
    {
        Task<int> InsertAsync(LibraryVideo library);

        Task<bool> UpdateAsync(LibraryVideo library);

        Task<bool> DeleteAsync(int libraryId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryVideo>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<LibraryVideo> GetAsync(int libraryId);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}