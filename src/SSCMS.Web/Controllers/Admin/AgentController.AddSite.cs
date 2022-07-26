using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteAddSite)]
        public async Task<ActionResult<AddSiteResult>> AddSite([FromBody] AddSiteRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            if (!request.Root)
            {
                if (_pathManager.IsSystemDirectory(request.SiteDir))
                {
                    return this.Error("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                var sitePath = await _pathManager.GetSitePathAsync(request.ParentId);
                var directories = DirectoryUtils.GetDirectoryNames(sitePath);
                if (ListUtils.ContainsIgnoreCase(directories, request.SiteDir))
                {
                    return this.Error("已存在相同的文件夹，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetSiteDirsAsync(request.ParentId);
                if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
                {
                    return this.Error("已存在相同的站点文件夹，请更改文件夹名称！");
                }
            }

            var channelInfo = new Channel();

            channelInfo.ChannelName = channelInfo.IndexName = "首页";
            channelInfo.ParentId = 0;
            channelInfo.ContentModelPluginId = string.Empty;

            var tableName = await _contentRepository.CreateNewContentTableAsync();
            var (siteId, errorMessage) = await _siteRepository.InsertSiteAsync(channelInfo, new Site
            {
                SiteName = request.SiteName,
                SiteType = Types.SiteTypes.Web,
                SiteDir = request.SiteDir,
                TableName = tableName,
                ParentId = request.ParentId,
                Root = request.Root
            }, 0);

            if (siteId == 0)
            {
                return this.Error(errorMessage);
            }

            var caching = new CacheUtils(_cacheManager);
            var site = await _siteRepository.GetAsync(siteId);

            caching.SetProcess(request.Guid, "任务初始化...");

            caching.SetProcess(request.Guid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

            var fileName = PageUtils.GetFileNameFromUrl(request.ThemeDownloadUrl);
            var filePath = _pathManager.GetSiteTemplatesPath(fileName);
            await HttpClientUtils.DownloadAsync(request.ThemeDownloadUrl, filePath);

            caching.SetProcess(request.Guid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            _pathManager.ExtractZip(filePath, directoryPath);

            caching.SetProcess(request.Guid, "模板压缩包解压成功，正在导入数据...");

            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
            await manager.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, true, true, 0, request.Guid);

            caching.SetProcess(request.Guid, "生成站点页面...");
            await _createManager.CreateByAllAsync(site.Id);

            caching.SetProcess(request.Guid, "清除系统缓存...");
            _cacheManager.Clear();

            var retVal = site.Clone<Site>();
            retVal.Set("LocalUrl", await _pathManager.GetSiteUrlAsync(site, true));
            retVal.Set("SiteUrl", await _pathManager.GetSiteUrlAsync(site, false));

            return new AddSiteResult
            {
                Site = retVal
            };
        }
    }
}
