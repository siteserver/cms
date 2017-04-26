using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Platform
{
    public class PageTextEditorUpload : Page
	{
        public Literal ltlScript;

        private string _fileType;

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("FileType");
            var body = new RequestBody();

            if (body.IsAdministratorLoggin)
            {
                _fileType = Request.QueryString["FileType"];

                UploadForUEditor();
            }
		}

        private void UploadForUEditor()
        {
            if (_fileType == "Image")
            {
                var info = new Hashtable();
                var up = new UEditorUploaderForUser();
                info = up.upFile(HttpContext.Current);                               //获取上传状态

                var title = up.getOtherInfo(HttpContext.Current, "pictitle");                              //获取图片描述
                var oriName = up.getOtherInfo(HttpContext.Current, "fileName");                //获取原始文件名

                Response.ContentType = "text/plain";
                Response.Write("{'url':'" + info["url"] + "','title':'" + title + "','original':'" + oriName + "','state':'" + info["state"] + "'}");
                Response.End();
            }
            else if (_fileType == "Scrawl")
            {
                var action = Request["action"];

                if (action == "tmpImg")
                {
                    var info = new Hashtable();
                    var up = new UEditorUploaderForUser();
                    info = up.upFile(HttpContext.Current); //获取上传状态

                    Response.ContentType = "text/html";
                    Response.Write("<script>parent.ue_callback('" + info["url"] + "','" + info["state"] + "')</script>");//回调函数
                    Response.End();
                }
                else
                {
                    var info = new Hashtable();
                    var up = new UEditorUploaderForUser();
                    info = up.upScrawl(HttpContext.Current, Request["content"]); //获取上传状态

                    Response.ContentType = "text/plain";
                    Response.Write("{'url':'" + info["url"] + "',state:'" + info["state"] + "'}");
                    Response.End();
                }
            }
            else if (_fileType == "File")
            {
                var info = new Hashtable();
                var up = new UEditorUploaderForUser();
                info = up.upFile(HttpContext.Current); //获取上传状态
                Response.ContentType = "text/plain";
                Response.Write("{'state':'" + info["state"] + "','url':'" + info["url"] + "','fileType':'" + info["currentType"] + "','original':'" + info["originalName"] + "'}");
                Response.End();
            }
            else if (_fileType == "ImageManager")
            {
                Response.ContentType = "text/plain";

                var directoryPath = PathUtils.GetUserUploadDirectoryPath(string.Empty);
                string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };                //文件允许格式

                var action = Server.HtmlEncode(Request["action"]);

                if (action == "get")
                {
                    var str = string.Empty;

                    //目录验证
                    if (DirectoryUtils.IsDirectoryExists(directoryPath))
                    {
                        var filePathArray = DirectoryUtils.GetFilePaths(directoryPath);
                        foreach (var filePath in filePathArray)
                        {
                            if (EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(filePath)))
                            {
                                str += PageUtils.GetRootUrlByPhysicalPath(filePath) + "ue_separate_ue";
                            }
                        }
                    }
                    Response.Write(str);
                    Response.End();
                }
            }
            else if (_fileType == "GetMovie")
            {
                Response.ContentType = "text/html";
                var key = Server.HtmlEncode(Request.Form["searchKey"]);
                var type = Server.HtmlEncode(Request.Form["videoType"]);

                var httpURL = new Uri("http://api.tudou.com/v3/gw?method=item.search&appKey=myKey&format=json&kw=" + key + "&pageNo=1&pageSize=20&channelId=" + type + "&inDays=7&media=v&sort=s");
                var MyWebClient = new WebClient();

                //获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                var pageData = MyWebClient.DownloadData(httpURL);

                Response.Write(Encoding.UTF8.GetString(pageData));
                Response.End();
            }
        }
	}
}
