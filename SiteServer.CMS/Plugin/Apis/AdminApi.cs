using BaiRong.Core;
using SiteServer.Plugin.Apis;
namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        public static AdminApi Instance { get; } = new AdminApi();

        public bool IsUserNameExists(string userName)
        {
            return BaiRongDataProvider.AdministratorDao.IsUserNameExists(userName);
        }
    }
}
