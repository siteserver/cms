using SiteServer.CMS.Common;
using SiteServer.Plugin;

namespace SiteServer.CMS.Controllers.Admin
{
    public static partial class AdminControllers
    {
        public static class Login
        {
            public const string GetStatusRoute = "";
            public static IControllerResult GetStatus(IRequest request)
            {
                var redirect = AdminRedirectCheck(request, checkInstall: true, checkDatabaseVersion: true);
                if (redirect != null) return Ok(redirect);

                return Ok(new
                {
                    Value = true
                });
            }
        }
    }
}
