using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IDepartmentRepository : IRepository
    {
        Task<int> InsertAsync(DepartmentInfo departmentInfo);

        Task<bool> UpdateAsync(DepartmentInfo departmentInfo);

        Task UpdateTaxisAsync(int selectedId, bool isSubtract);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<int>> GetIdListByParentIdAsync(int parentId);
    }
}
