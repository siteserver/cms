using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    
    [RoutePrefix("pages/settings/sitesTables")]
    public class PagesSitesTablesController : ApiController
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTables))
                {
                    return Unauthorized();
                }

                var nameDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var site in await DataProvider.SiteRepository.GetSiteListAsync())
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTables))
                {
                    return Unauthorized();
                }

                var columns = await TableColumnManager.GetTableColumnInfoListAsync(tableName, ContentAttribute.MetadataAttributes.Value);

                return Ok(new
                {
                    Value = columns,
                    Count = DataProvider.DatabaseRepository.GetCount(tableName)
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTables))
                {
                    return Unauthorized();
                }

                var columns = await TableColumnManager.GetTableColumnInfoListAsync(tableName, ContentAttribute.MetadataAttributes.Value);

                return Ok(new
                {
                    Value = columns,
                    Count = DataProvider.DatabaseRepository.GetCount(tableName)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
