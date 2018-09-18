using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("api/pages/dashboard")]
    public class PageDashboardController : ApiController
    {
        private const string Route = "";
        private const string RouteUnCheckedList = "unCheckedList";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = new
                    {
                        Version = SystemManager.Version == PackageUtils.VersionDev ? "dev" : SystemManager.Version,
                        LastActivityDate = DateUtils.GetDateString(request.AdminInfo.LastActivityDate, EDateFormatType.Chinese),
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
                var request = new AuthRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var unCheckedList = new List<object>();

                if (request.AdminPermissions.IsConsoleAdministrator)
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
                else if (request.AdminPermissions.IsSystemAdministrator)
                {
                    foreach (var siteId in TranslateUtils.StringCollectionToIntList(request.AdminInfo.SiteIdCollection))
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