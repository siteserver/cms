using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class UserRepository
    {
        public async Task<List<int>> GetUserIdsAsync(int departmentId)
        {
            var query = Q.OrderByDesc(nameof(User.Id));

            if (departmentId >= 0)
            {
                query.Where(nameof(User.DepartmentId), departmentId);
            }

            return await _repository.GetAllAsync<int>(query);
        }
        
        public async Task UpdateDepartmentIdAsync(User user, int departmentId)
        {
            if (user.DepartmentId != departmentId)
            {
                user.DepartmentId = departmentId;

                await _repository.UpdateAsync(Q
                    .Set(nameof(User.DepartmentId), departmentId)
                    .Where(nameof(User.Id), user.Id)
                    .CachingRemove(GetCacheKeysToRemove(user))
                );
            }

            await SyncDepartmentCountAsync(user.DepartmentId);
            await SyncDepartmentCountAsync(departmentId);
        }

        public async Task SyncDepartmentCountAsync(int departmentId)
        {
            if (departmentId > 0)
            {
                var count = await _repository.CountAsync(Q
                    .Where(nameof(User.DepartmentId), departmentId)
                );
                await _departmentRepository.UpdateCountAsync(departmentId, count);
            }
        }
    }
}

