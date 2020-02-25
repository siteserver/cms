using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sitesTables")]
    public partial class SitesTablesController : ControllerBase
    {
        private const string Route = "";
        private const string RouteTable = "{tableName}";
        private const string RouteTableActionsRemoveCache = "{tableName}/actions/removeCache";

        private readonly IAuthManager _authManager;

        public SitesTablesController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTables))
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

            return new GetResult
            {
                Value = nameDict.Keys.ToList(),
                NameDict = nameDict
            };
        }

        [HttpGet, Route(RouteTable)]
        public async Task<ActionResult<GetColumnsResult>> GetColumns(string tableName)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            var columns = await TableColumnManager.GetTableColumnInfoListAsync(tableName, ContentAttribute.MetadataAttributes.Value);

            return new GetColumnsResult
            {
                Columns = columns,
                Count = DataProvider.DatabaseRepository.GetCount(tableName)
            };
        }

        [HttpPost, Route(RouteTableActionsRemoveCache)]
        public async Task<ActionResult<GetColumnsResult>> RemoveCache(string tableName)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            var columns = await TableColumnManager.GetTableColumnInfoListAsync(tableName, ContentAttribute.MetadataAttributes.Value);

            return new GetColumnsResult
            {
                Columns = columns,
                Count = DataProvider.DatabaseRepository.GetCount(tableName)
            };
        }
    }
}
