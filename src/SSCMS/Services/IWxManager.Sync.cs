using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task PullMaterialAsync(string accessTokenOrAppId, MaterialType materialType, int groupId);

        Task<string> PushMaterialAsync(string accessTokenOrAppId, MaterialType materialType, int materialId);

        Task PullMenuAsync(string accessTokenOrAppId, int siteId);

        Task PushMenuAsync(string accessTokenOrAppId, int siteId);
    }
}
