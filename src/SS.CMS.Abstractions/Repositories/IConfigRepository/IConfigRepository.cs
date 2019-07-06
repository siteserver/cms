using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IConfigRepository : IRepository
    {
        Task InsertAsync(ConfigInfo configInfo);

        Task<bool> UpdateAsync(ConfigInfo configInfo);

        Task<ConfigInfo> GetConfigInfoAsync();
    }
}