using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task PullMaterialAsync(string token, MaterialType materialType, int groupId);

        Task<string> PushMaterialAsync(string token, MaterialType materialType, int materialId, bool syncUrl);

        Task PullMenuAsync(string token, int siteId);

        Task PushMenuAsync(string token, int siteId);
    }
}
