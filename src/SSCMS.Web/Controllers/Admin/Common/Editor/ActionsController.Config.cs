using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    public partial class ActionsController
    {
        [HttpGet, Route(RouteActionsConfig)]
        public async Task<ActionResult<ConfigResult>> GetConfig([FromQuery]SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            return new ConfigResult
            {
                ImageActionName = "uploadImage",
                ImageFieldName = "file",
                ImageMaxSize = site.ImageUploadTypeMaxSize * 1024,
                ImageAllowFiles = ListUtils.GetStringList(site.ImageUploadExtensions),
                ImageCompressEnable = false,
                ImageCompressBorder = 1600,
                ImageInsertAlign = "none",
                ImageUrlPrefix = "",
                ImagePathFormat = "",
                ScrawlActionName = "uploadScrawl",
                ScrawlFieldName = "file",
                ScrawlPathFormat = "",
                ScrawlMaxSize = site.ImageUploadTypeMaxSize * 1024,
                ScrawlUrlPrefix = "",
                ScrawlInsertAlign = "none",
                VideoActionName = "uploadVideo",
                VideoFieldName = "file",
                VideoUrlPrefix = "",
                VideoMaxSize = site.VideoUploadTypeMaxSize * 1024,
                VideoAllowFiles = ListUtils.GetStringList(site.VideoUploadExtensions),
                FileActionName = "uploadFile",
                FileFieldName = "file",
                FileUrlPrefix = "",
                FileMaxSize = site.FileUploadTypeMaxSize * 1024,
                FileAllowFiles = ListUtils.GetStringList($"{site.ImageUploadExtensions},{site.VideoUploadExtensions},{site.FileUploadExtensions}"),
                ImageManagerActionName = "listImage",
                ImageManagerListSize = 20,
                ImageManagerUrlPrefix = "",
                ImageManagerInsertAlign = "none",
                ImageManagerAllowFiles = ListUtils.GetStringList(site.ImageUploadExtensions),
                FileManagerActionName = "listFile",
                FileManagerListSize = 20,
                FileManagerUrlPrefix = "",
                FileManagerAllowFiles = ListUtils.GetStringList($"{site.ImageUploadExtensions},{site.VideoUploadExtensions},{site.FileUploadExtensions}")
            };
        }
    }
}
