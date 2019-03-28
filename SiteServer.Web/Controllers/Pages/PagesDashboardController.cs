using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages
{
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = new
                    {
                        Version = SystemManager.Version == PackageUtils.VersionDev ? "dev" : SystemManager.Version,
                        LastActivityDate = DateUtils.GetDateString(rest.AdminInfo.LastActivityDate, EDateFormatType.Chinese),
                        UpdateDate = DateUtils.GetDateString(ConfigManager.Instance.UpdateDate, EDateFormatType.Chinese)
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUnCheckedList)]
        public IHttpActionResult GetUnCheckedList()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var unCheckedList = new List<object>();

                if (rest.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    foreach(var siteInfo in SiteManager.GetSiteInfoList())
                    {
                        var count = ContentManager.GetCount(siteInfo, false);
                        if (count > 0)
                        {
                            unCheckedList.Add(new
                            {
                                Url = PageContentSearch.GetRedirectUrlCheck(siteInfo.Id),
                                siteInfo.SiteName,
                                Count = count
                            });
                        }
                    }
                }
                else if (rest.AdminPermissionsImpl.IsSystemAdministrator)
                {
                    foreach (var siteId in TranslateUtils.StringCollectionToIntList(rest.AdminInfo.SiteIdCollection))
                    {
                        var siteInfo = SiteManager.GetSiteInfo(siteId);
                        if (siteInfo == null) continue;

                        var count = ContentManager.GetCount(siteInfo, false);
                        if (count > 0)
                        {
                            unCheckedList.Add(new
                            {
                                Url = PageContentSearch.GetRedirectUrlCheck(siteInfo.Id),
                                siteInfo.SiteName,
                                Count = count
                            });
                        }
                    }
                }

                return Ok(new
                {
                    Value = unCheckedList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}