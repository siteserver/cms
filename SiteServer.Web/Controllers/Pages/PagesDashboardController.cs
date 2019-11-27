using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/dashboard")]
    public class PagesDashboardController : ApiController
    {
        private const string Route = "";
        private const string RouteUnCheckedList = "unCheckedList";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var lastActivityDate = request.Administrator.LastActivityDate ?? Constants.SqlMinValue;

                var config = await DataProvider.ConfigDao.GetAsync();

                return Ok(new
                {
                    Value = new
                    {
                        Version = SystemManager.ProductVersion == PackageUtils.VersionDev ? "dev" : SystemManager.ProductVersion,
                        LastActivityDate = DateUtils.GetDateString(lastActivityDate, EDateFormatType.Chinese),
                        UpdateDate = DateUtils.GetDateString(config.UpdateDate, EDateFormatType.Chinese),
                        config.AdminWelcomeHtml
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUnCheckedList)]
        public async Task<IHttpActionResult> GetUnCheckedList()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var checkingList = new List<object>();

                if (await request.AdminPermissionsImpl.IsSuperAdminAsync())
                {
                    foreach(var site in await DataProvider.SiteDao.GetSiteListAsync())
                    {
                        var count = await DataProvider.ContentDao.GetCountCheckingAsync(site);
                        if (count > 0)
                        {
                            checkingList.Add(new
                            {
                                Url = PageContentSearch.GetRedirectUrlCheck(site.Id),
                                site.SiteName,
                                Count = count
                            });
                        }
                    }
                }
                else if (await request.AdminPermissionsImpl.IsSiteAdminAsync())
                {
                    foreach (var siteId in request.Administrator.SiteIds)
                    {
                        var site = await DataProvider.SiteDao.GetAsync(siteId);
                        if (site == null) continue;

                        var count = await DataProvider.ContentDao.GetCountCheckingAsync(site);
                        if (count > 0)
                        {
                            checkingList.Add(new
                            {
                                Url = PageContentSearch.GetRedirectUrlCheck(site.Id),
                                site.SiteName,
                                Count = count
                            });
                        }
                    }
                }

                return Ok(new
                {
                    Value = checkingList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}