using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpPost, Route(RouteUnZip)]
        public async Task<ActionResult<ListResult>> UnZip([FromBody] UnZipRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var fileNameToUnZip = request.FileName;

            var directoryPathToUnZip = _pathManager.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileNameToUnZip));
            var zipFilePath = _pathManager.GetSiteTemplatesPath(fileNameToUnZip);

            _pathManager.ExtractZip(zipFilePath, directoryPathToUnZip);

            return await GetListResultAsync();
        }
    }
}
