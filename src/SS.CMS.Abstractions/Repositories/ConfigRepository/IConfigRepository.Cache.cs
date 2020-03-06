using System.Threading.Tasks;
using CacheManager.Core;

namespace SS.CMS.Abstractions
{
    public partial interface IConfigRepository
    {
        Task<Config> GetAsync();
    }
}
