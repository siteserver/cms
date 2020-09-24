using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils.Office;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpGet, Route(RouteExport)]
        public async Task<ActionResult> Export()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            const string fileName = "users.csv";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            var excelObject = new ExcelObject(_databaseManager, _pathManager);
            await excelObject.CreateExcelFileForUsersAsync(filePath, null);

            return this.Download(filePath);
        }
    }
}
