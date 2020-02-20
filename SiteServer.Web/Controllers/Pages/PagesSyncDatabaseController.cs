using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/syncDatabase")]
    public class PagesSyncDatabaseController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                //var request = await AuthenticatedRequest.GetAuthAsync();
                //if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
                //{
                //    return Unauthorized();
                //}

                if (await SystemManager.IsNeedInstallAsync())
                {
                    return BadRequest("系统未安装，向导被禁用！");
                }

                await DataProvider.DatabaseRepository.SyncDatabaseAsync();

                return Ok(new
                {
                    Version = SystemManager.ProductVersion
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}