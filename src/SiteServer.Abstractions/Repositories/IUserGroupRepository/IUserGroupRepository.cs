using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IUserGroupRepository : IRepository
    {
        Task<int> InsertAsync(UserGroup groupInfo);

        Task<bool> UpdateAsync(UserGroup groupInfo);

        Task<bool> DeleteAsync(int groupId);
    }
}
