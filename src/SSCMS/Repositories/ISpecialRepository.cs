using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface ISpecialRepository : IRepository
    {
        Task<int> InsertAsync(Special special);

        Task UpdateAsync(Special special);

        Task<bool> IsTitleExistsAsync(int siteId, string title);

        Task<bool> IsUrlExistsAsync(int siteId, string url);

        Task<List<Special>> GetSpecialsAsync(int siteId);

        Task DeleteAsync(int siteId, int specialId);

        Task<Special> GetSpecialAsync(int siteId, int specialId);

        Task<string> GetTitleAsync(int siteId, int specialId);

        Task<List<int>> GetAllSpecialIdsAsync(int siteId);
    }
}