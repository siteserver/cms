using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/siteTables")]
    public class PagesSiteTablesController : ApiController
    {
        private const string Route = "";
        private const string RouteTable = "{tableName}";
        private const string RouteTableActionsRemoveCache = "{tableName}/actions/removeCache";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetTables()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
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

        [HttpGet, Route(RouteTable)]
        public IHttpActionResult GetColumns(string tableName)
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var columns = TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.MetadataAttributes.Value);

                return Ok(new
                {
                    Value = columns,
                    Count = DataProvider.DatabaseDao.GetCount(tableName)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteTableActionsRemoveCache)]
        public IHttpActionResult RemoveCache(string tableName)
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                TableColumnManager.ClearCache();

                var columns = TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.MetadataAttributes.Value);

                return Ok(new
                {
                    Value = columns,
                    Count = DataProvider.DatabaseDao.GetCount(tableName)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
