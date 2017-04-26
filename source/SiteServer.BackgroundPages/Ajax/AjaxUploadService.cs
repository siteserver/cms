using System;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Images;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxUploadService : BasePageCms
    {
        public static string GetContentPhotoUploadSingleUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxUploadService), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"isContentPhoto", true.ToString()}
            });
        }

        public static string GetContentPhotoUploadMultipleUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxUploadService), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"isContentPhotoSwfUpload", true.ToString()}
            });
        }

        public static string GetUploadWordMultipleUrl(int publishmentSystemId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxUploadService), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"isWordSwfUpload", true.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var jsonAttributes = new NameValueCollection();

            if (TranslateUtils.ToBool(Request.QueryString["isContentPhoto"]))
            {
                string message;
                string url;
                string smallUrl;
                string middleUrl;
                string largeUrl;
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
                string message;
                string url;
                string smallUrl;
                string middleUrl;
                string largeUrl;
                var success = UploadContentPhotoSwfUpload(out message, out url, out smallUrl, out middleUrl, out largeUrl);
                jsonAttributes.Add("success", success.ToString().ToLower());
                jsonAttributes.Add("message", message);
                jsonAttributes.Add("url", url);
                jsonAttributes.Add("smallUrl", smallUrl);
                jsonAttributes.Add("middleUrl", middleUrl);
                jsonAttributes.Add("largeUrl", largeUrl);
            }
            else if (TranslateUtils.ToBool(Request.QueryString["isWordSwfUpload"]))
            {
                string message;
                string fileName;
                var success = UploadWordSwfUpload(out message, out fileName);
                jsonAttributes.Add("success", success.ToString().ToLower());
                jsonAttributes.Add("message", message);
                jsonAttributes.Add("fileName", fileName);
            }
            else if (TranslateUtils.ToBool(Request.QueryString["isResume"]))
            {
                string message;
                string url;
                string value;
                var success = UploadResumeImage(out message, out url, out value);
                jsonAttributes.Add("success", success.ToString().ToLower());
                jsonAttributes.Add("message", message);
                jsonAttributes.Add("url", url);
                jsonAttributes.Add("value", value);
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);

            Response.Write(jsonString);
            Response.End();
        }

        public bool UploadContentPhotoImage(out string message, out string url, out string smallUrl, out string middleUrl, out string largeUrl)
        {
            message = url = smallUrl = middleUrl = largeUrl = string.Empty;

            if (Request.Files["ImageUrl"] == null) return false;

            var postedFile = Request.Files["ImageUrl"];

            try
            {
                var fileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, postedFile.FileName);
                var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                var directoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                var fileNameSmall = "small_" + fileName;
                var fileNameMiddle = "middle_" + fileName;
                var filePath = PathUtils.Combine(directoryPath, fileName);
                var filePathSamll = PathUtils.Combine(directoryPath, fileNameSmall);
                var filePathMiddle = PathUtils.Combine(directoryPath, fileNameMiddle);

                if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                {
                    if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                    {
                        message = "上传失败，上传图片超出规定文件大小！";
                        return false;
                    }

                    postedFile.SaveAs(filePath);

                    FileUtility.AddWaterMark(PublishmentSystemInfo, filePath);

                    var widthSmall = PublishmentSystemInfo.Additional.PhotoSmallWidth;
                    var heightSamll = PublishmentSystemInfo.Additional.PhotoSmallHeight;
                    ImageUtils.MakeThumbnail(filePath, filePathSamll, widthSmall, heightSamll, true);

                    var widthMiddle = PublishmentSystemInfo.Additional.PhotoMiddleWidth;
                    var heightMiddle = PublishmentSystemInfo.Additional.PhotoMiddleHeight;
                    ImageUtils.MakeThumbnail(filePath, filePathMiddle, widthMiddle, heightMiddle, true);

                    url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePathSamll);

                    smallUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, url);
                    middleUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePathMiddle));
                    largeUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePath));
                    return true;
                }
                message = "您必须上传图片文件！";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }

        public bool UploadContentPhotoSwfUpload(out string message, out string url, out string smallUrl, out string middleUrl, out string largeUrl)
        {
            message = url = smallUrl = middleUrl = largeUrl = string.Empty;

            if (Request.Files["Filedata"] == null) return false;
            var postedFile = Request.Files["Filedata"];

            try
            {
                var fileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, postedFile.FileName);
                var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                var directoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                var fileNameSmall = "small_" + fileName;
                var fileNameMiddle = "middle_" + fileName;
                var filePath = PathUtils.Combine(directoryPath, fileName);
                var filePathSmall = PathUtils.Combine(directoryPath, fileNameSmall);
                var filePathMiddle = PathUtils.Combine(directoryPath, fileNameMiddle);

                if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                {
                    if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                    {
                        message = "上传失败，上传图片超出规定文件大小！";
                        return false;
                    }

                    postedFile.SaveAs(filePath);

                    FileUtility.AddWaterMark(PublishmentSystemInfo, filePath);

                    var widthSmall = PublishmentSystemInfo.Additional.PhotoSmallWidth;
                    var heightSmall = PublishmentSystemInfo.Additional.PhotoSmallHeight;
                    ImageUtils.MakeThumbnail(filePath, filePathSmall, widthSmall, heightSmall, true);

                    var widthMiddle = PublishmentSystemInfo.Additional.PhotoMiddleWidth;
                    var heightMiddle = PublishmentSystemInfo.Additional.PhotoMiddleHeight;
                    ImageUtils.MakeThumbnail(filePath, filePathMiddle, widthMiddle, heightMiddle, true);

                    url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePathSmall);

                    smallUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, url);
                    middleUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePathMiddle));
                    largeUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, filePath));
                    return true;
                }
                message = "您必须上传图片文件！";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }

        public bool UploadWordSwfUpload(out string message, out string fileName)
        {
            message = fileName = string.Empty;

            if (Request.Files["Filedata"] == null) return false;
            var postedFile = Request.Files["Filedata"];

            try
            {
                fileName = postedFile.FileName;
                var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                if (extendName == ".doc" || extendName == ".docx")
                {
                    var filePath = WordUtils.GetWordFilePath(fileName);
                    postedFile.SaveAs(filePath);
                    return true;
                }
                message = "请选择Word文件上传！";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }

        public bool UploadResumeImage(out string message, out string url, out string value)
        {
            message = url = value = string.Empty;

            if (Request.Files["ImageUrl"] == null) return false;
            var postedFile = Request.Files["ImageUrl"];

            var filePath = postedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                if (!PathUtility.IsImageExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                {
                    message = "上传失败，上传图片格式不正确！";
                    return false;
                }
                if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                {
                    message = "上传失败，上传图片超出规定文件大小！";
                    return false;
                }

                postedFile.SaveAs(localFilePath);

                url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                value = PageUtility.GetVirtualUrl(PublishmentSystemInfo, url);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}
