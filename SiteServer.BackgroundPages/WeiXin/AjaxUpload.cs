using System;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Images;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class AjaxUpload : BasePageCms
    {
        public static string GetImageUrlUploadUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(AjaxUpload), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"isUploadImageUrl", true.ToString()}
            });
        }

        public static string GetContentPhotoUploadMultipleUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(AjaxUpload), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"isContentPhotoSwfUpload", true.ToString()}
            });
        }

        public static string GetContentPhotoUploadSingleUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(AjaxUpload), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"isContentPhoto", true.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var jsonAttributes = new NameValueCollection();

            if (TranslateUtils.ToBool(Request.QueryString["isUploadImageUrl"]))
            {
                var message = string.Empty;
                var url = string.Empty;
                var virtualUrl = string.Empty;

                var success = UploadImageUrl(out message, out url, out virtualUrl);
                jsonAttributes.Add("success", success.ToString().ToLower());
                jsonAttributes.Add("message", message);
                jsonAttributes.Add("url", url);
                jsonAttributes.Add("virtualUrl", virtualUrl);
            }
            else if (TranslateUtils.ToBool(Request.QueryString["isContentPhoto"]))
            {
                var message = string.Empty;
                var url = string.Empty;
                var smallUrl = string.Empty;
                var middleUrl = string.Empty;
                var largeUrl = string.Empty;
                var success = UploadContentPhotoImage(out message, out url, out smallUrl, out middleUrl, out largeUrl);
                jsonAttributes.Add("success", success.ToString().ToLower());
                jsonAttributes.Add("message", message);
                jsonAttributes.Add("url", url);
                jsonAttributes.Add("smallUrl", smallUrl);
                jsonAttributes.Add("middleUrl", middleUrl);
                jsonAttributes.Add("largeUrl", largeUrl);
            }
            else if (TranslateUtils.ToBool(Request.QueryString["isContentPhotoSwfUpload"]))
            {
                var message = string.Empty;
                var url = string.Empty;
                var smallUrl = string.Empty;
                var largeUrl = string.Empty;
                var success = UploadContentPhotoSwfUpload(out message, out url, out smallUrl, out largeUrl);
                jsonAttributes.Add("success", success.ToString().ToLower());
                jsonAttributes.Add("message", message);
                jsonAttributes.Add("url", url);
                jsonAttributes.Add("smallUrl", smallUrl);
                jsonAttributes.Add("largeUrl", largeUrl);
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);

            Response.Write(jsonString);
            Response.End();
        }

        public bool UploadImageUrl(out string message, out string url, out string virtualUrl)
        {
            message = url = virtualUrl = string.Empty;

            if (Request.Files != null && Request.Files["Upload"] != null)
            {
                var postedFile = Request.Files["Upload"];

                try
                {
                    var fileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, postedFile.FileName);
                    var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                    var directoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                    var filePath = PathUtils.Combine(directoryPath, fileName);

                    if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                    {
                        if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                        {
                            FailMessage("上传失败，上传图片超出规定文件大小！");
                            return false;
                        }

                        postedFile.SaveAs(filePath);

                        FileUtility.AddWaterMark(PublishmentSystemInfo, filePath);

                        url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePath);
                        virtualUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, url);

                        return true;
                    }
                    else
                    {
                        message = "您必须上传图片文件！";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return false;
        }

        public bool UploadContentPhotoImage(out string message, out string url, out string smallUrl, out string middleUrl, out string largeUrl)
        {
            message = url = smallUrl = middleUrl = largeUrl = string.Empty;

            if (Request.Files != null && Request.Files["ImageUrl"] != null)
            {
                var postedFile = Request.Files["ImageUrl"];

                try
                {
                    var fileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, postedFile.FileName);
                    var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                    var directoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                    var fileNameSmall = "small_" + fileName;
                    var filePath = PathUtils.Combine(directoryPath, fileName);
                    var filePathSamll = PathUtils.Combine(directoryPath, fileNameSmall);

                    if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                    {
                        if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                        {
                            FailMessage("上传失败，上传图片超出规定文件大小！");
                            return false;
                        }

                        postedFile.SaveAs(filePath);

                        FileUtility.AddWaterMark(PublishmentSystemInfo, filePath);

                        var widthSmall = 200;
                        ImageUtils.MakeThumbnail(filePath, filePathSamll, widthSmall, 0, true);

                        url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePathSamll);

                        smallUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, url);
                        largeUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePath));
                        return true;
                    }
                    else
                    {
                        message = "您必须上传图片文件！";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return false;
        }

        public bool UploadContentPhotoSwfUpload(out string message, out string url, out string smallUrl, out string largeUrl)
        {
            message = url = smallUrl = largeUrl = string.Empty;

            if (Request.Files != null && Request.Files["Filedata"] != null)
            {
                var postedFile = Request.Files["Filedata"];

                try
                {
                    var fileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, postedFile.FileName);
                    var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                    var directoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                    var fileNameSmall = "small_" + fileName;
                    var filePath = PathUtils.Combine(directoryPath, fileName);
                    var filePathSmall = PathUtils.Combine(directoryPath, fileNameSmall);

                    if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                    {
                        if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                        {
                            FailMessage("上传失败，上传图片超出规定文件大小！");
                            return false;
                        }

                        postedFile.SaveAs(filePath);

                        FileUtility.AddWaterMark(PublishmentSystemInfo, filePath);

                        var widthSmall = 200;
                        ImageUtils.MakeThumbnail(filePath, filePathSmall, widthSmall, 0, true);

                        url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePathSmall);

                        smallUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, url);
                        largeUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePath));
                        return true;
                    }
                    else
                    {
                        message = "您必须上传图片文件！";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return false;
        }
    }
}
