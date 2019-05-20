using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Admin
{
    [Route("api/admin/index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly Request request;

        public IndexController(Request req)
        {
            request = req;
        }

        private const string Route = "";
        private const string RouteUnCheckedList = "unCheckedList";

        [HttpGet(Route)]
        public ActionResult Get()
        {
            try
            {
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
                return BadRequest(ex);
            }
        }

        [HttpGet, Route(RouteUnCheckedList)]
        public ActionResult GetUnCheckedList()
        {
            try
            {
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
                            //Url = PageContentSearch.GetRedirectUrlCheck(siteInfo.Id),
                            Url = string.Empty,
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
                return BadRequest(ex);
            }
        }
    }
}