using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IDepartmentRepository : IRepository
    {
        Task<int> InsertAsync(Department departmentInfo);

        Task<bool> UpdateAsync(Department departmentInfo);

        Task UpdateTaxisAsync(int selectedId, bool isSubtract);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<int>> GetIdListByParentIdAsync(int parentId);
    }
}
