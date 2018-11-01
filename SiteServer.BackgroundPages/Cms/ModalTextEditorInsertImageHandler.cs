using System.IO;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTextEditorInsertImageHandler : BaseHandler
    {
        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsWebHandlerUrl(siteId, nameof(ModalTextEditorInsertImageHandler), null);
        }

        protected override object Process()
        {
            var fileName = AuthRequest.HttpRequest["fileName"];

            var fileCount = AuthRequest.HttpRequest.Files.Count;

            string filePath = null;
            string errorMessage = null;

            var siteInfo = SiteManager.GetSiteInfo(AuthRequest.SiteId);

            if (fileCount > 0)
            {
                var file = AuthRequest.HttpRequest.Files[0];

                //var fileName = Path.GetFileName(file.FileName);
                //var path = context.Server.MapPath("~/upload/" + fileName);

                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                filePath = file.FileName;
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();

                if (!PathUtility.IsImageExtenstionAllowed(siteInfo, fileExtName))
                {
                    errorMessage = "上传失败，上传图片格式不正确！";
                }
                if (!PathUtility.IsImageSizeAllowed(siteInfo, file.ContentLength))
                {
                    errorMessage = "上传失败，上传图片超出规定文件大小！";
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(siteInfo, filePath);
                    filePath = PathUtils.Combine(localDirectoryPath, localFileName);
                    file.SaveAs(filePath);
                }
            }

            return GetResponseObject(fileName, filePath, errorMessage);
        }

        /// <summary>
        /// 获取返回的json字符串
        /// </summary>
        /// <returns></returns>
        private static object GetResponseObject(string fileName, string filePath, string errorMessage)
        {
            FileInfo file = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                file = new FileInfo(filePath);
            }
            if (file != null)
            {
                return new
                {
                    fileName,
                    filePath,
                    length = file?.Length,
                    ret = 1
                };
            }

            return new
            {
                ret = 0,
                errorMessage
            };
        }
    }
}
