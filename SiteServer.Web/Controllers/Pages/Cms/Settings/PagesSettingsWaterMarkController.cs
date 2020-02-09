using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsWaterMark")]
    public partial class PagesSettingsWaterMarkController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<GetResult> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var families = new InstalledFontCollection().Families.Select(x => x.Name);
            var imageUrl = PageUtility.ParseNavigationUrl(site, site.WaterMarkImagePath, true);

            return new GetResult
            {
                Site = site,
                Families = families,
                ImageUrl = imageUrl
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的图片文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            var filePath = file.FileName;
            var fileExtName = PathUtils.GetExtension(filePath).ToLower();
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, fileExtName);
            var localFileName = PathUtility.GetUploadFileName(site, filePath);
            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

            if (!EFileSystemTypeUtils.IsImage(fileExtName))
            {
                return Request.BadRequest<UploadResult>("请选择有效的图片文件上传");
            }

            file.SaveAs(localFilePath);
            var imageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, localFilePath, true);
            var virtualUrl = PageUtility.GetVirtualUrl(site, imageUrl);

            return new UploadResult
            {
                ImageUrl = imageUrl,
                VirtualUrl = virtualUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigWaterMark))
            {
                return Request.Unauthorized<BoolResult>();
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
