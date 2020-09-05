using System.Collections.Generic;
using Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTablesController : ControllerBase
    {
        private const string Route = "settings/sitesTables";
        private const string RouteTable = "settings/sitesTables/{tableName}";
        private const string RouteTableActionsRemoveCache = "settings/sitesTables/{tableName}/actions/removeCache";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;

        public SitesTablesController(IAuthManager authManager, IDatabaseManager databaseManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public List<string> Value { get; set; }
            public Dictionary<string, List<string>> NameDict { get; set; }
        }

        public class GetColumnsResult
        {
            public List<TableColumn> Columns { get; set; }
            public int Count { get; set; }
        }
    }
}
