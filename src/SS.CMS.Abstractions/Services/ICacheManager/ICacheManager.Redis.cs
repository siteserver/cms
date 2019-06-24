using System.Threading.Tasks;

namespace SS.CMS.Services
{
    public partial interface ICacheManager
    {
        Task<(bool IsConnectionWorks, string ErrorMessage)> IsRedisConnectionWorksAsync(string redisConnectionString);
    }
}
