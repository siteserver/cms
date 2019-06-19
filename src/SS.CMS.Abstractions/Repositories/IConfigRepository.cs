using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IConfigRepository : IRepository
    {
        int Insert(ConfigInfo configInfo);

        bool Update(ConfigInfo configInfo);

        bool IsInitialized();

        ConfigInfo GetConfigInfo();

        ConfigInfo Instance { get; }

        bool IsChanged { set; }
    }
}