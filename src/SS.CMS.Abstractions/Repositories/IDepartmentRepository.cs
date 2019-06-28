using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IDepartmentRepository : IRepository
    {
        int Insert(DepartmentInfo departmentInfo);

        bool Update(DepartmentInfo departmentInfo);

        void UpdateTaxis(int selectedId, bool isSubtract);

        Task<bool> DeleteAsync(int id);

        IList<int> GetIdListByParentId(int parentId);

        List<KeyValuePair<int, DepartmentInfo>> GetDepartmentInfoKeyValuePair();
    }
}
