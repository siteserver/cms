using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils.Office;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            const string fileName = "administrators.csv";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            var excelObject = new ExcelObject(_databaseManager, _pathManager);
            await excelObject.CreateExcelFileForAdministratorsAsync(filePath);
            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
