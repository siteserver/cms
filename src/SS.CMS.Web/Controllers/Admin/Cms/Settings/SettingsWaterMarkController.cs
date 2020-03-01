using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsWaterMark")]
    public partial class SettingsWaterMarkController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsWaterMarkController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var families = new InstalledFontCollection().Families.Select(x => x.Name);
            var imageUrl = await _pathManager.ParseNavigationUrlAsync(site, site.WaterMarkImagePath, true);

            return new GetResult
            {
                Site = site,
                Families = families,
                ImageUrl = imageUrl
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var fileExtName = PathUtils.GetExtension(fileName).ToLower();
            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, fileExtName);
            var localFileName = _pathManager.GetUploadFileName(site, fileName);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            if (!FileUtils.IsImage(fileExtName))
            {
                return this.Error("请选择有效的图片文件上传");
            }

            await _pathManager.UploadAsync(file, filePath);

            var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
            var virtualUrl = _pathManager.GetVirtualUrl(site, imageUrl);

            return new UploadResult
            {
                ImageUrl = imageUrl,
                VirtualUrl = virtualUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsWaterMark = request.IsWaterMark;
            site.WaterMarkPosition = request.WaterMarkPosition;
            site.WaterMarkTransparency = request.WaterMarkTransparency;
            site.WaterMarkMinWidth = request.WaterMarkMinWidth;
            site.WaterMarkMinHeight = request.WaterMarkMinHeight;
            site.IsImageWaterMark = request.IsImageWaterMark;
            site.WaterMarkFormatString = request.WaterMarkFormatString;
            site.WaterMarkFontName = request.WaterMarkFontName;
            site.WaterMarkFontSize = request.WaterMarkFontSize;
            site.WaterMarkImagePath = _pathManager.GetVirtualUrl(site, request.WaterMarkImagePath);

            await _siteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改图片水印设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
