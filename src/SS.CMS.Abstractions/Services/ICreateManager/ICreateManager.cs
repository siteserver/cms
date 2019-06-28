using System.Threading.Tasks;
using SS.CMS.Enums;

namespace SS.CMS.Services
{
    public interface ICreateManager
    {
        Task AddCreateByAllTaskAsync(int siteId);

        Task AddCreateByTemplateTaskAsync(int siteId, int templateId);

        Task AddCreateChannelTaskAsync(int siteId, int channelId);

        Task AddCreateContentTaskAsync(int siteId, int channelId, int contentId);

        Task AddCreateAllContentTaskAsync(int siteId, int channelId);

        Task AddCreateFileTaskAsync(int siteId, int fileTemplateId);

        Task AddCreateSpecialTaskAsync(int siteId, int specialId);

        Task TriggerContentChangedEventAsync(int siteId, int channelId);

        Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
                    int fileTemplateId, int specialId);
    }
}
