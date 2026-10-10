using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IAuthManager
    {
        Task<bool> IsSuperAdminAsync();

        Task<bool> IsSiteAdminAsync();

        Task<bool> IsSiteAdminAsync(int siteId);

        Task<string> GetAdminLevelAsync();

        Task<List<int>> GetSiteIdsAsync();

        Task<List<int>> GetContentPermissionsChannelIdsAsync(int siteId);

        Task<List<int>> GetVisibleChannelIdsAsync(List<int> channelIdsWithPermissions);

        Task<List<int>> GetContentPermissionsChannelIdsAsync(int siteId, params string[] permissions);

        Task<bool> HasAppPermissionsAsync(params string[] permissions);

        Task<List<string>> GetAppPermissionsAsync();

        Task<bool> HasSitePermissionsAsync(int siteId);

        Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions);

        Task<List<string>> GetSitePermissionsAsync(int siteId);

        Task<bool> HasContentPermissionsAsync(int siteId, int channelId, params string[] permissions);

        Task<bool> HasContentPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetContentPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetContentPermissionsAsync(int siteId);

        /// <summary>
        /// 判断当前管理员是否可以管理（查看详情/修改/删除/锁定/授权）目标管理员。
        /// 超级管理员可管理所有人；其他管理员需具备管理员管理权限，且目标为本人创建、非超级管理员。
        /// </summary>
        Task<bool> CanManageAdministratorAsync(Models.Administrator target);
    }
}
