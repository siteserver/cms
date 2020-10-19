using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils.Office;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            const string fileName = "users.csv";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            var excelObject = new ExcelObject(_databaseManager, _pathManager);
            await excelObject.CreateExcelFileForUsersAsync(filePath, null);

            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
