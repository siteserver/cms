using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
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
                var userCountListUnChecked = CheckManager.GetUserCountListUnChecked(request.AdminPermissions);

                if (userCountListUnChecked.Count > 0)
                {
                    var dict = new Dictionary<int, int>();

                    foreach (var pair in userCountListUnChecked)
                    {
                        var siteId = pair.Key;
                        var count = pair.Value;
                        if (dict.ContainsKey(siteId))
                        {
                            dict[siteId] = dict[siteId] + count;
                        }
                        else
                        {
                            dict[siteId] = count;
                        }
                    }

                    foreach (var siteId in dict.Keys)
                    {
                        var count = dict[siteId];
                        if (!SiteManager.IsExists(siteId)) continue;

                        unCheckedList.Add(new
                        {
                            Url = PageContentSearch.GetRedirectUrlCheck(siteId),
                            SiteManager.GetSiteInfo(siteId).SiteName,
                            Count = count
                        });
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