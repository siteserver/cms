using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesSpecialEditor")]
    public partial class TemplatesSpecialEditorController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public TemplatesSpecialEditorController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var specialInfo = await DataProvider.SpecialRepository.GetSpecialAsync(request.SiteId, request.SpecialId);

            if (specialInfo == null)
            {
                return this.Error("专题不存在！");
            }

            var specialUrl = await PageUtility.ParseNavigationUrlAsync(site, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
            var filePath = PathUtils.Combine(await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, specialInfo.Url), "index.html");
            var html = FileUtils.ReadText(filePath, Encoding.UTF8);

            return new GetResult
            {
                Special = specialInfo,
                SpecialUrl = specialUrl,
                Html = html
            };
        }
    }
}
