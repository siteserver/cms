using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesSpecialEditorController : ControllerBase
    {
        private const string Route = "cms/templates/templatesSpecialEditor";

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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialInfo = await _specialRepository.GetSpecialAsync(request.SiteId, request.SpecialId);

            if (specialInfo == null)
            {
                return this.Error("专题不存在！");
            }

            var specialUrl = await _pathManager.ParseSiteUrlAsync(site, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
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
