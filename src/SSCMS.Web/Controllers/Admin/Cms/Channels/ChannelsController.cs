using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ChannelsController : ControllerBase
    {
        private const string Route = "cms/channels/channels";
        private const string RouteGet = "cms/channels/channels/{siteId:int}/{channelId:int}";
        private const string RouteUpdate = "cms/channels/channels/actions/update";
        private const string RouteDelete = "cms/channels/channels/actions/delete";
        private const string RouteAppend = "cms/channels/channels/actions/append";
        private const string RouteUpload = "cms/channels/channels/actions/upload";
        private const string RouteImport = "cms/channels/channels/actions/import";
        private const string RouteExport = "cms/channels/channels/actions/export";
        private const string RouteDrop = "cms/channels/channels/actions/drop";
        private const string RouteColumns = "cms/channels/channels/actions/columns";

        private readonly ICacheManager _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public ChannelsController(ICacheManager cacheManager, IAuthManager authManager, ICloudManager cloudManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IChannelGroupRepository channelGroupRepository, ITemplateRepository templateRepository, ITableStyleRepository tableStyleRepository, IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _cloudManager = cloudManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _channelGroupRepository = channelGroupRepository;
            _templateRepository = templateRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        public class ChannelColumn
        {
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
            public InputType InputType { get; set; }
            public bool IsList { get; set; }
        }

        public class ColumnsRequest : ChannelRequest
        {
            public List<string> AttributeNames { get; set; }
        }

        public class Settings
        {
            public bool IsCloudImages { get; set; }
        }

        public class LinkTo
        {
            public List<int> ChannelIds { get; set; }

            public int ContentId { get; set; }

            public string ContentTitle { get; set; }
        }

        public class ListResult
        {
            public Cascade<int> Channel { get; set; }
            public IEnumerable<string> IndexNames { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public int CommandsWidth { get; set; }
            public bool IsTemplateEditable { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public IEnumerable<Select<string>> TaxisTypes { get; set; }
            public string SiteUrl { get; set; }
            public Settings Settings { get; set; }
            public List<Menu> ChannelMenus { get; set; }
            public List<Menu> ChannelsMenus { get; set; }
        }

        public class GetResult
        {
            public Entity Entity { get; set; }
            public IEnumerable<TableStyle> Styles { get; set; }
            public Dictionary<int, List<Dto.Cascade<int>>> RelatedFields { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public LinkTo LinkTo { get; set; }
        }

        public class ImportRequest : ChannelRequest
        {
            public string FileName { get; set; }
            public bool IsOverride { get; set; }
        }

        public class DropRequest : SiteRequest
        {
            public int SourceId { get; set; }
            public int TargetId { get; set; }
            public string DropType { get; set; }
        }

        public class ChannelIdsRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int ChannelId { get; set; }
            public string ChannelName { get; set; }
            public bool DeleteFiles { get; set; }
        }

        public class AppendRequest : SiteRequest
        {
            public int ParentId { get; set; }
            public int ChannelTemplateId { get; set; }
            public int ContentTemplateId { get; set; }
            public bool IsParentTemplates { get; set; }
            public bool IsIndexName { get; set; }
            public string Channels { get; set; }
        }

        public class UpdateRequest : Channel
        {
            public List<int> ChannelIds { get; set; }
            public int ContentId { get; set; }
        }

        private async Task<List<TableStyle>> GetStylesAsync(Channel channel)
        {
            var styles = new List<TableStyle>
            {
                new TableStyle()
                {
                    AttributeName = nameof(Channel.ImageUrl),
                    DisplayName = "栏目图片",
                    InputType = InputType.Image
                },
                new TableStyle()
                {
                    AttributeName = nameof(Channel.Content),
                    DisplayName = "栏目正文",
                    InputType = InputType.TextEditor
                }
            };
            var tableStyles = await _tableStyleRepository.GetChannelStylesAsync(channel);
            styles.AddRange(tableStyles);

            return styles;
        }
    }
}
