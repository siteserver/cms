using System.Threading.Tasks;
using CacheManager.Core;

namespace SSCMS
{
    public partial interface IConfigRepository
    {
        Task<Config> GetAsync();
    }
}
