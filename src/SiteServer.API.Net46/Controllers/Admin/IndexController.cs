using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Admin
{
    [RoutePrefix("admin/index")]
    public class IndexController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUnCheckedList = "unCheckedList";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = GetRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

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
                var request = GetRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var unCheckedList = new List<object>();

                foreach (var siteInfo in SiteManager.GetSiteInfoList())
                {
                    if (!request.AdminPermissions.IsSiteAdmin(siteInfo.Id)) continue;

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