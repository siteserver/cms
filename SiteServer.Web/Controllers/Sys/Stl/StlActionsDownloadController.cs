using System.Web;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsDownloadController : ApiController
    {
        [HttpGet]
        [Route(ApiRouteActionsDownload.Route)]
        public void Main()
        {
            var isSuccess = false;
            try
            {
                var context = new RequestContext();

                if (!string.IsNullOrEmpty(context.GetQueryString("siteID")) && !string.IsNullOrEmpty(context.GetQueryString("fileUrl")) && string.IsNullOrEmpty(context.GetQueryString("contentID")))
                {
                    var siteId = context.GetQueryInt("siteID");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(context.GetQueryString("fileUrl"));

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        isSuccess = true;
                        PageUtils.Redirect(fileUrl);
                    }
                    else
                    {
                        var siteInfo = SiteManager.GetSiteInfo(siteId);
                        var filePath = PathUtility.MapPath(siteInfo, fileUrl);
                        var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                        if (EFileSystemTypeUtils.IsDownload(fileType))
                        {
                            if (FileUtils.IsFileExists(filePath))
                            {
                                isSuccess = true;
                                PageUtils.Download(HttpContext.Current.Response, filePath);
                            }
                        }
                        else
                        {
                            isSuccess = true;
                            PageUtils.Redirect(PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false));
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(context.GetQueryString("filePath")))
                {
                    var filePath = TranslateUtils.DecryptStringBySecretKey(context.GetQueryString("filePath"));
                    var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                    if (EFileSystemTypeUtils.IsDownload(fileType))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            isSuccess = true;
                            PageUtils.Download(HttpContext.Current.Response, filePath);
                        }
                    }
                    else
                    {
                        isSuccess = true;
                        var fileUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);
                        PageUtils.Redirect(PageUtils.ParseNavigationUrl(fileUrl));
                    }
                }
                else if (!string.IsNullOrEmpty(context.GetQueryString("siteID")) && !string.IsNullOrEmpty(context.GetQueryString("channelID")) && !string.IsNullOrEmpty(context.GetQueryString("contentID")) && !string.IsNullOrEmpty(context.GetQueryString("fileUrl")))
                {
                    var siteId = context.GetQueryInt("siteID");
                    var channelId = context.GetQueryInt("channelID");
                    var contentId = context.GetQueryInt("contentID");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(context.GetQueryString("fileUrl"));
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);

                    if (!string.IsNullOrEmpty(contentInfo?.GetString(BackgroundContentAttribute.FileUrl)))
                    {
                        //string fileUrl = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
                        if (siteInfo.Additional.IsCountDownload)
                        {
                            CountManager.AddCount(tableName, contentId.ToString(), ECountType.Download);
                        }

                        if (PageUtils.IsProtocolUrl(fileUrl))
                        {
                            isSuccess = true;
                            PageUtils.Redirect(fileUrl);
                        }
                        else
                        {
                            var filePath = PathUtility.MapPath(siteInfo, fileUrl, true);
                            var fileType = EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath));
                            if (EFileSystemTypeUtils.IsDownload(fileType))
                            {
                                if (FileUtils.IsFileExists(filePath))
                                {
                                    isSuccess = true;
                                    PageUtils.Download(HttpContext.Current.Response, filePath);
                                }
                            }
                            else
                            {
                                isSuccess = true;
                                PageUtils.Redirect(PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false));
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
            if (!isSuccess)
            {
                HttpContext.Current.Response.Write("下载失败，不存在此文件！");
            }
        }
    }
}
