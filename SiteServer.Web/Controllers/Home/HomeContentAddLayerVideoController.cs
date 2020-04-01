using System;
using System.Collections.Generic;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentAddLayerVideo")]
    public class HomeContentAddLayerVideoController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                return Ok(new
                {
                    Value = siteInfo.Additional
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var path = string.Empty;
                var url = string.Empty;
                var contentLength = 0;

                if (request.HttpRequest.Files.Count > 0)
                {
                    var file = request.HttpRequest.Files[0];

                    var filePath = file.FileName;
                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(siteInfo, filePath);
                    path = PathUtils.Combine(localDirectoryPath, localFileName);
                    contentLength = file.ContentLength;

                    if (!PathUtility.IsVideoExtenstionAllowed(siteInfo, fileExtName))
                    {
                        return BadRequest("上传失败，上传视频格式不正确！");
                    }
                    if (!PathUtility.IsVideoSizeAllowed(siteInfo, contentLength))
                    {
                        return BadRequest("上传失败，上传视频超出规定文件大小！");
                    }

                    file.SaveAs(path);
                    FileUtility.AddWaterMark(siteInfo, path);

                    url = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, path, true);
                }

                return Ok(new
                {
                    Path = path,
                    Url = url,
                    ContentLength = contentLength
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var filePaths = TranslateUtils.StringCollectionToStringList(request.GetPostString("filePaths"));

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<string>();
                var editors = new List<object>();

                foreach (var filePath in filePaths)
                {
                    if (string.IsNullOrEmpty(filePath)) continue;

                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var fileName = PathUtility.GetUploadFileName(siteInfo, filePath);

                    var directoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                    var fixFilePath = PathUtils.Combine(directoryPath, Constants.TitleImageAppendix + fileName);
                    var editorFixFilePath = PathUtils.Combine(directoryPath, Constants.SmallImageAppendix + fileName);

                    var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, filePath, true);

                    retVal.Add(imageUrl);

                    editors.Add(new
                    {
                        ImageUrl = imageUrl,
                        OriginalUrl = imageUrl
                    });
                }

                return Ok(new
                {
                    Value = retVal,
                    Editors = editors
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
