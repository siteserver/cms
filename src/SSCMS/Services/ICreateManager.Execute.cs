using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public partial interface ICreateManager
    {
        Task ExecuteAsync(int siteId, CreateType createType, int channelId = 0, int contentId = 0,
            int fileTemplateId = 0, int specialId = 0);
    }
}