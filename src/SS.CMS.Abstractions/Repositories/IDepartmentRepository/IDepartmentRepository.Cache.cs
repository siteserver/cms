using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IDepartmentRepository
    {
        Task<List<KeyValuePair<int, string>>> GetRestDepartmentsAsync();

        string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict);

        Task<DepartmentInfo> GetDepartmentInfoAsync(int departmentId);

        Task<string> GetDepartmentNameAsync(int departmentId);

        Task<string> GetParentsPathAsync(int departmentId);

        Task<List<int>> GetDepartmentIdListAsync();

        Task<List<KeyValuePair<int, DepartmentInfo>>> GetDepartmentInfoKeyValuePairAsync();
    }
}
