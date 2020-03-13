using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Packaging;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route(Constants.ApiRoute)]
    public partial class DashboardController : ControllerBase
    {
        public const string Route = "dashboard";
        private const string RouteUnCheckedList = "dashboard/actions/unCheckedList";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public DashboardController(ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var admin = await _authManager.GetAdminAsync();
            var lastActivityDate = admin.LastActivityDate ?? Constants.SqlMinValue;
            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Version = _settingsManager.ProductVersion == PackageUtils.VersionDev ? "dev" : _settingsManager.ProductVersion,
                LastActivityDate = DateUtils.GetDateString(lastActivityDate, DateFormatType.Chinese),
                UpdateDate = DateUtils.GetDateString(config.UpdateDate, DateFormatType.Chinese),
                AdminWelcomeHtml = config.AdminWelcomeHtml
            };
        }

        [HttpGet, Route(RouteUnCheckedList)]
        public async Task<ActionResult<ObjectResult<List<Checking>>>> GetUnCheckedList()
        {
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var checkingList = new List<Checking>();

            if (await _authManager.IsSuperAdminAsync())
            {
                foreach (var site in await _siteRepository.GetSiteListAsync())
                {
                    var count = await _contentRepository.GetCountCheckingAsync(site);
                    if (count > 0)
                    {
                        checkingList.Add(new Checking
                        {
                            SiteName = site.SiteName,
                            Count = count
                        });
                    }
                }
            }
            else if (await _authManager.IsSiteAdminAsync())
            {
                var admin = await _authManager.GetAdminAsync();
                if (admin.SiteIds != null)
                {
                    foreach (var siteId in admin.SiteIds)
                    {
                        var site = await _siteRepository.GetAsync(siteId);
                        if (site == null) continue;

                        var count = await _contentRepository.GetCountCheckingAsync(site);
                        if (count > 0)
                        {
                            checkingList.Add(new Checking
                            {
                                SiteName = site.SiteName,
                                Count = count
                            });
                        }
                    }
                }
            }

            return new ObjectResult<List<Checking>>
            {
                Value = checkingList
            };
        }
    }
}