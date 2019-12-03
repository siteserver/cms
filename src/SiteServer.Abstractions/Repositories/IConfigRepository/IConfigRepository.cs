using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface IConfigRepository : IRepository
    {
        Task InsertAsync(Config configInfo);

        Task<bool> UpdateAsync(Config configInfo);

        Task DeleteAllAsync();

        Task<Config> GetConfigInfoAsync();
    }
}