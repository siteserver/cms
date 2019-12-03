using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IDepartmentRepository
    {
        Task<List<KeyValuePair<int, string>>> GetRestDepartmentsAsync();

        string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict);

        Task<Department> GetDepartmentInfoAsync(int departmentId);

        Task<string> GetDepartmentNameAsync(int departmentId);

        Task<string> GetParentsPathAsync(int departmentId);

        Task<List<int>> GetDepartmentIdListAsync();

        Task<List<KeyValuePair<int, Department>>> GetDepartmentInfoKeyValuePairAsync();
    }
}
