using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IAreaRepository : IRepository
    {
        Task<int> InsertAsync(AreaInfo areaInfo);

        Task<bool> UpdateAsync(AreaInfo areaInfo);

        Task UpdateTaxisAsync(int selectedId, bool isSubtract);

        Task<bool> DeleteAsync(int areaId);

        Task<IEnumerable<int>> GetIdListByParentIdAsync(int parentId);

        // cache

        string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict);

        Task<AreaInfo> GetAreaInfoAsync(int areaId);

        Task<string> GetThisAreaNameAsync(int areaId);

        Task<string> GetAreaNameAsync(int areaId);

        Task<string> GetParentsPathAsync(int areaId);

        Task<List<int>> GetAreaIdListAsync();
    }
}
