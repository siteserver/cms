using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Images;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
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
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                return Ok(new
                {
                    Value = site.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var path = string.Empty;
                var url = string.Empty;
                var contentLength = 0;

                if (request.HttpRequest.Files.Count > 0)
                {
                    var file = request.HttpRequest.Files[0];

                    var filePath = file.FileName;
                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = await PathUtility.GetUploadDirectoryPathAsync(site, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(site, filePath);
                    path = PathUtils.Combine(localDirectoryPath, localFileName);
                    contentLength = file.ContentLength;

                    if (!PathUtility.IsImageExtensionAllowed(site, fileExtName))
                    {
                        return BadRequest("上传失败，上传图片格式不正确！");
                    }
                    if (!PathUtility.IsImageSizeAllowed(site, contentLength))
                    {
                        return BadRequest("上传失败，上传图片超出规定文件大小！");
                    }

                    file.SaveAs(path);
                    await FileUtility.AddWaterMarkAsync(site, path);

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
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

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
                var filePaths = Utilities.GetStringList(request.GetPostString("filePaths"));

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<string>();
                var editors = new List<object>();

                foreach (var filePath in filePaths)
                {
                    if (string.IsNullOrEmpty(filePath)) continue;

                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var fileName = PathUtility.GetUploadFileName(site, filePath);

                    var directoryPath = await PathUtility.GetUploadDirectoryPathAsync(site, fileExtName);
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
                if (site.ConfigImageIsFix != isFix)
                {
                    changed = true;
                    site.ConfigImageIsFix = isFix;
                }
                if (site.ConfigImageFixWidth != fixWidth)
                {
                    changed = true;
                    site.ConfigImageFixWidth = fixWidth;
                }
                if (site.ConfigImageFixHeight != fixHeight)
                {
                    changed = true;
                    site.ConfigImageFixHeight = fixHeight;
                }
                if (site.ConfigImageIsEditor != isEditor)
                {
                    changed = true;
                    site.ConfigImageIsEditor = isEditor;
                }
                if (site.ConfigImageEditorIsFix != editorIsFix)
                {
                    changed = true;
                    site.ConfigImageEditorIsFix = editorIsFix;
                }
                if (site.ConfigImageEditorFixWidth != editorFixWidth)
                {
                    changed = true;
                    site.ConfigImageEditorFixWidth = editorFixWidth;
                }
                if (site.ConfigImageEditorFixHeight != editorFixHeight)
                {
                    changed = true;
                    site.ConfigImageEditorFixHeight = editorFixHeight;
                }
                if (site.ConfigImageEditorIsLinkToOriginal != editorIsLinkToOriginal)
                {
                    changed = true;
                    site.ConfigImageEditorIsLinkToOriginal = editorIsLinkToOriginal;
                }

                if (changed)
                {
                    await DataProvider.SiteRepository.UpdateAsync(site);
                }

                return Ok(new
                {
                    Value = retVal,
                    Editors = editors
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
