using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms.Special
{
    
    [RoutePrefix("pages/cms/specialEditor")]
    public class PagesSpecialEditorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var specialId = request.GetQueryInt("specialId");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.WebSitePermissions.Template))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);
                var specialInfo = await SpecialManager.GetSpecialAsync(siteId, specialId);

                if (specialInfo == null)
                {
                    return BadRequest("专题不存在！");
                }

                var specialUrl = PageUtility.ParseNavigationUrl(site, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
                var filePath = PathUtils.Combine(SpecialManager.GetSpecialDirectoryPath(site, specialInfo.Url), "index.html");
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
