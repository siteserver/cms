using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxUserRepository : IRepository
    {
        Task<int> InsertAsync(int siteId, WxUser user);

        Task UpdateAllAsync(int siteId, List<WxUser> users);

        Task DeleteAllAsync(int siteId, List<string> openIds);

        Task<(int Total, int Count, List<string> Results)> GetPageOpenIds(int siteId, int tagId, string keyword, int page, int perPage);

        Task<List<string>> GetAllOpenIds(int siteId);
    }
}