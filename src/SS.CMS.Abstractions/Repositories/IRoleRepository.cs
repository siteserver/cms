using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IRoleRepository : IRepository
    {
        string GetRoleDescription(string roleName);

        IList<string> GetRoleNameList();

        IList<string> GetRoleNameListByCreatorUserName(string creatorUserName);

        void InsertRole(RoleInfo roleInfo);

        void UpdateRole(string roleName, string description);

        void DeleteRole(string roleName);

        bool IsRoleExists(string roleName);
    }
}