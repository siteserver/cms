using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ISpecialRepository : IRepository
    {
        int Insert(SpecialInfo specialInfo);

        bool Update(SpecialInfo specialInfo);

        Task<SpecialInfo> DeleteAsync(int siteId, int specialId);

        bool IsTitleExists(int siteId, string title);

        bool IsUrlExists(int siteId, string url);

        IList<SpecialInfo> GetSpecialInfoList(int siteId);

        IList<SpecialInfo> GetSpecialInfoList(int siteId, string keyword);
    }
}