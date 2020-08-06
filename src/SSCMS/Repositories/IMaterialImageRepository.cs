using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialImageRepository : IRepository
    {
        Task<int> InsertAsync(MaterialImage image);

        Task<bool> UpdateAsync(MaterialImage image);

        Task UpdateMediaIdAsync(int id, string mediaId);

        Task<bool> DeleteAsync(int id);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<MaterialImage>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<MaterialImage> GetAsync(int id);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);

        Task<bool> IsExistsAsync(string mediaId);
    }
}
