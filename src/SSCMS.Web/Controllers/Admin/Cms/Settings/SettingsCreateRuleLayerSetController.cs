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
    public partial class SettingsCreateRuleLayerSetController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCreateRuleLayerSet";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsCreateRuleLayerSetController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public bool IsChannel { get; set; }
        }

        public class ChannelResult
        {
            public Channel Channel { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public IEnumerable<Select<string>> TaxisTypes { get; set; }
        }
    }
}