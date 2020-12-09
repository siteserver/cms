using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpPost, Route(RouteDownload)]
        public async Task<ActionResult<StringResult>> Download([FromBody] SpecialIdRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialInfo = await _specialRepository.GetSpecialAsync(request.SiteId, request.SpecialId);

            var directoryPath = await _pathManager.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
            var srcDirectoryPath = _pathManager.GetSpecialSrcDirectoryPath(directoryPath);
            var zipFilePath = _pathManager.GetSpecialZipFilePath(specialInfo.Title, directoryPath);

            FileUtils.DeleteFileIfExists(zipFilePath);
            _pathManager.CreateZip(zipFilePath, srcDirectoryPath);
            var url = await _pathManager.GetSpecialZipFileUrlAsync(site, specialInfo);

            return new StringResult
            {
                Value = url
            };
        }
    }
}