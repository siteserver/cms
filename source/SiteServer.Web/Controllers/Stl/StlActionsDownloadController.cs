using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Stl
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
                var body = new RequestBody();

                if (!string.IsNullOrEmpty(body.GetQueryString("publishmentSystemID")) && !string.IsNullOrEmpty(body.GetQueryString("fileUrl")) && string.IsNullOrEmpty(body.GetQueryString("contentID")))
                {
                    var publishmentSystemId = body.GetQueryInt("publishmentSystemID");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(body.GetQueryString("fileUrl"));

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
                            PageUtils.Redirect(PageUtility.ParseNavigationUrl(publishmentSystemInfo, fileUrl));
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(body.GetQueryString("filePath")))
                {
                    var filePath = TranslateUtils.DecryptStringBySecretKey(body.GetQueryString("filePath"));
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
                else if (!string.IsNullOrEmpty(body.GetQueryString("publishmentSystemID")) && !string.IsNullOrEmpty(body.GetQueryString("channelID")) && !string.IsNullOrEmpty(body.GetQueryString("contentID")) && !string.IsNullOrEmpty(body.GetQueryString("fileUrl")))
                {
                    var publishmentSystemId = body.GetQueryInt("publishmentSystemID");
                    var channelId = body.GetQueryInt("channelID");
                    var contentId = body.GetQueryInt("contentID");
                    var fileUrl = TranslateUtils.DecryptStringBySecretKey(body.GetQueryString("fileUrl"));
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                    var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

                    if (!string.IsNullOrEmpty(contentInfo?.GetExtendedAttribute(BackgroundContentAttribute.FileUrl)))
                    {
                        //string fileUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
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
                                PageUtils.Redirect(PageUtility.ParseNavigationUrl(publishmentSystemInfo, fileUrl));
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
