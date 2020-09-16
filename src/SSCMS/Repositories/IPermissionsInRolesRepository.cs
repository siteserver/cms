using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IPermissionsInRolesRepository : IRepository
    {
        Task InsertAsync(PermissionsInRoles pr);

        Task DeleteAsync(string roleName);

        Task<List<string>> GetAppPermissionsAsync(IEnumerable<string> roles);
    }
}
