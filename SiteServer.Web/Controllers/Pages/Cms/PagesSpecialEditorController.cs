using System;
using System.Text;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/specialEditor")]
    public class PagesSpecialEditorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var specialId = request.GetQueryInt("specialId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var specialInfo = SpecialManager.GetSpecialInfo(siteId, specialId);

                if (specialInfo == null)
                {
                    return BadRequest("专题不存在！");
                }

                var specialUrl = PageUtility.ParseNavigationUrl(siteInfo, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
                var filePath = PathUtils.Combine(SpecialManager.GetSpecialDirectoryPath(siteInfo, specialInfo.Url), "index.html");
                var html = FileUtils.ReadText(filePath, Encoding.UTF8);

                return Ok(new
                {
                    Value = specialInfo,
                    SpecialUrl = specialUrl,
                    Html = html
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
