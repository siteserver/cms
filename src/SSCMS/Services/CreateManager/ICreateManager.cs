using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ICreateManager
    {
        Task CreateByAllAsync(int siteId);

        Task CreateByTemplateAsync(int siteId, int templateId);

        Task CreateChannelAsync(int siteId, int channelId);

        Task CreateContentAsync(int siteId, int channelId, int contentId);

        Task CreateAllContentAsync(int siteId, int channelId);

        Task CreateFileAsync(int siteId, int fileTemplateId);

        Task CreateSpecialAsync(int siteId, int specialId);

        Task TriggerContentChangedEventAsync(int siteId, int channelId);
    }
}
