using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public partial interface ICreateManager
    {
        Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId);
    }
}