using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IRoleRepository : IRepository
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