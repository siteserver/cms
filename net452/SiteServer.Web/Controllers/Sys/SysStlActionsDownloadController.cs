using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Utils;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Fx;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
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
                var rest = Request.GetAuthenticatedRequest();

                if (!string.IsNullOrEmpty(Request.GetQueryString("siteId")) && !string.IsNullOrEmpty(Request.GetQueryString("fileUrl")) && string.IsNullOrEmpty(Request.GetQueryString("contentId")))
                {
                    var siteId = Request.GetQueryInt("siteId");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(Request.GetQueryString("fileUrl"));

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        WebPageUtils.Redirect(fileUrl);
                        return;
                    }

                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    var filePath = PathUtility.MapPath(siteInfo, fileUrl);
                    var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                    if (EFileSystemTypeUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            WebPageUtils.Download(HttpContext.Current.Response, filePath);
                            return;
                        }
                    }
                    else
                    {
                        WebPageUtils.Redirect(PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false));
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(Request.GetQueryString("filePath")))
                {
                    var filePath = TranslateUtils.DecryptStringBySecretKey(Request.GetQueryString("filePath"));
                    var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                    if (EFileSystemTypeUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            WebPageUtils.Download(HttpContext.Current.Response, filePath);
                            return;
                        }
                    }
                    else
                    {
                        var fileUrl = FxUtils.GetRootUrlByPhysicalPath(filePath);
                        WebPageUtils.Redirect(FxUtils.ParseNavigationUrl(fileUrl));
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(Request.GetQueryString("siteId")) && !string.IsNullOrEmpty(Request.GetQueryString("channelId")) && !string.IsNullOrEmpty(Request.GetQueryString("contentId")) && !string.IsNullOrEmpty(Request.GetQueryString("fileUrl")))
                {
                    var siteId = Request.GetQueryInt("siteId");
                    var channelId = Request.GetQueryInt("channelId");
                    var contentId = Request.GetQueryInt("contentId");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(Request.GetQueryString("fileUrl"));
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);

                    channelInfo.ContentRepository.AddDownloads(channelId, contentId);

                    if (!string.IsNullOrEmpty(contentInfo?.Get<string>(ContentAttribute.FileUrl)))
                    {
                        if (PageUtils.IsProtocolUrl(fileUrl))
                        {
                            WebPageUtils.Redirect(fileUrl);
                            return;
                        }

                        var filePath = PathUtility.MapPath(siteInfo, fileUrl, true);
                        var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                        if (EFileSystemTypeUtils.IsDownload(fileType))
                        {
                            if (FileUtils.IsFileExists(filePath))
                            {
                                WebPageUtils.Download(HttpContext.Current.Response, filePath);
                                return;
                            }
                        }
                        else
                        {
                            WebPageUtils.Redirect(PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false));
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
