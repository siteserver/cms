using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home.Write
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ContentsLayerImportController : ControllerBase
    {
        private const string Route = "contentsLayerImport";
        private const string RouteUpload = "contentsLayerImport/actions/upload";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ContentsLayerImportController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class GetResult
        {
            public int CheckedLevel { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
        }

        public class UploadResult
        {
            public string FileName { set; get; }
            public long Length { set; get; }
            public int Ret { set; get; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ImportType { set; get; }
            public int CheckedLevel { set; get; }
            public bool IsOverride { set; get; }
            public List<string> FileNames { set; get; }
        }
    }
}
