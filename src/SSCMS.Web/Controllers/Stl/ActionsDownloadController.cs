using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsDownloadController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ActionsDownloadController(ISettingsManager settingsManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet]
        [Route(Constants.RouteActionsDownload)]
        public async Task<ActionResult> Get([FromQuery]GetRequest request)
        {
            try
            {
                if (request.SiteId.HasValue && !string.IsNullOrEmpty(request.FileUrl) && !request.ContentId.HasValue)
                {
                    var fileUrl = _settingsManager.Decrypt(request.FileUrl);

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        return Redirect(fileUrl);
                    }

                    var site = await _siteRepository.GetAsync(request.SiteId.Value);
                    var filePath = await _pathManager.MapPathAsync(site, fileUrl);
                    var fileType = FileUtils.GetType(PathUtils.GetExtension(filePath));
                    if (FileUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            return this.Download(filePath);
                        }
                    }
                    else
                    {
                        var redirectUrl = await _pathManager.ParseNavigationUrlAsync(site, fileUrl, false);
                        return Redirect(redirectUrl);
                    }
                }
                else if (!string.IsNullOrEmpty(request.FilePath))
                {
                    var filePath = _settingsManager.Decrypt(request.FilePath);
                    var fileType = FileUtils.GetType(PathUtils.GetExtension(filePath));
                    if (FileUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            return this.Download(filePath);
                        }
                    }
                    else
                    {
                        var fileUrl = _pathManager.GetRootUrlByPhysicalPath(filePath);
                        return Redirect(_pathManager.ParseNavigationUrl(fileUrl));
                    }
                }
                else if (request.SiteId.HasValue && request.ChannelId.HasValue && request.ContentId.HasValue && !string.IsNullOrEmpty(request.FileUrl))
                {
                    var fileUrl = _settingsManager.Decrypt(request.FileUrl);
                    var site = await _siteRepository.GetAsync(request.SiteId.Value);
                    var channel = await _channelRepository.GetAsync(request.ChannelId.Value);
                    var content = await _contentRepository.GetAsync(site, channel, request.ContentId.Value);

                    await _contentRepository.AddDownloadsAsync(_channelRepository.GetTableName(site, channel), request.ChannelId.Value, request.ContentId.Value);

                    if (!string.IsNullOrEmpty(content?.FileUrl))
                    {
                        if (PageUtils.IsProtocolUrl(fileUrl))
                        {
                            return Redirect(fileUrl);
                        }

                        var filePath = await _pathManager.MapPathAsync(site, fileUrl, true);
                        var fileType = FileUtils.GetType(PathUtils.GetExtension(filePath));
                        if (FileUtils.IsDownload(fileType))
                        {
                            if (FileUtils.IsFileExists(filePath))
                            {
                                return this.Download(filePath);
                            }
                        }
                        else
                        {
                            var redirectUrl = await _pathManager.ParseNavigationUrlAsync(site, fileUrl, false);
                            return Redirect(redirectUrl);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return this.Error("下载失败，不存在此文件！");
        }
    }
}
