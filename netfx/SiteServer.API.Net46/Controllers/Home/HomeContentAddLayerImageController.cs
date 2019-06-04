using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentAddLayerImage")]
    public class HomeContentAddLayerImageController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = GetRequest();

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
                    Value = siteInfo
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
                var request = GetRequest();

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

                if (request.Files.Count > 0)
                {
                    var file = request.Files[0];

                    var filePath = file.FileName;
                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(siteInfo, filePath);
                    path = PathUtils.Combine(localDirectoryPath, localFileName);
                    contentLength = file.ContentLength;

                    if (!PathUtility.IsImageExtenstionAllowed(siteInfo, fileExtName))
                    {
                        return BadRequest("上传失败，上传图片格式不正确！");
                    }
                    if (!PathUtility.IsImageSizeAllowed(siteInfo, contentLength))
                    {
                        return BadRequest("上传失败，上传图片超出规定文件大小！");
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
                var request = GetRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var isFix = request.GetPostBool("isFix");
                var fixWidth = request.GetPostString("fixWidth");
                var fixHeight = request.GetPostString("fixHeight");
                var isEditor = request.GetPostBool("isEditor");
                var editorIsFix = request.GetPostBool("editorIsFix");
                var editorFixWidth = request.GetPostString("editorFixWidth");
                var editorFixHeight = request.GetPostString("editorFixHeight");
                var editorIsLinkToOriginal = request.GetPostBool("editorIsLinkToOriginal");
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

                var retval = new List<string>();
                var editors = new List<object>();

                foreach (var filePath in filePaths)
                {
                    if (string.IsNullOrEmpty(filePath)) continue;

                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var fileName = PathUtility.GetUploadFileName(siteInfo, filePath);

                    var directoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, fileExtName);
                    var fixFilePath = PathUtils.Combine(directoryPath, Constants.TitleImageAppendix + fileName);
                    var editorFixFilePath = PathUtils.Combine(directoryPath, Constants.SmallImageAppendix + fileName);

                    var isImage = EFileSystemTypeUtils.IsImage(fileExtName);

                    if (isImage)
                    {
                        if (isFix)
                        {
                            var width = TranslateUtils.ToInt(fixWidth);
                            var height = TranslateUtils.ToInt(fixHeight);
                            ImageUtils.MakeThumbnail(filePath, fixFilePath, width, height, true);
                        }

                        if (isEditor)
                        {
                            if (editorIsFix)
                            {
                                var width = TranslateUtils.ToInt(editorFixWidth);
                                var height = TranslateUtils.ToInt(editorFixHeight);
                                ImageUtils.MakeThumbnail(filePath, editorFixFilePath, width, height, true);
                            }
                        }
                    }

                    var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, filePath, true);
                    var fixImageUrl = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, fixFilePath, true);
                    var editorFixImageUrl = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, editorFixFilePath, true);

                    retval.Add(isFix ? fixImageUrl : imageUrl);

                    editors.Add(new
                    {
                        ImageUrl = isFix ? editorFixImageUrl : imageUrl,
                        OriginalUrl = imageUrl
                    });
                }

                var changed = false;
                if (siteInfo.ConfigImageIsFix != isFix)
                {
                    changed = true;
                    siteInfo.ConfigImageIsFix = isFix;
                }
                if (siteInfo.ConfigImageFixWidth != fixWidth)
                {
                    changed = true;
                    siteInfo.ConfigImageFixWidth = fixWidth;
                }
                if (siteInfo.ConfigImageFixHeight != fixHeight)
                {
                    changed = true;
                    siteInfo.ConfigImageFixHeight = fixHeight;
                }
                if (siteInfo.ConfigImageIsEditor != isEditor)
                {
                    changed = true;
                    siteInfo.ConfigImageIsEditor = isEditor;
                }
                if (siteInfo.ConfigImageEditorIsFix != editorIsFix)
                {
                    changed = true;
                    siteInfo.ConfigImageEditorIsFix = editorIsFix;
                }
                if (siteInfo.ConfigImageEditorFixWidth != editorFixWidth)
                {
                    changed = true;
                    siteInfo.ConfigImageEditorFixWidth = editorFixWidth;
                }
                if (siteInfo.ConfigImageEditorFixHeight != editorFixHeight)
                {
                    changed = true;
                    siteInfo.ConfigImageEditorFixHeight = editorFixHeight;
                }
                if (siteInfo.ConfigImageEditorIsLinkToOriginal != editorIsLinkToOriginal)
                {
                    changed = true;
                    siteInfo.ConfigImageEditorIsLinkToOriginal = editorIsLinkToOriginal;
                }

                if (changed)
                {
                    DataProvider.SiteDao.Update(siteInfo);
                }

                return Ok(new
                {
                    Value = retval,
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
