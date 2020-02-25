using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(Site site, Template template, string templateContent, int adminId);

        Task UpdateAsync(Site site, Template template, string templateContent, int adminId);

        Task SetDefaultAsync(int templateId);

        Task DeleteAsync(Site site, int templateId);

        Task CreateDefaultTemplateAsync(Site site, int adminId);
    }
}