using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsStyleContentController : ControllerBase
    {
        private const string Route = "cms/settings/settingsStyleContent";
        private const string RouteImport = "cms/settings/settingsStyleContent/actions/import";
        private const string RouteExport = "cms/settings/settingsStyleContent/actions/export";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public SettingsStyleContentController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public IEnumerable<Select<string>> InputTypes { get; set; }
            public string TableName { get; set; }
            public string RelatedIdentities { get; set; }
            public List<TableStyle> Styles { get; set; }
            public Cascade<int> Channels { get; set; }
        }

        public class DeleteRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public string AttributeName { get; set; }
        }

        public class ImportRequest : SiteRequest
        {
            public int ChannelId { get; set; }
        }

        public class DeleteResult
        {
            public List<TableStyle> Styles { get; set; }
        }
    }
}
