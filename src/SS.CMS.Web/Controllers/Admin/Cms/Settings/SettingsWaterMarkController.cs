using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsWaterMark")]
    public partial class SettingsWaterMarkController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;

        public SettingsWaterMarkController(IAuthManager authManager)
        {
            _authManager = authManager;
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

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var families = new InstalledFontCollection().Families.Select(x => x.Name);
            var imageUrl = await PageUtility.ParseNavigationUrlAsync(site, site.WaterMarkImagePath, true);

            return new GetResult
            {
                Site = site,
                Families = families,
                ImageUrl = imageUrl
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromBody] UploadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            if (request.File == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(request.File.FileName);

            var fileExtName = PathUtils.GetExtension(fileName).ToLower();
            var localDirectoryPath = await PathUtility.GetUploadDirectoryPathAsync(site, fileExtName);
            var localFileName = PathUtility.GetUploadFileName(site, fileName);
            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

            if (!FileUtils.IsImage(fileExtName))
            {
                return this.Error("请选择有效的图片文件上传");
            }

            request.File.CopyTo(new FileStream(localFilePath, FileMode.Create));
            var imageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, localFilePath, true);
            var virtualUrl = PageUtility.GetVirtualUrl(site, imageUrl);

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

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.IsWaterMark = request.IsWaterMark;
            site.WaterMarkPosition = request.WaterMarkPosition;
            site.WaterMarkTransparency = request.WaterMarkTransparency;
            site.WaterMarkMinWidth = request.WaterMarkMinWidth;
            site.WaterMarkMinHeight = request.WaterMarkMinHeight;
            site.IsImageWaterMark = request.IsImageWaterMark;
            site.WaterMarkFormatString = request.WaterMarkFormatString;
            site.WaterMarkFontName = request.WaterMarkFontName;
            site.WaterMarkFontSize = request.WaterMarkFontSize;
            site.WaterMarkImagePath = PageUtility.GetVirtualUrl(site, request.WaterMarkImagePath);

            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改图片水印设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
