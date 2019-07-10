using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IAreaRepository : IRepository
    {
        Task<int> InsertAsync(Area areaInfo);

        Task<bool> UpdateAsync(Area areaInfo);

        Task UpdateTaxisAsync(int selectedId, bool isSubtract);

        Task<bool> DeleteAsync(int areaId);

        Task<IEnumerable<int>> GetIdListByParentIdAsync(int parentId);
    }
}
