using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Editors;
using SiteServer.CMS.UEditor;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Sys
{
    public class SysUEditorController : ApiController
    {
        [HttpGet, Route(ApiRouteUEditor.Route)]
        public void GetMain(int siteId)
        {
            Main(siteId);
        }

        [HttpPost, Route(ApiRouteUEditor.Route)]
        public void PostMain(int siteId)
        {
            Main(siteId);
        }

        public void Main(int siteId)
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
                    }, siteId, EUploadType.Image);
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
                    }, siteId, EUploadType.Image);
                    break;
                case "uploadvideo":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                        PathFormat = Config.GetString("videoPathFormat"),
                        SizeLimit = Config.GetInt("videoMaxSize"),
                        UploadFieldName = Config.GetString("videoFieldName")
                    }, siteId, EUploadType.Video);
                    break;
                case "uploadfile":
                    action = new UploadHandler(HttpContext.Current, new UploadConfig
                    {
                        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                        PathFormat = Config.GetString("filePathFormat"),
                        SizeLimit = Config.GetInt("fileMaxSize"),
                        UploadFieldName = Config.GetString("fileFieldName")
                    }, siteId, EUploadType.File);
                    break;
                case "listimage":
                    action = new ListFileManager(HttpContext.Current, Config.GetString("imageManagerListPath"), Config.GetStringList("imageManagerAllowFiles"), siteId, EUploadType.Image);
                    break;
                case "listfile":
                    action = new ListFileManager(HttpContext.Current, Config.GetString("fileManagerListPath"), Config.GetStringList("fileManagerAllowFiles"), siteId, EUploadType.File);
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