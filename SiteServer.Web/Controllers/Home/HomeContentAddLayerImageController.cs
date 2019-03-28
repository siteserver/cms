using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentAddLayerImage")]
    public class HomeContentAddLayerImageController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetQueryInt("siteId");
                var channelId = rest.GetQueryInt("channelId");

                if (!rest.IsUserLoggin ||
                    !rest.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
#pragma warning disable CS0612 // '“RequestImpl”已过时
                var request = new RequestImpl(HttpContext.Current.Request);
#pragma warning restore CS0612 // '“RequestImpl”已过时

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
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var isFix = rest.GetPostBool("isFix");
                var fixWidth = rest.GetPostString("fixWidth");
                var fixHeight = rest.GetPostString("fixHeight");
                var isEditor = rest.GetPostBool("isEditor");
                var editorIsFix = rest.GetPostBool("editorIsFix");
                var editorFixWidth = rest.GetPostString("editorFixWidth");
                var editorFixHeight = rest.GetPostString("editorFixHeight");
                var editorIsLinkToOriginal = rest.GetPostBool("editorIsLinkToOriginal");
                var filePaths = TranslateUtils.StringCollectionToStringList(rest.GetPostString("filePaths"));

                if (!rest.IsUserLoggin ||
                    !rest.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
                    var fixFilePath = PathUtils.Combine(directoryPath, StringUtils.Constants.TitleImageAppendix + fileName);
                    var editorFixFilePath = PathUtils.Combine(directoryPath, StringUtils.Constants.SmallImageAppendix + fileName);

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
                    DataProvider.Site.Update(siteInfo);
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
