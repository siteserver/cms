using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Utils;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Fx;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contentsLayerImport")]
    public class PagesContentsLayerImportController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                var channelId = Request.GetQueryInt("channelId");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = CheckManager.GetUserCheckLevel(rest.AdminPermissions, siteInfo, siteId, out var checkedLevel);
                var checkedLevels = CheckManager.GetCheckedLevels(siteInfo, isChecked, checkedLevel, true);

                return Ok(new
                {
                    Value = checkedLevel,
                    CheckedLevels = checkedLevels
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

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var fileName = request.HttpRequest["fileName"];

                var fileCount = request.HttpRequest.Files.Count;

                string filePath = null;

                if (fileCount > 0)
                {
                    var file = request.HttpRequest.Files[0];

                    if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                    var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                    if (extendName == ".zip" || extendName == ".csv" || extendName == ".txt")
                    {
                        filePath = PathUtils.GetTemporaryFilesPath(fileName);
                        DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                        file.SaveAs(filePath);
                    }
                }

                FileInfo fileInfo = null;
                if (!string.IsNullOrEmpty(filePath))
                {
                    fileInfo = new FileInfo(filePath);
                }
                if (fileInfo != null)
                {
                    return Ok(new
                    {
                        fileName,
                        length = fileInfo.Length,
                        ret = 1
                    });
                }

                return Ok(new
                {
                    ret = 0
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
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                var channelId = Request.GetPostInt("channelId");
                var importType = Request.GetPostString("importType");
                var checkedLevel = Request.GetPostInt("checkedLevel");
                var isOverride = Request.GetPostBool("isOverride");
                var fileNames = Request.GetPostObject<List<string>>("fileNames");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = checkedLevel >= siteInfo.CheckContentLevel;

                if (importType == "zip")
                {
                    foreach (var fileName in fileNames)
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(fileName);

                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Zip, PathUtils.GetExtension(localFilePath)))
                            continue;

                        var importObject = new ImportObject(siteId, rest.AdminName);
                        importObject.ImportContentsByZipFile(channelInfo, localFilePath, isOverride, isChecked, checkedLevel, rest.AdminId, 0, SourceManager.Default);
                    }
                }
                
                else if (importType == "csv")
                {
                    foreach (var fileName in fileNames)
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(fileName);

                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Csv, PathUtils.GetExtension(localFilePath)))
                            continue;

                        var importObject = new ImportObject(siteId, rest.AdminName);
                        importObject.ImportContentsByCsvFile(channelInfo, localFilePath, isOverride, isChecked, checkedLevel, rest.AdminId, 0, SourceManager.Default);
                    }
                }
                else if (importType == "txt")
                {
                    foreach (var fileName in fileNames)
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(fileName);
                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Txt, PathUtils.GetExtension(localFilePath)))
                            continue;

                        var importObject = new ImportObject(siteId, rest.AdminName);
                        importObject.ImportContentsByTxtFile(channelInfo, localFilePath, isOverride, isChecked, checkedLevel, rest.AdminId, 0, SourceManager.Default);
                    }
                }

                LogUtils.AddSiteLog(siteId, channelId, rest.AdminName, "导入内容", string.Empty);

                return Ok(new
                {
                    Value = true
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
