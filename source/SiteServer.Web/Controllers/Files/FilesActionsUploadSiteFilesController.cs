using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Files;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Files
{
    [RoutePrefix("api")]
    public class FilesActionsUploadSiteFilesController : ApiController
    {
        [HttpPost, Route(ActionsUploadSiteFiles.Route)]
        public HttpResponseMessage Main()
        {
            var body = new RequestBody();

            var siteId = TranslateUtils.ToInt(body.GetQueryString("siteId"));
            var uploadType = EInputTypeUtils.GetEnumType(body.GetQueryString("uploadType"));

            var errorMessage = string.Empty;
            var fileUrls = new List<string>();
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            if (body.IsUserLoggin && publishmentSystemInfo != null)
            {
                try
                {
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var postedFile = HttpContext.Current.Request.Files[i];
                            var filePath = postedFile.FileName;
                            var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, fileExtName);
                            var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, filePath);
                            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                            if (uploadType == EInputType.Image)
                            {
                                if (!PathUtility.IsImageExtenstionAllowed(publishmentSystemInfo, fileExtName))
                                {
                                    errorMessage = "上传图片格式不正确！";
                                    break;
                                }
                                else if (!PathUtility.IsImageSizeAllowed(publishmentSystemInfo, postedFile.ContentLength))
                                {
                                    errorMessage = "上传失败，上传图片超出规定文件大小！";
                                    break;
                                }

                                postedFile.SaveAs(localFilePath);
                                FileUtility.AddWaterMark(publishmentSystemInfo, localFilePath);
                                var imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, localFilePath);
                                fileUrls.Add(imageUrl);
                            }
                            else if (uploadType == EInputType.Video)
                            {
                                if (!PathUtility.IsVideoExtenstionAllowed(publishmentSystemInfo, fileExtName))
                                {
                                    errorMessage = "上传视频格式不正确！";
                                    break;
                                }
                                if (!PathUtility.IsVideoSizeAllowed(publishmentSystemInfo, postedFile.ContentLength))
                                {
                                    errorMessage = "上传失败，上传视频超出规定文件大小！";
                                    break;
                                }
                                postedFile.SaveAs(localFilePath);
                                var videoUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, localFilePath);
                                fileUrls.Add(videoUrl);
                            }
                            else if (uploadType == EInputType.File)
                            {
                                if (!PathUtility.IsFileExtenstionAllowed(publishmentSystemInfo, fileExtName))
                                {
                                    errorMessage = "此格式不允许上传，请选择有效的文件！";
                                    break;
                                }
                                if (!PathUtility.IsFileSizeAllowed(publishmentSystemInfo, postedFile.ContentLength))
                                {
                                    errorMessage = "上传失败，上传文件超出规定文件大小！";
                                    break;
                                }
                                postedFile.SaveAs(localFilePath);
                                FileUtility.AddWaterMark(publishmentSystemInfo, localFilePath);
                                var fileUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, localFilePath);
                                fileUrls.Add(fileUrl);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    //errorMessage = ex.Message;
                    errorMessage = "程序错误";
                }
            }

            var builder = new StringBuilder();
            if (fileUrls.Count == 0)
            {
                builder.Append("{\"errorMessage\":\"");
                builder.Append(!string.IsNullOrEmpty(errorMessage) ? errorMessage : "未知错误");
                builder.Append("\"}");
            }
            else
            {
                
                builder.Append("{\"fileUrls\":[");
                foreach (var fileUrl in fileUrls)
                {
                    builder.Append("\"");
                    builder.Append(fileUrl);
                    builder.Append("\",");
                }
                builder.Length--;
                builder.Append("]}");
            }

            var resp = new HttpResponseMessage();
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Headers.Get("X-Access-Token")))
            {
                resp.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");
            }
            else
            {
                resp.Content = new StringContent(builder.ToString(), Encoding.UTF8, "text/plain");
            }

            return resp;
        }
    }
}