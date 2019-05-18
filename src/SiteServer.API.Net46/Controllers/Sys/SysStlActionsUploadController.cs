using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsUploadController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsUpload.Route)]
        public IHttpActionResult Main(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var type = HttpContext.Current.Request.QueryString["type"];

            var jsonAttributes = new NameValueCollection();

            var success = false;
            var message = string.Empty;
            var url = string.Empty;
            var value = string.Empty;

            if (type == ApiRouteActionsUpload.TypeResume)
            {
                success = UploadResumeImage(siteInfo, out message, out url, out value);
            }
            else if (type == ApiRouteActionsUpload.TypeGovPublicApply)
            {
                success = UploadGovPublicApply(siteInfo, out message, out url, out value);
            }

            jsonAttributes.Add("success", success.ToString().ToLower());
            jsonAttributes.Add("message", message);
            jsonAttributes.Add("url", url);
            jsonAttributes.Add("value", value);

            return Ok(jsonAttributes);
        }

        public bool UploadResumeImage(SiteInfo siteInfo, out string message, out string url, out string value)
        {
            message = url = value = string.Empty;

            if (HttpContext.Current.Request.Files["ImageUrl"] == null) return false;
            var postedFile = HttpContext.Current.Request.Files["ImageUrl"];

            var filePath = postedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(siteInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                if (!PathUtility.IsImageExtenstionAllowed(siteInfo, fileExtName))
                {
                    message = "上传失败，上传图片格式不正确！";
                    return false;
                }
                if (!PathUtility.IsImageSizeAllowed(siteInfo, postedFile.ContentLength))
                {
                    message = "上传失败，上传图片超出规定文件大小！";
                    return false;
                }

                postedFile.SaveAs(localFilePath);

                url = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, localFilePath, false);
                value = PageUtility.GetVirtualUrl(siteInfo, url);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }

        public bool UploadGovPublicApply(SiteInfo siteInfo, out string message, out string url, out string value)
        {
            message = url = value = string.Empty;

            if (HttpContext.Current.Request.Files.Count == 0) return false;
            var postedFile = HttpContext.Current.Request.Files[0];

            var filePath = postedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(siteInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                postedFile.SaveAs(localFilePath);

                url = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, localFilePath, false);
                value = PageUtility.GetVirtualUrl(siteInfo, url);
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
