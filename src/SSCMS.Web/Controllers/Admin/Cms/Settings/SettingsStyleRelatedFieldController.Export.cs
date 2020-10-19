using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRelatedFieldListAsync(_pathManager, _databaseManager, request.SiteId);

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
