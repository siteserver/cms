using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTablesController : ControllerBase
    {
        private const string Route = "settings/sitesTables";
        private const string RouteTable = "settings/sitesTables/{tableName}";
        private const string RouteTableActionsRemoveCache = "settings/sitesTables/{tableName}/actions/removeCache";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;

        public SitesTablesController(IAuthManager authManager, IDatabaseManager databaseManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            var nameDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var site in await _siteRepository.GetSitesAsync())
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            var columns = await _databaseManager.GetTableColumnInfoListAsync(tableName, ColumnsManager.MetadataAttributes.Value);

            return new GetColumnsResult
            {
                Columns = columns,
                Count = _databaseManager.GetCount(tableName)
            };
        }

        [HttpPost, Route(RouteTableActionsRemoveCache)]
        public async Task<ActionResult<GetColumnsResult>> RemoveCache(string tableName)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            var columns = await _databaseManager.GetTableColumnInfoListAsync(tableName, ColumnsManager.MetadataAttributes.Value);

            return new GetColumnsResult
            {
                Columns = columns,
                Count = _databaseManager.GetCount(tableName)
            };
        }
    }
}
