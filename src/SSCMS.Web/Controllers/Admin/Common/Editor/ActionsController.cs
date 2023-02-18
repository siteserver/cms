using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ActionsController : ControllerBase
    {
        private const string RouteActionsConfig = "common/editor/actions/config";
        private const string RouteActionsUploadImage = "common/editor/actions/uploadImage";
        private const string RouteActionsListImage = "common/editor/actions/listImage";
        private const string RouteActionsUploadFile = "common/editor/actions/uploadFile";
        private const string RouteActionsListFile = "common/editor/actions/listFile";
        private const string RouteActionsUploadVideo = "common/editor/actions/uploadVideo";
        private const string RouteActionsUploadScrawl = "common/editor/actions/uploadScrawl";

        private readonly IPathManager _pathManager;
        private readonly IStorageManager _storageManager;
        private readonly IVodManager _vodManager;
        private readonly ISiteRepository _siteRepository;

        public ActionsController(IPathManager pathManager, IStorageManager storageManager, IVodManager vodManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _storageManager = storageManager;
            _vodManager = vodManager;
            _siteRepository = siteRepository;
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

        public class ListFileRequest : SiteRequest
        {
            public int Start { get; set; }
            public int Size { get; set; }
        }

        public class FileResult
        {
            public string Url { get; set; }
        }

        public class ListFileResult
        {
            public string State { get; set; }
            public int Start { get; set; }
            public int Size { get; set; }
            public int Total { get; set; }
            public IEnumerable<FileResult> List { get; set; }
        }

        public class ListImageRequest : SiteRequest
        {
            public int Start { get; set; }
            public int Size { get; set; }
        }

        public class ImageResult
        {
            public string Url { get; set; }
        }

        public class ListImageResult
        {
            public string State { get; set; }
            public int Start { get; set; }
            public int Size { get; set; }
            public int Total { get; set; }
            public IEnumerable<ImageResult> List { get; set; }
        }

        public class UploadScrawlRequest
        {
            public string File { get; set; }
        }

        public class UploadScrawlResult
        {
            public string State { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Original { get; set; }
            public string Error { get; set; }
        }

        public class UploadVideoResult
        {
            public string State { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Original { get; set; }
            public string Error { get; set; }
        }
    }
}