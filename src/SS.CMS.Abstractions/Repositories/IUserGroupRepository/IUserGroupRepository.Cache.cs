using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Repositories
{
    public partial interface IUserGroupRepository
    {
        Task<IList<UserGroup>> GetAllUserGroupsAsync();

        Task<bool> IsExistsAsync(string groupName);

        Task<UserGroup> GetUserGroupInfoAsync(int groupId);
    }
}
