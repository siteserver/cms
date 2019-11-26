using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Site
{
    
    [RoutePrefix("pages/settings/siteTables")]
    public class PagesSiteTablesController : ApiController
    {
        private const string Route = "";
        private const string RouteTable = "{tableName}";
        private const string RouteTableActionsRemoveCache = "{tableName}/actions/removeCache";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetTables()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var nameDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var site in await DataProvider.SiteDao.GetSiteListAsync())
                {
                    if (nameDict.ContainsKey(site.TableName))
                    {
                        var list = nameDict[site.TableName];
                        list.Add(site.SiteName);
                    }
                    else
                    {
                        nameDict[site.TableName] = new List<string> { site.SiteName };
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
        public async Task<IHttpActionResult> GetColumns(string tableName)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Site))
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
        public async Task<IHttpActionResult> RemoveCache(string tableName)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Site))
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
