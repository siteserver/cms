using SiteServer.CMS.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Admin
{
    public static partial class AdminControllers
    {
        private static object AdminRedirectCheck(IRequest request, bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(AppSettings.ConnectionString))
            {
                redirect = true;
                redirectUrl = PageUtilsEx.GetAdminUrl("installer/default.aspx");
            }
            else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
                     ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = AdminPagesUtils.UpdateUrl;
            }
            else if (checkLogin && !request.IsAdminLoggin)
            {
                redirect = true;
                redirectUrl = AdminPagesUtils.LoginUrl;
            }

            if (redirect)
            {
                return new
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            return null;
        }

        private static IControllerResult Ok(object result)
        {
            return new OkResult(result);
        }
    }
}
