using System;
using System.Web.Http;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/createStatus")]
    public class PagesCreateStatusController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(request.SiteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                var siteId = request.SiteId;

                var summary = CreateTaskManager.GetTaskSummary(siteId);

                return Ok(new
                {
                    Value = summary
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}