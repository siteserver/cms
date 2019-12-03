using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    
    [RoutePrefix("pages/settings/utilityDbLogDelete")]
    public class PagesUtilityDbLogDeleteController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                var dt = await DataProvider.LogRepository.GetLastRemoveLogDateAsync();
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                await DataProvider.DatabaseRepository.DeleteDbLogAsync();

                await request.AddAdminLogAsync("清空数据库日志");

                var dt = await DataProvider.LogRepository.GetLastRemoveLogDateAsync();
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
