using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentAddLayerImage")]
    public class HomeContentAddLayerImageController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
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

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                return Ok(new
                {
                    Value = site.Additional
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
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

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var path = string.Empty;
                var url = string.Empty;
                var contentLength = 0;

                if (request.HttpRequest.Files.Count > 0)
                {
                    var file = request.HttpRequest.Files[0];

                    var filePath = file.FileName;
                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(site, filePath);
                    path = PathUtils.Combine(localDirectoryPath, localFileName);
                    contentLength = file.ContentLength;

                    if (!PathUtility.IsImageExtenstionAllowed(site, fileExtName))
                    {
                        return BadRequest("上传失败，上传图片格式不正确！");
                    }
                    if (!PathUtility.IsImageSizeAllowed(site, contentLength))
                    {
                        return BadRequest("上传失败，上传图片超出规定文件大小！");
                    }

                    file.SaveAs(path);
                    FileUtility.AddWaterMark(site, path);

                    url = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, path, true);
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
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

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

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<string>();
                var editors = new List<object>();

                foreach (var filePath in filePaths)
                {
                    if (string.IsNullOrEmpty(filePath)) continue;

                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var fileName = PathUtility.GetUploadFileName(site, filePath);

                    var directoryPath = PathUtility.GetUploadDirectoryPath(site, fileExtName);
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

                    var imageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
                    var fixImageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, fixFilePath, true);
                    var editorFixImageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, editorFixFilePath, true);

                    retVal.Add(isFix ? fixImageUrl : imageUrl);

                    editors.Add(new
                    {
                        ImageUrl = isFix ? editorFixImageUrl : imageUrl,
                        OriginalUrl = imageUrl
                    });
                }

                var changed = false;
                if (site.Additional.ConfigImageIsFix != isFix)
                {
                    changed = true;
                    site.Additional.ConfigImageIsFix = isFix;
                }
                if (site.Additional.ConfigImageFixWidth != fixWidth)
                {
                    changed = true;
                    site.Additional.ConfigImageFixWidth = fixWidth;
                }
                if (site.Additional.ConfigImageFixHeight != fixHeight)
                {
                    changed = true;
                    site.Additional.ConfigImageFixHeight = fixHeight;
                }
                if (site.Additional.ConfigImageIsEditor != isEditor)
                {
                    changed = true;
                    site.Additional.ConfigImageIsEditor = isEditor;
                }
                if (site.Additional.ConfigImageEditorIsFix != editorIsFix)
                {
                    changed = true;
                    site.Additional.ConfigImageEditorIsFix = editorIsFix;
                }
                if (site.Additional.ConfigImageEditorFixWidth != editorFixWidth)
                {
                    changed = true;
                    site.Additional.ConfigImageEditorFixWidth = editorFixWidth;
                }
                if (site.Additional.ConfigImageEditorFixHeight != editorFixHeight)
                {
                    changed = true;
                    site.Additional.ConfigImageEditorFixHeight = editorFixHeight;
                }
                if (site.Additional.ConfigImageEditorIsLinkToOriginal != editorIsLinkToOriginal)
                {
                    changed = true;
                    site.Additional.ConfigImageEditorIsLinkToOriginal = editorIsLinkToOriginal;
                }

                if (changed)
                {
                    await DataProvider.SiteDao.UpdateAsync(site);
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
