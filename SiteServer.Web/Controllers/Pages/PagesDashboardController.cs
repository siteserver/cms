using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/dashboard")]
    public partial class PagesDashboardController : ApiController
    {
        private const string Route = "";
        private const string RouteUnCheckedList = "actions/unCheckedList";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

            var lastActivityDate = auth.Administrator.LastActivityDate ?? Constants.SqlMinValue;
            var config = await DataProvider.ConfigRepository.GetAsync();

            return new GetResult
            {
                Version = SystemManager.ProductVersion == PackageUtils.VersionDev ? "dev" : SystemManager.ProductVersion,
                LastActivityDate = DateUtils.GetDateString(lastActivityDate, DateFormatType.Chinese),
                UpdateDate = DateUtils.GetDateString(config.UpdateDate, DateFormatType.Chinese),
                AdminWelcomeHtml = config.AdminWelcomeHtml
            };
        }

        [HttpGet, Route(RouteUnCheckedList)]
        public async Task<GenericResult<List<Checking>>> GetUnCheckedList()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GenericResult<List<Checking>>>();

            var checkingList = new List<Checking>();

            if (await auth.AdminPermissionsImpl.IsSuperAdminAsync())
            {
                foreach (var site in await DataProvider.SiteRepository.GetSiteListAsync())
                {
                    var count = await DataProvider.ContentRepository.GetCountCheckingAsync(site);
                    if (count > 0)
                    {
                        checkingList.Add(new Checking
                        {
                            Url = PageContentSearch.GetRedirectUrlCheck(site.Id),
                            SiteName = site.SiteName,
                            Count = count
                        });
                    }
                }
            }
            else if (await auth.AdminPermissionsImpl.IsSiteAdminAsync())
            {
                foreach (var siteId in auth.Administrator.SiteIds)
                {
                    var site = await DataProvider.SiteRepository.GetAsync(siteId);
                    if (site == null) continue;

                    var count = await DataProvider.ContentRepository.GetCountCheckingAsync(site);
                    if (count > 0)
                    {
                        checkingList.Add(new Checking
                        {
                            Url = PageContentSearch.GetRedirectUrlCheck(site.Id),
                            SiteName = site.SiteName,
                            Count = count
                        });
                    }
                }
            }

            return new GenericResult<List<Checking>>
            {
                Value = checkingList
            };
        }
    }
}