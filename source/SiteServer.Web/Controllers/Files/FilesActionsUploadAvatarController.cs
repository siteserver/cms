using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Files;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Files
{
    [RoutePrefix("api")]
    public class FilesActionsUploadAvatarController : ApiController
    {
        [HttpPost, Route(ActionsUploadAvatar.Route)]
        public HttpResponseMessage Main()
        {
            var body = new RequestBody();

            var avatarUrl = string.Empty;
            var relatedUrl = string.Empty;
            var errorMessage = string.Empty;

            if (body.IsUserLoggin)
            {
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    try
                    {
                        var postedFile = HttpContext.Current.Request.Files[0];
                        var filePath = postedFile.FileName;
                        var fileExtName = PathUtils.GetExtension(filePath).ToLower();

                        var localDirectoryPath = PathUtils.GetUserUploadDirectoryPath(body.UserName);
                        var localFileName = PathUtils.GetUserUploadFileName(filePath);
                        var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                        var extendType = ".jpg|.png|.gif|.jpeg|";
                        if (extendType.IndexOf(fileExtName, StringComparison.Ordinal) != -1)
                        {
                            postedFile.SaveAs(localFilePath);
                            relatedUrl = PathUtils.GetPathDifference(PathUtils.GetUserFilesPath(body.UserName, string.Empty), localFilePath);
                            relatedUrl = relatedUrl.Replace("\\", "/");
                            avatarUrl = PageUtils.AddProtocolToUrl(PageUtils.GetUserFilesUrl(body.UserName, relatedUrl));
                        }
                    }
                    catch (Exception ex)
                    { 
                        errorMessage = "程序错误";
                        //errorMessage = ex.Message;
                    }
                }
            }

            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                builder.Append("{\"avatarUrl\":\"" + avatarUrl + "\", \"relatedUrl\":\"" + relatedUrl + "\"}");
            }
            else
            {
                builder.Append("{\"errorMessage\":\"");
                builder.Append(!string.IsNullOrEmpty(errorMessage) ? errorMessage : "未知错误");
                builder.Append("\"}");
            }

            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Headers.Get("X-Access-Token")))
            {
                resp.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");
            }
            else
            {
                resp.Content = new StringContent(builder.ToString(), Encoding.UTF8, "text/plain");
            }

            return resp;
        }
    }
}