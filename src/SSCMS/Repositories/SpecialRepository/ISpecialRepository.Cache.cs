using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ISpecialRepository
    {
        Task DeleteAsync(int siteId, int specialId);

        Task<Special> GetSpecialAsync(int siteId, int specialId);

        Task<string> GetTitleAsync(int siteId, int specialId);

        Task<List<int>> GetAllSpecialIdListAsync(int siteId);
    }
}