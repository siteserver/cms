using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialFileRepository : IRepository
    {
        Task<int> InsertAsync(MaterialFile file);

        Task<bool> UpdateAsync(MaterialFile file);

        Task<bool> DeleteAsync(int id);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<MaterialFile>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<MaterialFile> GetAsync(int id);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);

        Task<bool> IsExistsAsync(string mediaId);
    }
}