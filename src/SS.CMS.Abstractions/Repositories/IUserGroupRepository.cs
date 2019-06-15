using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IUserGroupRepository : IRepository
    {
        int Insert(UserGroupInfo groupInfo);

        bool Update(UserGroupInfo groupInfo);

        bool Delete(int groupId);

        IList<UserGroupInfo> GetAllUserGroups();

        bool IsExists(string groupName);

        UserGroupInfo GetUserGroupInfo(int groupId);
    }
}
