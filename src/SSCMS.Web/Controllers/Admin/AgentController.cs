using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AgentController : ControllerBase
    {
        public const string Route = "agent";
        private const string RouteInstall = "agent/actions/install";
        private const string RouteSites = "agent/sites";
        private const string RouteSetDomains = "agent/actions/setDomains";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;

        public AgentController(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
        }

        public class AgentRequest
        {
            public string SecurityKey { get; set; }
        }

        public class InstallRequest : AgentRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string Themes { get; set; }
            public string Plugins { get; set; }
        }

        public class SitesResult
        {
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
        }

        public class SiteDomain
        {
            public int SiteId { get; set; }
            public string Domain { get; set; }
        }

        public class SetDomainsRequest : AgentRequest
        {
            public string HostDomain { get; set; }
            public List<SiteDomain> SiteDomains { get; set; }
        }
    }
}