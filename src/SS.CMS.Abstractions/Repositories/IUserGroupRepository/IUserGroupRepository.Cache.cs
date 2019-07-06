using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Repositories
{
    public partial interface IUserGroupRepository
    {
        Task<IList<UserGroupInfo>> GetAllUserGroupsAsync();

        Task<bool> IsExistsAsync(string groupName);

        Task<UserGroupInfo> GetUserGroupInfoAsync(int groupId);
    }
}
