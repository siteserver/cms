using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        [HttpPost, Route(RouteActionsData)]
        public async Task<ActionResult<BoolResult>> SaveData([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(request.TemplateDir);
            var siteContentDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.SiteContent);

            var caching = new CacheUtils(_cacheManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, site);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, request.IsSaveContents, request.IsSaveAllChannels, request.CheckedChannelIds);

            await SiteTemplateManager.ExportSiteToSiteTemplateAsync(_pathManager, _databaseManager, caching, site, request.TemplateDir);

            var siteTemplateInfo = new SiteTemplateInfo
            {
                SiteTemplateName = request.TemplateName,
                PicFileName = string.Empty,
                WebSiteUrl = request.WebSiteUrl,
                Description = request.Description
            };
            var xmlPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath,
                DirectoryUtils.SiteFiles.SiteTemplates.FileMetadata);
            XmlUtils.SaveAsXml(siteTemplateInfo, xmlPath);

            return new BoolResult
            {
                Value = true,
            };
        }
    }
}
