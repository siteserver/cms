using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SiteServer.Abstractions
{
    public interface ILibraryImageRepository : IRepository
    {
        Task<int> InsertAsync(LibraryImage image);

        Task<bool> UpdateAsync(LibraryImage image);

        Task<bool> DeleteAsync(int imageId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryImage>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}
