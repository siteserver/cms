using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    [AutoValidateAntiforgeryToken]
    public partial class EditorController : ControllerBase
    {
        private const string Route = "cms/editor";
        private const string RouteInsert = "cms/editor/actions/insert";
        private const string RouteUpdate = "cms/editor/actions/update";
        private const string RoutePreview = "cms/editor/actions/preview";
        private const string RouteCensor = "cms/editor/actions/censor";
        private const string RouteCensorAddWords = "cms/editor/actions/censorAddWords";
        private const string RouteSpell = "cms/editor/actions/spell";
        private const string RouteSpellAddWords = "cms/editor/actions/spellAddWords";
        private const string RouteTags = "cms/editor/actions/tags";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ICensorManager _censorManager;
        private readonly ISpellManager _spellManager;
        private readonly IMailManager _mailManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly ITranslateRepository _translateRepository;
        private readonly IScheduledTaskRepository _scheduledTaskRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public EditorController(ISettingsManager settingsManager, IAuthManager authManager, ICloudManager cloudManager, ICreateManager createManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ICensorManager censorManager, ISpellManager spellManager, IMailManager mailManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, ITableStyleRepository tableStyleRepository, IRelatedFieldItemRepository relatedFieldItemRepository, ITemplateRepository templateRepository, IContentCheckRepository contentCheckRepository, ITranslateRepository translateRepository, IScheduledTaskRepository scheduledTaskRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _cloudManager = cloudManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _censorManager = censorManager;
            _spellManager = spellManager;
            _mailManager = mailManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _templateRepository = templateRepository;
            _contentCheckRepository = contentCheckRepository;
            _translateRepository = translateRepository;
            _scheduledTaskRepository = scheduledTaskRepository;
            _errorLogRepository = errorLogRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class Settings
        {
            public bool IsCloudCensor { get; set; }
            public CensorSettings CensorSettings { get; set; }
            public bool IsCloudSpell { get; set; }
            public SpellSettings SpellSettings { get; set; }
            public bool IsCloudImages { get; set; }
            public CloudType CloudType { get; set; }
        }

        public class GetResult
        {
            public string CSRFToken { get; set; }
            public Content Content { get; set; }
            public Site Site { get; set; }
            public string SiteUrl { get; set; }
            public Channel Channel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<TableStyle> Styles { get; set; }
            public Dictionary<int, List<Cascade<int>>> RelatedFields { get; set; }
            public IEnumerable<Template> Templates { get; set; }
            public List<Select<int>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public LinkTo LinkTo { get; set; }
            public Cascade<int> Root { get; set; }
            public Settings Settings { get; set; }
            public List<Select<int>> BreadcrumbItems { get; set; }
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

        public class LinkTo
        {
            public List<int> ChannelIds { get; set; }

            public int ContentId { get; set; }

            public string ContentTitle { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
            public List<Translate> Translates { get; set; }
            public LinkTo LinkTo { get; set; }
            public bool IsScheduled { get; set; }
            public DateTime ScheduledDate { get; set; }
        }

        public class CensorRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public string Text { get; set; }
        }

        public class SpellRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public string Text { get; set; }
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

        public class CensorAddWordsRequest : ChannelRequest
        {
            public string Word { get; set; }
        }

        public class SpellAddWordsRequest : ChannelRequest
        {
            public string Word { get; set; }
        }
    }
}
