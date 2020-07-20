using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface ILibraryCardRepository : IRepository
    {
        Task<int> InsertAsync(LibraryCard library);

        Task<bool> UpdateAsync(LibraryCard library);

        Task<bool> DeleteAsync(int libraryId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryCard>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<LibraryCard> GetAsync(int libraryId);

        Task<string> GetBodyByIdAsync(int id);

        Task<string> GetBodyByTitleAsync(string title);
    }
}