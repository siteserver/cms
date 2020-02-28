using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesAssetsEditor")]
    public partial class TemplatesAssetsEditorController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public TemplatesAssetsEditorController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] FileRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var filePath = await _pathManager.GetSitePathAsync(site, request.DirectoryPath, request.FileName);
            var content = string.Empty;
            if (FileUtils.IsFileExists(filePath))
            {
                content = await FileUtils.ReadTextAsync(filePath);
            }
            var extName = PathUtils.GetExtension(filePath).ToLower();
            var path = string.Empty;

            if (StringUtils.EqualsIgnoreCase(extName, ".html"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsIncludeDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(extName, ".css"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsCssDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(extName, ".js"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsJsDir,
                    string.Empty), request.FileName);
            }

            return new GetResult
            {
                Path = StringUtils.TrimSlash(PathUtils.RemoveExtension(path)),
                ExtName = extName,
                Content = content
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<ContentResult>> Add([FromBody] ContentRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveFile(request, site, false);
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<ContentResult>> Edit([FromBody] ContentRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveFile(request, site, false);
        }
    }
}
