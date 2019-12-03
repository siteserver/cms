using System.IO;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

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

            var site = DataProvider.SiteRepository.GetAsync(AuthRequest.SiteId).GetAwaiter().GetResult();

            if (fileCount > 0)
            {
                var file = AuthRequest.HttpRequest.Files[0];

                //var fileName = Path.GetFileName(file.FileName);
                //var path = context.Server.MapPath("~/upload/" + fileName);

                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                filePath = file.FileName;
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();

                if (!PathUtility.IsImageExtensionAllowed(site, fileExtName))
                {
                    errorMessage = "上传失败，上传图片格式不正确！";
                }
                if (!PathUtility.IsImageSizeAllowed(site, file.ContentLength))
                {
                    errorMessage = "上传失败，上传图片超出规定文件大小！";
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(site, filePath);
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
