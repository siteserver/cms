using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SiteServer.Abstractions
{
    public interface ILibraryVideoRepository : IRepository
    {
        Task<int> InsertAsync(LibraryVideo video);

        Task<bool> UpdateAsync(LibraryVideo video);

        Task<bool> DeleteAsync(int videoId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryVideo>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}
