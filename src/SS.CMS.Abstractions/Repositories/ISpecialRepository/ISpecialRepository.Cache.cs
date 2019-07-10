using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface ISpecialRepository
    {
        Task<Special> GetSpecialInfoAsync(int siteId, int specialId);

        Task<string> GetTitleAsync(int siteId, int specialId);

        Task<List<Template>> GetTemplateInfoListAsync(Site siteInfo, int specialId, IPathManager pathManager);

        Task<List<int>> GetAllSpecialIdListAsync(int siteId);
    }
}
