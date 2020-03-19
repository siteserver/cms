using System.Threading.Tasks;
using CacheManager.Core;

namespace SSCMS.Abstractions
{
    public partial interface IConfigRepository
    {
        Task<Config> GetAsync();
    }
}
