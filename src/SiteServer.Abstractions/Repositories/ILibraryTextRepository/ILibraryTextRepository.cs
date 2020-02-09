using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SiteServer.Abstractions
{
    public interface ILibraryTextRepository : IRepository
    {
        Task<int> InsertAsync(LibraryText text);

        Task<bool> UpdateAsync(LibraryText text);

        Task<bool> DeleteAsync(int textId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryText>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<string> GetContentByIdAsync(int id);

        Task<string> GetContentByTitleAsync(string title);
    }
}
