using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IPermissionsInRolesRepository : IRepository
    {
        void Insert(PermissionsInRolesInfo info);

        bool Delete(string roleName);

        void UpdateRoleAndGeneralPermissions(string roleName, string description, List<string> generalPermissionList);

        List<string> GetGeneralPermissionList(IEnumerable<string> roles);
    }
}