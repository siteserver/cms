using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages
{
    [OpenApiIgnore]
    [RoutePrefix("pages/syncDatabase")]
    public class PagesSyncDatabaseController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                //var request = await AuthenticatedRequest.GetRequestAsync();
                //if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
                //{
                //    return Unauthorized();
                //}

                if (await SystemManager.IsNeedInstallAsync())
                {
                    return BadRequest("系统未安装，向导被禁用！");
                }

                await SystemManager.SyncDatabaseAsync();

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