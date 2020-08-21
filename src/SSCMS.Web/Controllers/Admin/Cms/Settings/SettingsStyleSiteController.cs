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
    public partial class SettingsStyleSiteController : ControllerBase
    {
        private const string Route = "cms/settings/settingsStyleSite";
        private const string RouteImport = "cms/settings/settingsStyleSite/actions/import";
        private const string RouteExport = "cms/settings/settingsStyleSite/actions/export";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public SettingsStyleSiteController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public IEnumerable<Select<string>> InputTypes { get; set; }
            public string TableName { get; set; }
            public string RelatedIdentities { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class DeleteRequest
        {
            public int SiteId { get; set; }
            public string AttributeName { get; set; }
        }

        public class DeleteResult
        {
            public List<TableStyle> Styles { get; set; }
        }
    }
}
