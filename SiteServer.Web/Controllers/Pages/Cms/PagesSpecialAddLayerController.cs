using System;
using System.IO;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/specialAddLayer")]
    public class PagesSpecialAddLayerController : ApiController
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
                var specialId = request.GetQueryInt("specialId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                SpecialInfo specialInfo = null;
                if (specialId > 0)
                {
                    specialInfo = SpecialManager.GetSpecialInfo(siteId, specialId);
                }

                return Ok(new
                {
                    Value = specialInfo,
                    Guid = StringUtils.GetShortGuid(false),
                });
            }
            catch (Exception ex)
            {
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
                var guid = request.GetQueryString("guid");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
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

                    filePath = PathUtils.GetTemporaryFilesPath($"{guid}/{fileName}");
                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    file.SaveAs(filePath);
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
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var guid = request.GetPostString("guid");
                var specialId = request.GetPostInt("specialId");
                var isEditOnly = request.GetPostBool("isEditOnly");
                var isUploadOnly = request.GetPostBool("isUploadOnly");
                var title = request.GetPostString("title");
                var url = request.GetPostString("url");
                var fileNames = TranslateUtils.StringCollectionToStringList(request.GetPostString("fileNames"));
                var siteInfo = SiteManager.GetSiteInfo(siteId);

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                if (specialId > 0 && isEditOnly)
                {
                    var specialInfo = SpecialManager.GetSpecialInfo(siteId, specialId);
                    var oldDirectoryPath = string.Empty;
                    var newDirectoryPath = string.Empty;

                    if (specialInfo.Title != title && DataProvider.SpecialDao.IsTitleExists(siteId, title))
                    {
                        return BadRequest("专题修改失败，专题名称已存在！");
                    }
                    if (specialInfo.Url != url)
                    {
                        if (DataProvider.SpecialDao.IsUrlExists(siteId, url))
                        {
                            return BadRequest("专题修改失败，专题访问地址已存在！");
                        }

                        oldDirectoryPath = SpecialManager.GetSpecialDirectoryPath(siteInfo, specialInfo.Url);
                        newDirectoryPath = SpecialManager.GetSpecialDirectoryPath(siteInfo, url);
                    }

                    specialInfo.Title = title;
                    specialInfo.Url = url;
                    DataProvider.SpecialDao.Update(specialInfo);

                    if (oldDirectoryPath != newDirectoryPath)
                    {
                        DirectoryUtils.MoveDirectory(oldDirectoryPath, newDirectoryPath, true);
                    }
                }
                else if (specialId > 0 && isUploadOnly)
                {
                    var specialInfo = SpecialManager.GetSpecialInfo(siteId, specialId);

                    var directoryPath = SpecialManager.GetSpecialDirectoryPath(siteInfo, specialInfo.Url);
                    var srcDirectoryPath = SpecialManager.GetSpecialSrcDirectoryPath(directoryPath);
                    DirectoryUtils.CreateDirectoryIfNotExists(srcDirectoryPath);

                    var uploadDirectoryPath = PathUtils.GetTemporaryFilesPath(guid);
                    foreach (var filePath in DirectoryUtils.GetFilePaths(uploadDirectoryPath))
                    {
                        var fileName = PathUtils.GetFileName(filePath);
                        if (!StringUtils.ContainsIgnoreCase(fileNames, fileName)) continue;

                        if (EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(filePath)))
                        {
                            ZipUtils.ExtractZip(filePath, srcDirectoryPath);
                        }
                        else
                        {
                            FileUtils.MoveFile(filePath, PathUtils.Combine(srcDirectoryPath, fileName), true);
                        }
                    }

                    DirectoryUtils.Copy(srcDirectoryPath, directoryPath);
                }
                else if (specialId == 0)
                {
                    if (DataProvider.SpecialDao.IsTitleExists(siteId, title))
                    {
                        return BadRequest("专题添加失败，专题名称已存在！");
                    }
                    if (DataProvider.SpecialDao.IsUrlExists(siteId, url))
                    {
                        return BadRequest("专题添加失败，专题访问地址已存在！");
                    }

                    var directoryPath = SpecialManager.GetSpecialDirectoryPath(siteInfo, url);
                    var srcDirectoryPath = SpecialManager.GetSpecialSrcDirectoryPath(directoryPath);
                    DirectoryUtils.CreateDirectoryIfNotExists(srcDirectoryPath);

                    var uploadDirectoryPath = PathUtils.GetTemporaryFilesPath(guid);
                    foreach (var filePath in DirectoryUtils.GetFilePaths(uploadDirectoryPath))
                    {
                        var fileName = PathUtils.GetFileName(filePath);
                        if (!StringUtils.ContainsIgnoreCase(fileNames, fileName)) continue;

                        if (EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(filePath)))
                        {
                            ZipUtils.ExtractZip(filePath, srcDirectoryPath);
                        }
                        else
                        {
                            FileUtils.MoveFile(filePath, PathUtils.Combine(srcDirectoryPath, fileName), true);
                        }
                    }

                    DirectoryUtils.Copy(srcDirectoryPath, directoryPath);

                    specialId = DataProvider.SpecialDao.Insert(new SpecialInfo
                    {
                        Id = 0,
                        SiteId = siteId,
                        Title = title,
                        Url = url,
                        AddDate = DateTime.Now
                    });

                    request.AddSiteLog(siteId, "新建专题", $"专题名称:{title}");
                }

                CreateManager.CreateSpecial(siteId, specialId);

                return Ok(new
                {
                    Value = specialId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
