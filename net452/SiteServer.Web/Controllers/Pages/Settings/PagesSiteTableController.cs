using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/siteTables")]
    public class PagesSiteTablesController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsGetColumns = "actions/getColumns";
        private const string RouteActionsRemoveCache = "actions/removeCache";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetTables()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var nameDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var siteInfo in SiteManager.GetSiteInfoList())
                {
                    if (nameDict.ContainsKey(siteInfo.TableName))
                    {
                        var list = nameDict[siteInfo.TableName];
                        list.Add(siteInfo.SiteName);
                    }
                    else
                    {
                        nameDict[siteInfo.TableName] = new List<string> { siteInfo.SiteName };
                    }
                }

                return Ok(new
                {
                    Value = nameDict.Keys,
                    nameDict
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsGetColumns)]
        public IHttpActionResult GetColumns()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var tableName = Request.GetPostString("tableName");

                var columns = TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.MetadataAttributes.Value);

                return Ok(new
                {
                    Value = columns,
                    Count = DataProvider.DatabaseApi.GetCount(tableName)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsRemoveCache)]
        public IHttpActionResult RemoveCache()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var tableName = Request.GetPostString("tableName");

                TableColumnManager.ClearCache();

                var columns = TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.MetadataAttributes.Value);

                return Ok(new
                {
                    Value = columns,
                    Count = DataProvider.DatabaseApi.GetCount(tableName)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
