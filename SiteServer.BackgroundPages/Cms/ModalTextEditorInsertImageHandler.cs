using System;
using System.IO;
using System.Web;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTextEditorInsertImageHandler : IHttpHandler
    {
        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsWebHandlerUrl(siteId, nameof(ModalTextEditorInsertImageHandler), null);
        }

        public void ProcessRequest(HttpContext context)
        {
            var body = new AuthRequest();

            if (!body.IsAdminLoggin) return;

            var request = context.Request;

            var action = request["action"];
            var hash = request["hash"];
            var fileName = request["fileName"];

            var fileCount = request.Files.Count;

            string filePath = null;
            string errorMessage = null;

            var siteInfo = SiteManager.GetSiteInfo(body.SiteId);

            if (string.IsNullOrEmpty(hash))
            {
                //普通上传
                if (fileCount > 0)
                {
                    var file = request.Files[0];

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
            }
            else
            {
                //秒传或断点续传
                //var path = context.Server.MapPath("~/upload/" + hash);
                var path = PathUtils.GetTemporaryFilesPath(hash);

                var fileExtName = PathUtils.GetExtension(fileName).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(siteInfo, localDirectoryPath);
                var pathOk = PathUtils.Combine(localDirectoryPath, localFileName);

                //状态查询
                if (action == "query")
                {
                    if (File.Exists(pathOk))
                    {
                        Finish(GetResponseJson(fileName, pathOk, string.Empty));
                    }
                    else if (File.Exists(path))
                    {
                        Finish(new FileInfo(path).Length.ToString());
                    }
                    else
                    {
                        Finish("0");
                    }
                }
                else
                {
                    if (fileCount > 0)
                    {
                        var file = request.Files[0];
                        using (var fs = File.Open(path, FileMode.Append))
                        {
                            byte[] buffer = new byte[file.ContentLength];
                            file.InputStream.Read(buffer, 0, file.ContentLength);

                            fs.Write(buffer, 0, buffer.Length);
                        }
                    }

                    var isOk = request["ok"] == "1";
                    if (!isOk) Finish("1");

                    if (File.Exists(path)) File.Move(path, pathOk);
                }
            }

            Finish(GetResponseJson(fileName, filePath, errorMessage));
        }

        /// <summary>
        /// 获取返回的json字符串
        /// </summary>
        /// <returns></returns>
        private static string GetResponseJson(string fileName, string filePath, string errorMessage)
        {
            FileInfo file = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                file = new FileInfo(filePath);
            }
            if (file != null)
            {
                return TranslateUtils.JsonSerialize(new
                {
                    fileName,
                    filePath,
                    length = file?.Length,
                    ret = 1
                });
            }

            return TranslateUtils.JsonSerialize(new
            {
                ret = 0,
                errorMessage
            });
        }

        /// <summary>
        /// 完成上传
        /// </summary>
        /// <param name="json">回调函数参数</param>
        private static void Finish(string json)
        {
            var response = HttpContext.Current.Response;

            response.Write(json);
            response.End();
        }

        public bool IsReusable => false;
    }
}
