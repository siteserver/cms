using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITemplateRepository
    {
        Task<Template> GetTemplateInfoAsync(int templateId);
    }
}
