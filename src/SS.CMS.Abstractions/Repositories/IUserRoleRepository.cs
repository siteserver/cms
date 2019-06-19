using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IUserRoleRepository : IRepository
    {
        IList<string> GetUserNameListByRoleName(string roleName);

        IList<string> GetRoles(string userName);

        void RemoveUser(string userName);

        void RemoveUserFromRole(string userName, string roleName);

        bool IsUserInRole(string userName, string roleName);

        int AddUserToRole(string userName, string roleName);
    }
}
