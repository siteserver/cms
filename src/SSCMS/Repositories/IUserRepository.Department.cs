using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserRepository
    {
        Task<List<int>> GetUserIdsAsync(int departmentId);
        
        Task UpdateDepartmentIdAsync(User user, int departmentId);
    }
}