using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpPost, Route(RouteZip)]
        public async Task<ActionResult<StringResult>> Zip([FromBody] ZipRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var directoryName = PathUtils.RemoveParentPath(request.DirectoryName);
            var fileName = directoryName + ".zip";
            var filePath = _pathManager.GetSiteTemplatesPath(fileName);
            var directoryPath = _pathManager.GetSiteTemplatesPath(directoryName);

            FileUtils.DeleteFileIfExists(filePath);

            _pathManager.CreateZip(filePath, directoryPath);

            return new StringResult
            {
                Value = _pathManager.GetSiteTemplatesUrl(fileName)
            };
        }
    }
}
