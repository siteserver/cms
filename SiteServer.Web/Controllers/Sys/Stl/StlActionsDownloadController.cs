using System.Web;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsDownloadController : ApiController
    {
        [HttpGet]
        [Route(ActionsDownload.Route)]
        public void Main()
        {
            var isSuccess = false;
            try
            {
                var context = new RequestContext();

                if (!string.IsNullOrEmpty(context.GetQueryString("publishmentSystemID")) && !string.IsNullOrEmpty(context.GetQueryString("fileUrl")) && string.IsNullOrEmpty(context.GetQueryString("contentID")))
                {
                    var publishmentSystemId = context.GetQueryInt("publishmentSystemID");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(context.GetQueryString("fileUrl"));

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        isSuccess = true;
                        PageUtils.Redirect(fileUrl);
                    }
                    else
                    {
                        var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                        var filePath = PathUtility.MapPath(publishmentSystemInfo, fileUrl);
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
                            PageUtils.Redirect(PageUtility.ParseNavigationUrl(publishmentSystemInfo, fileUrl, false));
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
                else if (!string.IsNullOrEmpty(context.GetQueryString("publishmentSystemID")) && !string.IsNullOrEmpty(context.GetQueryString("channelID")) && !string.IsNullOrEmpty(context.GetQueryString("contentID")) && !string.IsNullOrEmpty(context.GetQueryString("fileUrl")))
                {
                    var publishmentSystemId = context.GetQueryInt("publishmentSystemID");
                    var channelId = context.GetQueryInt("channelID");
                    var contentId = context.GetQueryInt("contentID");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(context.GetQueryString("fileUrl"));
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);

                    if (!string.IsNullOrEmpty(contentInfo?.GetString(BackgroundContentAttribute.FileUrl)))
                    {
                        //string fileUrl = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
                        if (publishmentSystemInfo.Additional.IsCountDownload)
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
                            var filePath = PathUtility.MapPath(publishmentSystemInfo, fileUrl, true);
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
                                PageUtils.Redirect(PageUtility.ParseNavigationUrl(publishmentSystemInfo, fileUrl, false));
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
