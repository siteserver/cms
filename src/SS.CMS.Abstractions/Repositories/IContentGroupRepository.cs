using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IContentGroupRepository : IRepository
    {
        int Insert(ContentGroupInfo groupInfo);

        bool Update(ContentGroupInfo groupInfo);

        Task DeleteAsync(int siteId, string groupName);

        void UpdateTaxisToUp(int siteId, string groupName);

        void UpdateTaxisToDown(int siteId, string groupName);

        // cache

        Dictionary<int, List<ContentGroupInfo>> GetAllContentGroups();

        bool IsExists(int siteId, string groupName);

        ContentGroupInfo GetContentGroupInfo(int siteId, string groupName);

        List<string> GetGroupNameList(int siteId);

        List<ContentGroupInfo> GetContentGroupInfoList(int siteId);
    }
}