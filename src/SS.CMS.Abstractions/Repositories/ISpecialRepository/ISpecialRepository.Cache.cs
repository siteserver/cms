using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface ISpecialRepository
    {
        Task<SpecialInfo> GetSpecialInfoAsync(int siteId, int specialId);

        Task<string> GetTitleAsync(int siteId, int specialId);

        Task<List<TemplateInfo>> GetTemplateInfoListAsync(SiteInfo siteInfo, int specialId, IPathManager pathManager);

        Task<List<int>> GetAllSpecialIdListAsync(int siteId);
    }
}
