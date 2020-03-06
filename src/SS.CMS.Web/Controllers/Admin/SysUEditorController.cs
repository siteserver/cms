using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Pages
{
    [Route("sys/editors/ueditor/{siteId}")]
    public partial class SysUEditorController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public void GetMain(int siteId)
        {
            Main(siteId);
        }

        [HttpPost, Route(Route)]
        public void PostMain(int siteId)
        {
            Main(siteId);
        }

        private void Main(int siteId)
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
                    }, siteId, UploadType.Image);
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
                    }, siteId, UploadType.Image);
                    break;
                case "uploadvideo":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                        PathFormat = Config.GetString("videoPathFormat"),
                        SizeLimit = Config.GetInt("videoMaxSize"),
                        UploadFieldName = Config.GetString("videoFieldName")
                    }, siteId, UploadType.Video);
                    break;
                case "uploadfile":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                        PathFormat = Config.GetString("filePathFormat"),
                        SizeLimit = Config.GetInt("fileMaxSize"),
                        UploadFieldName = Config.GetString("fileFieldName")
                    }, siteId, UploadType.File);
                    break;
                case "listimage":
                    action = new ListFileManager(HttpContext.Current, Config.GetString("imageManagerListPath"), Config.GetStringList("imageManagerAllowFiles"), siteId, UploadType.Image);
                    break;
                case "listfile":
                    action = new ListFileManager(HttpContext.Current, Config.GetString("fileManagerListPath"), Config.GetStringList("fileManagerAllowFiles"), siteId, UploadType.File);
                    break;
                case "catchimage":
                    action = new CrawlerHandler(HttpContext.Current, siteId);
                    break;
                default:
                    action = new NotSupportedHandler(HttpContext.Current);
                    break;
            }
            action.Process();
        }
    }
}