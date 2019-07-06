using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IAreaRepository
    {
        Task<List<KeyValuePair<int, string>>> GetRestAreasAsync();

        string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict);

        Task<AreaInfo> GetAreaInfoAsync(int areaId);

        Task<string> GetThisAreaNameAsync(int areaId);

        Task<string> GetAreaNameAsync(int areaId);

        Task<string> GetParentsPathAsync(int areaId);

        Task<List<int>> GetAreaIdListAsync();
    }
}
