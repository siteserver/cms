using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IRoleRepository : IRepository
    {
        string GetRoleDescription(string roleName);

        IList<string> GetRoleNameList();

        IList<string> GetRoleNameListByUserId(int userId);

        void InsertRole(RoleInfo roleInfo);

        void UpdateRole(string roleName, string description);

        Task DeleteRoleAsync(string roleName);

        bool IsRoleExists(string roleName);
    }
}