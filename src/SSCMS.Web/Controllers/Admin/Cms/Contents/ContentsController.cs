using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsController : ControllerBase
    {
        private const string RouteList = "cms/contents/contents/actions/list";
        private const string RouteTree = "cms/contents/contents/actions/tree";
        private const string RouteCreate = "cms/contents/contents/actions/create";
        private const string RouteColumns = "cms/contents/contents/actions/columns";
        private const string RouteWidth = "cms/contents/contents/actions/width";
        private const string RouteAll = "cms/contents/contents/actions/all";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;

        public ContentsController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
        }

        public class AllRequest : ChannelRequest
        {
            public bool IsAllContents { get; set; }
        }

        public class ColumnsRequest : ChannelRequest
        {
            public List<string> AttributeNames { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public string ChannelContentIds { get; set; }
        }

        public class ListRequest : ChannelRequest
        {
            public int Page { get; set; }
            public string SearchType { get; set; }
            public string SearchText { get; set; }
            public bool IsAdvanced { get; set; }
            public List<int> CheckedLevels { get; set; }
            public bool IsTop { get; set; }
            public bool IsRecommend { get; set; }
            public bool IsHot { get; set; }
            public bool IsColor { get; set; }
            public List<string> GroupNames { get; set; }
            public List<string> TagNames { get; set; }
        }

        public class Permissions
        {
            public bool IsAdd { get; set; }
            public bool IsDelete { get; set; }
            public bool IsEdit { get; set; }
            public bool IsArrange { get; set; }
            public bool IsTranslate { get; set; }
            public bool IsCheck { get; set; }
            public bool IsCreate { get; set; }
            public bool IsChannelEdit { get; set; }
        }

        public class ListResult
        {
            public List<Content> PageContents { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
            public ContentColumn TitleColumn { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public bool IsAllContents { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public Permissions Permissions { get; set; }
            public List<Menu> Menus { get; set; }
        }

        public class TreeRequest : SiteRequest
        {
            public bool Reload { get; set; }
        }

        public class TreeResult
        {
            public Cascade<int> Root { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
        }

        public class WidthRequest : ChannelRequest
        {
            public string PrevAttributeName { get; set; }
            public int PrevWidth { get; set; }
            public string NextAttributeName { get; set; }
            public int NextWidth { get; set; }
        }
    }
}
