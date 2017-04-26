using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Controllers.Administrators;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.User;

namespace SiteServer.API.Controllers.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsSiteCheckListController : ApiController
    {
        [HttpGet, Route(SiteCheckList.Route)]
        public IHttpActionResult Main(string userName)
        {
            var body = new RequestBody();

            if (!body.IsAdministratorLoggin)
            {
                return Unauthorized();
            }

            var list = new List<object>();
            var unCheckedList = CheckManager.GetUserCountListUnChecked(body.AdministratorName);
            if (unCheckedList.Count <= 0) return Ok(list);

            var dict = new Dictionary<int, int>();

            foreach (var pair in unCheckedList)
            {
                var publishmentSystemId = pair.Key;
                var count = pair.Value;
                if (dict.ContainsKey(publishmentSystemId))
                {
                    dict[publishmentSystemId] = dict[publishmentSystemId] + count;
                }
                else
                {
                    dict[publishmentSystemId] = count;
                }
            }

            foreach (var publishmentSystemId in dict.Keys)
            {
                var count = dict[publishmentSystemId];
                if (!PublishmentSystemManager.IsExists(publishmentSystemId)) continue;

                list.Add(new
                {
                    Url = PageContentCheck.GetRedirectUrl(publishmentSystemId),
                    SiteName = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId).PublishmentSystemName,
                    Count = count
                });
            }

            return Ok(list);
        }
    }
}
