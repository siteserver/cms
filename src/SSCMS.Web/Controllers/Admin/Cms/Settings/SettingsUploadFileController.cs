using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsUploadFileController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadFile";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadFileController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<Site>>> GetConfig([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsUploadFile))
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsUploadFile))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.FileUploadDirectoryName = request.FileUploadDirectoryName;
            site.FileUploadDateFormatString = request.FileUploadDateFormatString;
            site.IsFileUploadChangeFileName = request.IsFileUploadChangeFileName;
            site.FileUploadExtensions = request.FileUploadExtensions.Replace("|", ",");
            site.FileUploadTypeMaxSize = request.FileUploadTypeMaxSize;

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改视频上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
