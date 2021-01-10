using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdminLayerViewController : ControllerBase
    {
        private const string Route = "common/adminLayerView";

        private readonly IHttpContextAccessor _context;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;

        public AdminLayerViewController(IHttpContextAccessor context, ICacheManager cacheManager, ISettingsManager settingsManager, IDatabaseManager databaseManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository)
        {
            _context = context;
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
        }

        public class GetRequest
        {
            public int AdminId { get; set; }
            public string UserName { get; set; }
        }

        public class GetResult
        {
            public Administrator Administrator { get; set; }
            public string Level { get; set; }
            public string SiteNames { get; set; }
            public string RoleNames { get; set; }
        }
    }
}
