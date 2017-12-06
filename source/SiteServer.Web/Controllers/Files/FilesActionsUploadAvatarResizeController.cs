using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Files;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Files
{
    [RoutePrefix("api")]
    public class FilesActionsUploadAvatarResizeController : ApiController
    {
        [HttpPost, Route(ActionsUploadAvatarResize.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();

            try
            {
                if (!body.IsUserLoggin) return Unauthorized();

                var size = TranslateUtils.ToInt(body.GetPostString("size"));
                var x = TranslateUtils.ToInt(body.GetPostString("x"));
                var y = TranslateUtils.ToInt(body.GetPostString("y"));
                var relatedUrl = body.GetPostString("relatedUrl");

                var filePath = PathUtils.GetUserFilesPath(body.UserName, relatedUrl);
                
                var originalImage = Image.FromFile(filePath);

                var bmpOut = new Bitmap(size, size, PixelFormat.Format24bppRgb);
                var g = Graphics.FromImage(bmpOut);
                g.DrawImage(originalImage, new Rectangle(0, 0, size, size), new Rectangle(x, y, size, size), GraphicsUnit.Pixel);
                g.Dispose();

                if (size > 150)
                {
                    bmpOut = new Bitmap(bmpOut, 150, 150);
                }

                //保存图片地址
                var localDirectoryPath = PathUtils.GetUserUploadDirectoryPath(body.UserName);
                var dt = DateTime.Now;
                string strDateTime = $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}";
                string localFileName = $"{strDateTime}{PathUtils.GetExtension(filePath)}";
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);
                bmpOut.Save(localFilePath, ImageFormat.Jpeg);

                relatedUrl = PathUtils.GetPathDifference(PathUtils.GetUserFilesPath(body.UserName, string.Empty), localFilePath);
                relatedUrl = relatedUrl.Replace("\\", "/");
                var avatarUrl = PageUtils.AddProtocolToUrl(PageUtils.GetUserFilesUrl(body.UserName, relatedUrl)); 

                return Ok(new
                {
                    AvatarUrl = avatarUrl
                });
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}