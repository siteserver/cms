using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SiteServer.Abstractions
{
    public interface ILibraryFileRepository : IRepository
    {
        Task<int> InsertAsync(LibraryFile image);

        Task<bool> UpdateAsync(LibraryFile image);

        Task<bool> DeleteAsync(int fileId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryFile>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}
