using System.Threading.Tasks;
using SS.CMS.Abstractions.Enums;

namespace SS.CMS.Abstractions.Services
{
    public interface ICreateManager
    {
        void CreateByAll(int siteId);

        void CreateByTemplate(int siteId, int templateId);

        void CreateChannel(int siteId, int channelId);

        void CreateContent(int siteId, int channelId, int contentId);

        void CreateAllContent(int siteId, int channelId);

        void CreateFile(int siteId, int fileTemplateId);

        void CreateSpecial(int siteId, int specialId);

        void TriggerContentChangedEvent(int siteId, int channelId);

        Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId);
    }
}
