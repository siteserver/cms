using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Api.Sys.Administrators;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Sys.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsSiteCheckListController : ApiController
    {
        [HttpGet, Route(ApiRouteSiteCheckList.Route)]
        public IHttpActionResult Main(string userName)
        {
            var request = new AuthRequest();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var list = new List<object>();
            var unCheckedList = CheckManager.GetUserCountListUnChecked(request.AdminPermissions);
            if (unCheckedList.Count <= 0) return Ok(list);

            var dict = new Dictionary<int, int>();

            foreach (var pair in unCheckedList)
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

                list.Add(new
                {
                    Url = PageContentSearch.GetRedirectUrlCheck(siteId),
                    SiteManager.GetSiteInfo(siteId).SiteName,
                    Count = count
                });
            }

            return Ok(list);
        }
    }
}
