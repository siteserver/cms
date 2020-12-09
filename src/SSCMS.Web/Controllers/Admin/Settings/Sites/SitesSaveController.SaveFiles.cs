using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        [HttpPost, Route(RouteFiles)]
        public async Task<ActionResult<SaveFilesResult>> SaveFiles([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var caching = new CacheUtils(_cacheManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, site);
            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(request.TemplateDir);
            await exportObject.ExportFilesToSiteAsync(siteTemplatePath, request.IsAllFiles, request.CheckedDirectories, request.CheckedFiles, true);

            var channel = await _channelRepository.GetAsync(request.SiteId);
            channel.Children = await _channelRepository.GetChildrenAsync(request.SiteId, request.SiteId);

            return new SaveFilesResult
            {
                Channel = channel
            };
        }
    }
}
