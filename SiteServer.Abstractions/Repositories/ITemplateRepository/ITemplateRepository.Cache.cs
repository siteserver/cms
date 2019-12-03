using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface ITemplateRepository
    {
        Task<Template> GetTemplateInfoAsync(int templateId);
    }
}
