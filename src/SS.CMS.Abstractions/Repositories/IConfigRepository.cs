using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IConfigRepository : IRepository
    {
        int Insert(ConfigInfo configInfo);

        bool Update(ConfigInfo configInfo);

        bool IsInitialized();

        ConfigInfo GetConfigInfo();
    }
}