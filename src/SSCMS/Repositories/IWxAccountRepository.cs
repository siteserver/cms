using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxAccountRepository : IRepository
    {
        Task SetAsync(WxAccount account);

        Task DeleteAllAsync(int siteId);

        Task<WxAccount> GetBySiteIdAsync(int siteId);
    }
}