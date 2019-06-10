using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Packaging;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteUnCheckedList = "index/unCheckedList";

        private readonly IIdentity _identity;

        public IndexController(IIdentity identity)
        {
            _identity = identity;
        }

        [HttpGet(Route)]
        public ActionResult Get()
        {
            if (!_identity.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var adminInfo = AdminManager.GetAdminInfoByUserId(_identity.AdminId);

            return Ok(new
            {
                Value = new
                {
                    Version = SystemManager.ProductVersion == PackageUtils.VersionDev ? "dev" : SystemManager.ProductVersion,
                    LastActivityDate = DateUtils.GetDateString(adminInfo.LastActivityDate, EDateFormatType.Chinese),
                    UpdateDate = DateUtils.GetDateString(ConfigManager.Instance.UpdateDate, EDateFormatType.Chinese)
                }
            });
        }

        [HttpGet(RouteUnCheckedList)]
        public ActionResult GetUnCheckedList()
        {
            if (!_identity.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var unCheckedList = new List<object>();

            foreach (var siteInfo in SiteManager.GetSiteInfoList())
            {
                if (!_identity.AdminPermissions.IsSiteAdmin(siteInfo.Id)) continue;

                var count = ContentManager.GetCount(siteInfo, false);
                if (count > 0)
                {
                    unCheckedList.Add(new
                    {
                        //Url = PageContentSearch.GetRedirectUrlCheck(siteInfo.Id),
                        Url = PageUtils.UnclickedUrl,
                        siteInfo.SiteName,
                        Count = count
                    });
                }
            }

            return Ok(new
            {
                Value = unCheckedList
            });
        }
    }
}