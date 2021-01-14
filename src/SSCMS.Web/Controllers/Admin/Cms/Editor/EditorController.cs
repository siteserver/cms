using System.Collections.Generic;
using Datory;
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
        private const string RouteCensor = "cms/editor/editor/actions/censor";
        private const string RouteTags = "cms/editor/editor/actions/tags";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ICensorManager _censorManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly ITranslateRepository _translateRepository;

        public EditorController(IAuthManager authManager, ICreateManager createManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ICensorManager censorManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository, IContentCheckRepository contentCheckRepository, ITranslateRepository translateRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _censorManager = censorManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _tableStyleRepository = tableStyleRepository;
            _templateRepository = templateRepository;
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
            public IEnumerable<Template> Templates { get; set; }
            public List<Select<int>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
            public bool IsCensorTextEnabled { get; set; }
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

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
            public List<Translate> Translates { get; set; }
        }

        public class CensorRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public Content Content { get; set; }
        }

        public class CensorSubmitResult
        {
            public bool Success { get; set; }
            public CensorResult TextResult { get; set; }
        }

        public class TagsRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public string Content { get; set; }
        }

        public class TagsResult
        {
            public List<string> Tags { get; set; }
        }
    }
}
