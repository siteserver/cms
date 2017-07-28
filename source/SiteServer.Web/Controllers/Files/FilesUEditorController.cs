using System.Web;
using System.Web.Http;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Files;
using SiteServer.CMS.UEditor;

namespace SiteServer.API.Controllers.Files
{
    [RoutePrefix("api")]
    public class FilesUEditorController : ApiController
    {
        [HttpGet, Route(UEditor.Route)]
        public void GetMain(int publishmentSystemId)
        {
            Main(publishmentSystemId);
        }

        [HttpPost, Route(UEditor.Route)]
        public void PostMain(int publishmentSystemId)
        {
            Main(publishmentSystemId);
        }

        public void Main(int publishmentSystemId)
        {
            var queryString = HttpContext.Current.Request.QueryString;

            Handler action;
            switch (queryString["action"])
            {
                case "config":
                    action = new ConfigHandler(HttpContext.Current);
                    break;
                case "uploadimage":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("imageAllowFiles"),
                        PathFormat = Config.GetString("imagePathFormat"),
                        SizeLimit = Config.GetInt("imageMaxSize"),
                        UploadFieldName = Config.GetString("imageFieldName")
                    }, publishmentSystemId, EUploadType.Image);
                    break;
                case "uploadscrawl":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = new[] { ".png" },
                        PathFormat = Config.GetString("scrawlPathFormat"),
                        SizeLimit = Config.GetInt("scrawlMaxSize"),
                        UploadFieldName = Config.GetString("scrawlFieldName"),
                        Base64 = true,
                        Base64Filename = "scrawl.png"
                    }, publishmentSystemId, EUploadType.Image);
                    break;
                case "uploadvideo":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                        PathFormat = Config.GetString("videoPathFormat"),
                        SizeLimit = Config.GetInt("videoMaxSize"),
                        UploadFieldName = Config.GetString("videoFieldName")
                    }, publishmentSystemId, EUploadType.Video);
                    break;
                case "uploadfile":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                        PathFormat = Config.GetString("filePathFormat"),
                        SizeLimit = Config.GetInt("fileMaxSize"),
                        UploadFieldName = Config.GetString("fileFieldName")
                    }, publishmentSystemId, EUploadType.File);
                    break;
                case "listimage":
                    action = new ListFileManager(HttpContext.Current, Config.GetString("imageManagerListPath"), Config.GetStringList("imageManagerAllowFiles"), publishmentSystemId, EUploadType.Image);
                    break;
                case "listfile":
                    action = new ListFileManager(HttpContext.Current, Config.GetString("fileManagerListPath"), Config.GetStringList("fileManagerAllowFiles"), publishmentSystemId, EUploadType.File);
                    break;
                case "catchimage":
                    action = new CrawlerHandler(HttpContext.Current, publishmentSystemId);
                    break;
                default:
                    action = new NotSupportedHandler(HttpContext.Current);
                    break;
            }
            action.Process();
        }
    }
}