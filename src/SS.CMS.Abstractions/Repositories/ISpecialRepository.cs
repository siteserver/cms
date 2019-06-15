using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface ISpecialRepository : IRepository
    {
        int Insert(SpecialInfo specialInfo);

        bool Update(SpecialInfo specialInfo);

        SpecialInfo Delete(int siteId, int specialId);

        bool IsTitleExists(int siteId, string title);

        bool IsUrlExists(int siteId, string url);

        IList<SpecialInfo> GetSpecialInfoList(int siteId);

        IList<SpecialInfo> GetSpecialInfoList(int siteId, string keyword);
    }
}