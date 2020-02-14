using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    
    [RoutePrefix("pages/cms/templates/templatesSpecialEditor")]
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
                        Constants.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var specialInfo = await DataProvider.SpecialRepository.GetSpecialAsync(siteId, specialId);

                if (specialInfo == null)
                {
                    return BadRequest("专题不存在！");
                }

                var specialUrl = PageUtility.ParseNavigationUrlAsync(site, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
                var filePath = PathUtils.Combine(await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, specialInfo.Url), "index.html");
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
