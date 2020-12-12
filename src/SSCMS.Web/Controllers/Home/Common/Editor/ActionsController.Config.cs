using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common.Editor
{
    public partial class ActionsController
    {
        [HttpGet, Route(RouteActionsConfig)]
        public async Task<ActionResult<ConfigResult>> GetConfig([FromQuery]SiteRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

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

        public class ConfigResult
        {
            public string ImageActionName { get; set; }
            public string ImageFieldName { get; set; }
            public long ImageMaxSize { get; set; }
            public List<string> ImageAllowFiles { get; set; }
            public bool ImageCompressEnable { get; set; }
            public int ImageCompressBorder { get; set; }
            public string ImageInsertAlign { get; set; }
            public string ImageUrlPrefix { get; set; }
            public string ImagePathFormat { get; set; }
            public string ScrawlActionName { get; set; }
            public string ScrawlFieldName { get; set; }
            public string ScrawlPathFormat { get; set; }
            public long ScrawlMaxSize { get; set; }
            public string ScrawlUrlPrefix { get; set; }
            public string ScrawlInsertAlign { get; set; }
            public string SnapscreenActionName { get; set; }
            public string SnapscreenPathFormat { get; set; }
            public string SnapscreenUrlPrefix { get; set; }
            public string SnapscreenInsertAlign { get; set; }
            public List<string> CatcherLocalDomain { get; set; }
            public string CatcherActionName { get; set; }
            public string CatcherFieldName { get; set; }
            public string CatcherPathFormat { get; set; }
            public string CatcherUrlPrefix { get; set; }
            public long CatcherMaxSize { get; set; }
            public List<string> CatcherAllowFiles { get; set; }
            public string VideoActionName { get; set; }
            public string VideoFieldName { get; set; }
            public string VideoPathFormat { get; set; }
            public string VideoUrlPrefix { get; set; }
            public long VideoMaxSize { get; set; }
            public List<string> VideoAllowFiles { get; set; }
            public string FileActionName { get; set; }
            public string FileFieldName { get; set; }
            public string FilePathFormat { get; set; }
            public string FileUrlPrefix { get; set; }
            public long FileMaxSize { get; set; }
            public List<string> FileAllowFiles { get; set; }
            public string ImageManagerActionName { get; set; }
            public string ImageManagerListPath { get; set; }
            public int ImageManagerListSize { get; set; }
            public string ImageManagerUrlPrefix { get; set; }
            public string ImageManagerInsertAlign { get; set; }
            public List<string> ImageManagerAllowFiles { get; set; }
            public string FileManagerActionName { get; set; }
            public string FileManagerListPath { get; set; }
            public string FileManagerUrlPrefix { get; set; }
            public int FileManagerListSize { get; set; }
            public List<string> FileManagerAllowFiles { get; set; }
        }
    }
}
