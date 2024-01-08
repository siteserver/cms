using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerImportController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerImport";
        private const string RouteUpload = "cms/contents/contentsLayerImport/actions/upload";

        private readonly ICacheManager _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IStorageManager _storageManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentsLayerImportController(ICacheManager cacheManager, IAuthManager authManager, IPathManager pathManager, IStorageManager storageManager, ICreateManager createManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ITableStyleRepository tableStyleRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _storageManager = storageManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public int Value { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
            public Options Options { get; set; }
        }

        public class UploadRequest : ChannelRequest
        {
            public string ImportType { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public List<string> Columns { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ImportType { get; set; }
            public int CheckedLevel { get; set; }
            public bool IsOverride { get; set; }
            public List<string> FileNames { get; set; }
            public List<string> FileUrls { get; set; }
            public List<string> Attributes { get; set; }
        }

        public class Options
        {
            public string ImportType { get; set; }
            public bool IsOverride { get; set; }
        }

        private static Options GetOptions(Site site)
        {
            return TranslateUtils.JsonDeserialize(site.Get<string>(nameof(ContentsLayerImportController)), new Options
            {
                ImportType = "zip",
                IsOverride = false,
            });
        }

        private static void SetOptions(Site site, Options options)
        {
            site.Set(nameof(ContentsLayerImportController), TranslateUtils.JsonSerialize(options));
        }
    }
}
