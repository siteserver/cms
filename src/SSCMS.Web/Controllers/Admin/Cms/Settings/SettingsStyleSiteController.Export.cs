using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleSiteController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleSite))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(_pathManager, _databaseManager, request.SiteId, _siteRepository.TableName, _tableStyleRepository.GetRelatedIdentities(request.SiteId));

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
