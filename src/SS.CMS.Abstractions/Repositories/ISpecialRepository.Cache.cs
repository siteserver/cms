using System.Collections.Generic;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface ISpecialRepository
    {
        SpecialInfo GetSpecialInfo(int siteId, int specialId);

        string GetTitle(int siteId, int specialId);

        List<TemplateInfo> GetTemplateInfoList(SiteInfo siteInfo, int specialId, IPathManager pathManager);

        List<int> GetAllSpecialIdList(int siteId);

        void RemoveCache(int siteId);
    }
}
