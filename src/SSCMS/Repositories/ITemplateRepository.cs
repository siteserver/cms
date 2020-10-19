using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(Template template);

        Task UpdateAsync(Template template);

        Task SetDefaultAsync(int templateId);

        Task DeleteAsync(IPathManager pathManager, Site site, int templateId);

        Task CreateDefaultTemplateAsync(int siteId);
    }
}