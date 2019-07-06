using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IAreaRepository : IRepository
    {
        Task<int> InsertAsync(AreaInfo areaInfo);

        Task<bool> UpdateAsync(AreaInfo areaInfo);

        Task UpdateTaxisAsync(int selectedId, bool isSubtract);

        Task<bool> DeleteAsync(int areaId);

        Task<IEnumerable<int>> GetIdListByParentIdAsync(int parentId);
    }
}
