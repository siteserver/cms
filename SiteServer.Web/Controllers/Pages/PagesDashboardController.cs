using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages
{
    [OpenApiIgnore]
    [RoutePrefix("pages/dashboard")]
    public class PagesDashboardController : ApiController
    {
        private const string Route = "";
        private const string RouteUnCheckedList = "unCheckedList";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var lastActivityDate = request.Administrator.LastActivityDate ?? DateUtils.SqlMinValue;

                return Ok(new
                {
                    Value = new
                    {
                        Version = SystemManager.ProductVersion == PackageUtils.VersionDev ? "dev" : SystemManager.ProductVersion,
                        LastActivityDate = DateUtils.GetDateString(lastActivityDate, EDateFormatType.Chinese),
                        UpdateDate = DateUtils.GetDateString(ConfigManager.Instance.UpdateDate, EDateFormatType.Chinese),
                        ConfigManager.SystemConfigInfo.AdminWelcomeHtml
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var checkingList = new List<object>();

                if (request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    foreach(var site in await SiteManager.GetSiteListAsync())
                    {
                        var count = ContentManager.GetCountChecking(site);
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
                else if (request.AdminPermissionsImpl.IsSystemAdministrator)
                {
                    foreach (var siteId in TranslateUtils.StringCollectionToIntList(request.Administrator.SiteIdCollection))
                    {
                        var site = await SiteManager.GetSiteAsync(siteId);
                        if (site == null) continue;

                        var count = ContentManager.GetCountChecking(site);
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