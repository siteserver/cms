using System;
using System.IO;
using System.Web;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Cms
{
    public class WebHandlerUploadWord : IHttpHandler
    {
        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsWebHandlerUrl(siteId, nameof(WebHandlerUploadWord), null);
        } 

        public void ProcessRequest(HttpContext context)
        {
            var body = new Request();

            if (!body.IsAdminLoggin) return;

            var request = context.Request;

            var action = request["action"];
            var hash = request["hash"];
            var fileName = request["fileName"];

            var fileCount = request.Files.Count;

            string filePath = null;

            if (string.IsNullOrEmpty(hash))
            {
                //普通上传
                if (fileCount > 0)
                {
                    var file = request.Files[0];

                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = context.Server.MapPath("~/upload/" + fileName);

                    if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                    var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                    if (extendName == ".doc" || extendName == ".docx")
                    {
                        filePath = WordUtils.GetWordFilePath(fileName);
                        file.SaveAs(filePath);
                    }
                }
            }
            else
            {
                //秒传或断点续传
                //var path = context.Server.MapPath("~/upload/" + hash);
                var path = WordUtils.GetWordFilePath(hash);
                var pathOk = path + Path.GetExtension(fileName);

                //状态查询
                if (action == "query")
                {
                    if (File.Exists(pathOk))
                    {
                        Finish(GetResponseJson(fileName, pathOk));
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

            Finish(GetResponseJson(fileName, filePath));
        }

        /// <summary>
        /// 获取返回的json字符串
        /// </summary>
        /// <returns></returns>
        private static string GetResponseJson(string fileName, string filePath)
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
                    length = file?.Length,
                    ret = 1
                });
            }

            return TranslateUtils.JsonSerialize(new
            {
                ret = 0
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
