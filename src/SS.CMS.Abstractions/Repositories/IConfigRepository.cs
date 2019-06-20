using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IConfigRepository : IRepository
    {
        void Insert(ConfigInfo configInfo);

        bool Update(ConfigInfo configInfo);

        ConfigInfo Instance { get; }
    }
}