using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] ChannelIdsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var caching = new CacheUtils(_cacheManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, site);
            var fileName = await exportObject.ExportChannelsAsync(request.ChannelIds);
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var url = _pathManager.GetDownloadApiUrl(filePath);

            return new StringResult
            {
                Value = url
            };
        }
    }
}
