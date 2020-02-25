using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface ICreateManager
    {
        Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId);
    }
}