using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IConfigRepository : IRepository
    {
        Task InsertAsync(Config configInfo);

        Task<bool> UpdateAsync(Config configInfo);

        Task DeleteAllAsync();

        Task<Config> GetConfigInfoAsync();
    }
}