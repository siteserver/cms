using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<List<int>>> Import([FromBody] ImportRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ChannelPermissions.Add))
            {
                return Unauthorized();
            }

            try
            {
                var site = await _siteRepository.GetAsync(request.SiteId);
                var filePath = _pathManager.GetTemporaryFilesPath(request.FileName);
                var adminId = _authManager.AdminId;
                var caching = new CacheUtils(_cacheManager);

                var importObject = new ImportObject(_pathManager, _databaseManager, caching, site, adminId);
                await importObject.ImportChannelsAndContentsByZipFileAsync(request.ChannelId, filePath,
                    request.IsOverride, null);

                await _authManager.AddSiteLogAsync(request.SiteId, "导入栏目");
            }
            catch
            {
                return this.Error("压缩包格式不正确，请上传正确的栏目压缩包");
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }
    }
}
