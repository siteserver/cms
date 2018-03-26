using System;
using System.Collections.Specialized;
using SiteServer.Utils;
using SiteServer.Utils.Images;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxUploadService : BasePageCms
    {
        public static string GetContentPhotoUploadSingleUrl(int siteId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxUploadService), new NameValueCollection
            {
                {"siteID", siteId.ToString()},
                {"isContentPhoto", true.ToString()}
            });
        }

        public static string GetContentPhotoUploadMultipleUrl(int siteId)
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxUploadService), new NameValueCollection
            {
                {"siteID", siteId.ToString()},
                {"isContentPhotoSwfUpload", true.ToString()}
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
                var fileName = PathUtility.GetUploadFileName(SiteInfo, postedFile.FileName);
                var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                var directoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                var fileNameSmall = "small_" + fileName;
                var fileNameMiddle = "middle_" + fileName;
                var filePath = PathUtils.Combine(directoryPath, fileName);
                var filePathSamll = PathUtils.Combine(directoryPath, fileNameSmall);
                var filePathMiddle = PathUtils.Combine(directoryPath, fileNameMiddle);

                if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                {
                    if (!PathUtility.IsImageSizeAllowed(SiteInfo, postedFile.ContentLength))
                    {
                        message = "上传失败，上传图片超出规定文件大小！";
                        return false;
                    }

                    postedFile.SaveAs(filePath);

                    FileUtility.AddWaterMark(SiteInfo, filePath);

                    var widthSmall = SiteInfo.Additional.PhotoSmallWidth;
                    ImageUtils.MakeThumbnail(filePath, filePathSamll, widthSmall, 0, true);

                    var widthMiddle = SiteInfo.Additional.PhotoMiddleWidth;
                    ImageUtils.MakeThumbnail(filePath, filePathMiddle, widthMiddle, 0, true);

                    url = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePathSamll, true);

                    smallUrl = PageUtility.GetVirtualUrl(SiteInfo, url);
                    middleUrl = PageUtility.GetVirtualUrl(SiteInfo, PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePathMiddle, true));
                    largeUrl = PageUtility.GetVirtualUrl(SiteInfo, PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePath, true));
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
                var fileName = PathUtility.GetUploadFileName(SiteInfo, postedFile.FileName);
                var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                var directoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                var fileNameSmall = "small_" + fileName;
                var fileNameMiddle = "middle_" + fileName;
                var filePath = PathUtils.Combine(directoryPath, fileName);
                var filePathSmall = PathUtils.Combine(directoryPath, fileNameSmall);
                var filePathMiddle = PathUtils.Combine(directoryPath, fileNameMiddle);

                if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                {
                    if (!PathUtility.IsImageSizeAllowed(SiteInfo, postedFile.ContentLength))
                    {
                        message = "上传失败，上传图片超出规定文件大小！";
                        return false;
                    }

                    postedFile.SaveAs(filePath);

                    FileUtility.AddWaterMark(SiteInfo, filePath);

                    var widthSmall = SiteInfo.Additional.PhotoSmallWidth;
                    ImageUtils.MakeThumbnail(filePath, filePathSmall, widthSmall, 0, true);

                    var widthMiddle = SiteInfo.Additional.PhotoMiddleWidth;
                    ImageUtils.MakeThumbnail(filePath, filePathMiddle, widthMiddle, 0, true);

                    url = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePathSmall, true);

                    smallUrl = PageUtility.GetVirtualUrl(SiteInfo, url);
                    middleUrl = PageUtility.GetVirtualUrl(SiteInfo, PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePathMiddle, true));
                    largeUrl = PageUtility.GetVirtualUrl(SiteInfo, PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePath, true));
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

        public bool UploadResumeImage(out string message, out string url, out string value)
        {
            message = url = value = string.Empty;

            if (Request.Files["ImageUrl"] == null) return false;
            var postedFile = Request.Files["ImageUrl"];

            var filePath = postedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(SiteInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                if (!PathUtility.IsImageExtenstionAllowed(SiteInfo, fileExtName))
                {
                    message = "上传失败，上传图片格式不正确！";
                    return false;
                }
                if (!PathUtility.IsImageSizeAllowed(SiteInfo, postedFile.ContentLength))
                {
                    message = "上传失败，上传图片超出规定文件大小！";
                    return false;
                }

                postedFile.SaveAs(localFilePath);

                url = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                value = PageUtility.GetVirtualUrl(SiteInfo, url);
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
