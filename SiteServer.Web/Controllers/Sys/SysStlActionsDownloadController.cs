using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Fx;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsDownloadController : ApiController
    {
        [HttpGet]
        [Route(ApiRouteActionsDownload.Route)]
        public void Main()
        {
            try
            {
                var rest = new Rest(Request);

                if (!string.IsNullOrEmpty(rest.GetQueryString("siteId")) && !string.IsNullOrEmpty(rest.GetQueryString("fileUrl")) && string.IsNullOrEmpty(rest.GetQueryString("contentId")))
                {
                    var siteId = rest.GetQueryInt("siteId");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(rest.GetQueryString("fileUrl"));

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        FxUtils.Redirect(fileUrl);
                        return;
                    }

                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    var filePath = PathUtility.MapPath(siteInfo, fileUrl);
                    var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                    if (EFileSystemTypeUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            FxUtils.Download(HttpContext.Current.Response, filePath);
                            return;
                        }
                    }
                    else
                    {
                        FxUtils.Redirect(PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false));
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(rest.GetQueryString("filePath")))
                {
                    var filePath = TranslateUtils.DecryptStringBySecretKey(rest.GetQueryString("filePath"));
                    var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                    if (EFileSystemTypeUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            FxUtils.Download(HttpContext.Current.Response, filePath);
                            return;
                        }
                    }
                    else
                    {
                        var fileUrl = FxUtils.GetRootUrlByPhysicalPath(filePath);
                        FxUtils.Redirect(FxUtils.ParseNavigationUrl(fileUrl));
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(rest.GetQueryString("siteId")) && !string.IsNullOrEmpty(rest.GetQueryString("channelId")) && !string.IsNullOrEmpty(rest.GetQueryString("contentId")) && !string.IsNullOrEmpty(rest.GetQueryString("fileUrl")))
                {
                    var siteId = rest.GetQueryInt("siteId");
                    var channelId = rest.GetQueryInt("channelId");
                    var contentId = rest.GetQueryInt("contentId");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(rest.GetQueryString("fileUrl"));
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);

                    channelInfo.ContentRepository.AddDownloads(channelId, contentId);

                    if (!string.IsNullOrEmpty(contentInfo?.Get<string>(ContentAttribute.FileUrl)))
                    {
                        if (PageUtils.IsProtocolUrl(fileUrl))
                        {
                            FxUtils.Redirect(fileUrl);
                            return;
                        }

                        var filePath = PathUtility.MapPath(siteInfo, fileUrl, true);
                        var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                        if (EFileSystemTypeUtils.IsDownload(fileType))
                        {
                            if (FileUtils.IsFileExists(filePath))
                            {
                                FxUtils.Download(HttpContext.Current.Response, filePath);
                                return;
                            }
                        }
                        else
                        {
                            FxUtils.Redirect(PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false));
                            return;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            HttpContext.Current.Response.Write("下载失败，不存在此文件！");
        }
    }
}
