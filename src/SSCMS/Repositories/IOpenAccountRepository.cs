using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IOpenAccountRepository : IRepository
    {
        Task SetAsync(OpenAccount account);

        Task DeleteBySiteIdAsync(int siteId);

        Task<OpenAccount> GetBySiteIdAsync(int siteId);
    }
}