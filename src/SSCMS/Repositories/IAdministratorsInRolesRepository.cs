using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SSCMS.Repositories
{
    public interface IAdministratorsInRolesRepository : IRepository
    {
        Task<IList<string>> GetRolesForUserAsync(string userName);

        Task<IList<string>> GetUsersInRoleAsync(string roleName);

        Task InsertAsync(string userName, string roleName);

        Task RemoveUserAsync(string userName);
    }
}
