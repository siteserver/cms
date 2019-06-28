using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IUserGroupRepository : IRepository
    {
        int Insert(UserGroupInfo groupInfo);

        bool Update(UserGroupInfo groupInfo);

        Task<bool> DeleteAsync(int groupId);

        IList<UserGroupInfo> GetAllUserGroups();

        bool IsExists(string groupName);

        UserGroupInfo GetUserGroupInfo(int groupId);
    }
}
