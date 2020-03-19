using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IAdministratorRepository
    {
        Task AddUserToRolesAsync(string userName, string[] roleNames);

        Task AddUserToRoleAsync(string userName, string roleName);

        Task<string> GetRolesAsync(string userName);
    }
}
