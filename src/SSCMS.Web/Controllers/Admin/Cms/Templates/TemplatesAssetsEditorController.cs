using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesAssetsEditorController : ControllerBase
    {
        private const string Route = "cms/templates/templatesAssetsEditor";

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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.TemplatesAssets))
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.TemplatesAssets))
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.TemplatesAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveFile(request, site, false);
        }
    }
}
