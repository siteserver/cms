using System;
using System.IO;
using SiteServer.CMS.Core.Office;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadWordHandler : BaseHandler
    {
        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsWebHandlerUrl(siteId, nameof(ModalUploadWordHandler), null);
        }

        protected override object Process()
        {
            var fileName = AuthRequest.HttpRequest["fileName"];

            var fileCount = AuthRequest.HttpRequest.Files.Count;

            string filePath = null;

            if (fileCount > 0)
            {
                var file = AuthRequest.HttpRequest.Files[0];

                //var fileName = Path.GetFileName(file.FileName);
                //var path = context.Server.MapPath("~/upload/" + fileName);

                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                if (extendName == ".doc" || extendName == ".docx")
                {
                    filePath = PathUtils.GetTemporaryFilesPath(fileName);
                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    file.SaveAs(filePath);
                }
            }

            return GetResponseObject(fileName, filePath);
        }

        /// <summary>
        /// 获取返回的json字符串
        /// </summary>
        /// <returns></returns>
        private static object GetResponseObject(string fileName, string filePath)
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
                    length = file?.Length,
                    ret = 1
                };
            }

            return new
            {
                ret = 0
            };
        }
    }
}
