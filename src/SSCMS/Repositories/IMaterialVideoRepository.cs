using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialVideoRepository : IRepository
    {
        Task<int> InsertAsync(MaterialVideo video);

        Task<bool> UpdateAsync(MaterialVideo video);

        Task UpdateMediaIdAsync(int id, string mediaId);

        Task<bool> DeleteAsync(int id);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<MaterialVideo>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<MaterialVideo> GetAsync(int id);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);

        Task<bool> IsExistsAsync(string mediaId);
    }
}