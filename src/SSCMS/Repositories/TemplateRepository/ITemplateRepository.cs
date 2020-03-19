using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(IPathManager pathManager, Site site, Template template, string templateContent, int adminId);

        Task UpdateAsync(IPathManager pathManager, Site site, Template template, string templateContent, int adminId);

        Task SetDefaultAsync(int templateId);

        Task DeleteAsync(IPathManager pathManager, Site site, int templateId);

        Task CreateDefaultTemplateAsync(IPathManager pathManager, Site site, int adminId);
    }
}