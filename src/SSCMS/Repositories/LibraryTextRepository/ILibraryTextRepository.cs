using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    public interface ILibraryTextRepository : IRepository
    {
        Task<int> InsertAsync(LibraryText library);

        Task<bool> UpdateAsync(LibraryText library);

        Task<bool> DeleteAsync(int libraryId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryText>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<LibraryText> GetAsync(int libraryId);

        Task<string> GetContentByIdAsync(int id);

        Task<string> GetContentByTitleAsync(string title);
    }
}