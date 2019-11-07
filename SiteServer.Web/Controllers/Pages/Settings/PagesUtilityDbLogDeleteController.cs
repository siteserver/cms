using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/utilityDbLogDelete")]
    public class PagesUtilityDbLogDeleteController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                var dt = DataProvider.LogDao.GetLastRemoveLogDate();
                var lastExecuteDate = dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);

                return Ok(new
                {
                    Value = lastExecuteDate
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                DataProvider.DatabaseDao.DeleteDbLog();

                await request.AddAdminLogAsync("清空数据库日志");

                var dt = DataProvider.LogDao.GetLastRemoveLogDate();
                var lastExecuteDate = dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);

                return Ok(new
                {
                    Value = lastExecuteDate
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
