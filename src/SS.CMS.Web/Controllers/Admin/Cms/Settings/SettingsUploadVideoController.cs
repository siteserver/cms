using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsUploadVideo")]
    public partial class SettingsUploadVideoController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadVideoController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<Site>>> GetConfig([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigUpload))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigUpload))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.VideoUploadDirectoryName = request.VideoUploadDirectoryName;
            site.VideoUploadDateFormatString = request.VideoUploadDateFormatString;
            site.IsVideoUploadChangeFileName = request.IsVideoUploadChangeFileName;
            site.VideoUploadTypeCollection = request.VideoUploadTypeCollection.Replace("|", ",");
            site.VideoUploadTypeMaxSize = request.VideoUploadTypeMaxSize;

            await _siteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改视频上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
