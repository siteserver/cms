using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsDownloadController : ControllerBase
    {
        [HttpGet]
        [Route(ApiRouteActionsDownload.Route)]
        public async Task<ActionResult> Get([FromQuery]GetRequest request)
        {
            try
            {
                if (request.SiteId.HasValue && !string.IsNullOrEmpty(request.FileUrl) && !request.ContentId.HasValue)
                {
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(request.FileUrl, WebConfigUtils.SecretKey);

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        return Redirect(fileUrl);
                    }

                    var site = await DataProvider.SiteRepository.GetAsync(request.SiteId.Value);
                    var filePath = await PathUtility.MapPathAsync(site, fileUrl);
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
                        var redirectUrl = await PageUtility.ParseNavigationUrlAsync(site, fileUrl, false);
                        return Redirect(redirectUrl);
                    }
                }
                else if (!string.IsNullOrEmpty(request.FilePath))
                {
                    var filePath = TranslateUtils.DecryptStringBySecretKey(request.FilePath, WebConfigUtils.SecretKey);
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
                        var fileUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);
                        return Redirect(PageUtils.ParseNavigationUrl(fileUrl));
                    }
                }
                else if (request.SiteId.HasValue && request.ChannelId.HasValue && request.ContentId.HasValue && !string.IsNullOrEmpty(request.FileUrl))
                {
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(request.FileUrl, WebConfigUtils.SecretKey);
                    var site = await DataProvider.SiteRepository.GetAsync(request.SiteId.Value);
                    var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId.Value);
                    var content = await DataProvider.ContentRepository.GetAsync(site, channel, request.ContentId.Value);

                    await DataProvider.ContentRepository.AddDownloadsAsync(await DataProvider.ChannelRepository.GetTableNameAsync(site, channel), request.ChannelId.Value, request.ContentId.Value);

                    if (!string.IsNullOrEmpty(content?.Get<string>(ContentAttribute.FileUrl)))
                    {
                        if (PageUtils.IsProtocolUrl(fileUrl))
                        {
                            return Redirect(fileUrl);
                        }

                        var filePath = await PathUtility.MapPathAsync(site, fileUrl, true);
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
                            var redirectUrl = await PageUtility.ParseNavigationUrlAsync(site, fileUrl, false);
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
