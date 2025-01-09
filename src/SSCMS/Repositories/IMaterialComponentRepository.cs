using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialComponentRepository : IRepository
    {
        Task<int> InsertAsync(MaterialComponent component);

        Task<bool> UpdateAsync(MaterialComponent component);

        Task<bool> DeleteAsync(int id);

        Task<bool> IsExistsAsync(string title);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<MaterialComponent>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<MaterialComponent> GetAsync(int id);

        Task<string> GetImageUrlByIdAsync(int id);

        Task<string> GetImageUrlByTitleAsync(string title);
    }
}