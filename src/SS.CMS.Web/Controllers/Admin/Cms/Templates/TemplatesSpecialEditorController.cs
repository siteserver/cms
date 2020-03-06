using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesSpecialEditor")]
    public partial class TemplatesSpecialEditorController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;

        public TemplatesSpecialEditorController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ISpecialRepository specialRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
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

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialInfo = await _specialRepository.GetSpecialAsync(request.SiteId, request.SpecialId);

            if (specialInfo == null)
            {
                return this.Error("专题不存在！");
            }

            var specialUrl = await _pathManager.ParseNavigationUrlAsync(site, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
            var filePath = PathUtils.Combine(await _pathManager.GetSpecialDirectoryPathAsync(site, specialInfo.Url), "index.html");
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
