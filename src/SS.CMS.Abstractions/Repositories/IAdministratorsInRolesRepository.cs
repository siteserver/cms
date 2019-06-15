using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IAdministratorsInRolesRepository : IRepository
    {
        IEnumerable<string> GetUserNameListByRoleName(string roleName);

        IEnumerable<string> GetRolesForUser(string userName);

        void RemoveUser(string userName);

        void RemoveUserFromRole(string userName, string roleName);

        bool IsUserInRole(string userName, string roleName);

        int AddUserToRole(string userName, string roleName);
    }
}
