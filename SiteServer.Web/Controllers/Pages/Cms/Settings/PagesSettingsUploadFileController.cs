using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsUploadFile")]
    public partial class PagesSettingsUploadFileController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<Site>> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigUpload))
            {
                return Request.Unauthorized<ObjectResult<Site>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigUpload))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.FileUploadDirectoryName = request.FileUploadDirectoryName;
            site.FileUploadDateFormatString = request.FileUploadDateFormatString;
            site.IsFileUploadChangeFileName = request.IsFileUploadChangeFileName;
            site.FileUploadTypeCollection = request.FileUploadTypeCollection.Replace("|", ",");
            site.FileUploadTypeMaxSize = request.FileUploadTypeMaxSize;

            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改视频上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
