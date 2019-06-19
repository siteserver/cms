using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IAreaRepository : IRepository
    {
        int Insert(AreaInfo areaInfo);

        bool Update(AreaInfo areaInfo);

        void UpdateTaxis(int selectedId, bool isSubtract);

        bool Delete(int areaId);

        IEnumerable<int> GetIdListByParentId(int parentId);

        // cache

        List<KeyValuePair<int, string>> GetRestAreas();

        string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict);

        AreaInfo GetAreaInfo(int areaId);

        string GetThisAreaName(int areaId);

        string GetAreaName(int areaId);

        string GetParentsPath(int areaId);

        List<int> GetAreaIdList();

        void ClearCache();
    }
}
