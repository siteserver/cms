using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorController : ControllerBase
    {
        private const string Route = "cms/editor/editor";
        private const string RoutePreview = "cms/editor/editor/actions/preview";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly ITranslateRepository _translateRepository;

        public EditorController(IAuthManager authManager, ICreateManager createManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, ITableStyleRepository tableStyleRepository, IContentCheckRepository contentCheckRepository, ITranslateRepository translateRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _tableStyleRepository = tableStyleRepository;
            _contentCheckRepository = contentCheckRepository;
            _translateRepository = translateRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public Content Content { get; set; }
            public Site Site { get; set; }
            public string SiteUrl { get; set; }
            public Channel Channel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public List<Select<int>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
        }

        public class PreviewRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
        }

        public class PreviewResult
        {
            public string Url { get; set; }
        }

        public class SaveRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
            public List<Translate> Translates { get; set; }
        }
    }
}
