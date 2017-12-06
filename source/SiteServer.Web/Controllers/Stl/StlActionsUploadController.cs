using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsUploadController : ApiController
    {
        [HttpPost, Route(ActionsUpload.Route)]
        public IHttpActionResult Main(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var type = HttpContext.Current.Request.QueryString["type"];

            var jsonAttributes = new NameValueCollection();

            var success = false;
            var message = string.Empty;
            var url = string.Empty;
            var value = string.Empty;

            if (type == ActionsUpload.TypeResume)
            {
                success = UploadResumeImage(publishmentSystemInfo, out message, out url, out value);
            }
            else if (type == ActionsUpload.TypeGovPublicApply)
            {
                success = UploadGovPublicApply(publishmentSystemInfo, out message, out url, out value);
            }

            jsonAttributes.Add("success", success.ToString().ToLower());
            jsonAttributes.Add("message", message);
            jsonAttributes.Add("url", url);
            jsonAttributes.Add("value", value);

            return Ok(jsonAttributes);
        }

        public bool UploadResumeImage(PublishmentSystemInfo publishmentSystemInfo, out string message, out string url, out string value)
        {
            message = url = value = string.Empty;

            if (HttpContext.Current.Request.Files["ImageUrl"] == null) return false;
            var postedFile = HttpContext.Current.Request.Files["ImageUrl"];

            var filePath = postedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                if (!PathUtility.IsImageExtenstionAllowed(publishmentSystemInfo, fileExtName))
                {
                    message = "上传失败，上传图片格式不正确！";
                    return false;
                }
                if (!PathUtility.IsImageSizeAllowed(publishmentSystemInfo, postedFile.ContentLength))
                {
                    message = "上传失败，上传图片超出规定文件大小！";
                    return false;
                }

                postedFile.SaveAs(localFilePath);

                url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, localFilePath);
                value = PageUtility.GetVirtualUrl(publishmentSystemInfo, url);
                return true;
            }
            catch (Exception ex)
            {
                message = "程序错误";
                //message = ex.Message;
            }
            return false;
        }

        public bool UploadGovPublicApply(PublishmentSystemInfo publishmentSystemInfo, out string message, out string url, out string value)
        {
            message = url = value = string.Empty;

            if (HttpContext.Current.Request.Files.Count == 0) return false;
            var postedFile = HttpContext.Current.Request.Files[0];

            var filePath = postedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                postedFile.SaveAs(localFilePath);

                url = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, localFilePath);
                value = PageUtility.GetVirtualUrl(publishmentSystemInfo, url);
                return true;
            }
            catch (Exception ex)
            {
                //message = ex.Message;
                message = "程序出错。";
            }
            return false;
        }
    }
}
