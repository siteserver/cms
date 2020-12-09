using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTablesController
    {
        [HttpPost, Route(RouteTableActionsRemoveCache)]
        public async Task<ActionResult<GetColumnsResult>> RemoveCache(string tableName)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTables))
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
