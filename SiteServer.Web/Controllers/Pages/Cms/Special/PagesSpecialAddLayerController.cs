using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Special
{
    
    [RoutePrefix("pages/cms/specialAddLayer")]
    public class PagesSpecialAddLayerController : ApiController
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
                var specialId = request.GetQueryInt("specialId");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.WebSitePermissions.Template))
                {
                    return Unauthorized();
                }

                Abstractions.Special special = null;
                if (specialId > 0)
                {
                    special = await SpecialManager.GetSpecialAsync(siteId, specialId);
                }

                return Ok(new
                {
                    Value = special,
                    Guid = StringUtils.GetShortGuid(false),
                });
            }
            catch (Exception ex)
            {
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
                var guid = request.GetQueryString("guid");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.WebSitePermissions.Template))
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
                var guid = request.GetPostString("guid");
                var specialId = request.GetPostInt("specialId");
                var isEditOnly = request.GetPostBool("isEditOnly");
                var isUploadOnly = request.GetPostBool("isUploadOnly");
                var title = request.GetPostString("title");
                var url = request.GetPostString("url");
                var fileNames = StringUtils.GetStringList(request.GetPostString("fileNames"));
                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.WebSitePermissions.Template))
                {
                    return Unauthorized();
                }

                if (specialId > 0 && isEditOnly)
                {
                    var specialInfo = await SpecialManager.GetSpecialAsync(siteId, specialId);
                    var oldDirectoryPath = string.Empty;
                    var newDirectoryPath = string.Empty;

                    if (specialInfo.Title != title && await DataProvider.SpecialRepository.IsTitleExistsAsync(siteId, title))
                    {
                        return BadRequest("专题修改失败，专题名称已存在！");
                    }
                    if (specialInfo.Url != url)
                    {
                        if (await DataProvider.SpecialRepository.IsUrlExistsAsync(siteId, url))
                        {
                            return BadRequest("专题修改失败，专题访问地址已存在！");
                        }

                        oldDirectoryPath = SpecialManager.GetSpecialDirectoryPath(site, specialInfo.Url);
                        newDirectoryPath = SpecialManager.GetSpecialDirectoryPath(site, url);
                    }

                    specialInfo.Title = title;
                    specialInfo.Url = url;
                    await DataProvider.SpecialRepository.UpdateAsync(specialInfo);

                    if (oldDirectoryPath != newDirectoryPath)
                    {
                        DirectoryUtils.MoveDirectory(oldDirectoryPath, newDirectoryPath, true);
                    }
                }
                else if (specialId > 0 && isUploadOnly)
                {
                    var specialInfo = await SpecialManager.GetSpecialAsync(siteId, specialId);

                    var directoryPath = SpecialManager.GetSpecialDirectoryPath(site, specialInfo.Url);
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
                    if (await DataProvider.SpecialRepository.IsTitleExistsAsync(siteId, title))
                    {
                        return BadRequest("专题添加失败，专题名称已存在！");
                    }
                    if (await DataProvider.SpecialRepository.IsUrlExistsAsync(siteId, url))
                    {
                        return BadRequest("专题添加失败，专题访问地址已存在！");
                    }

                    var directoryPath = SpecialManager.GetSpecialDirectoryPath(site, url);
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

                    specialId = await DataProvider.SpecialRepository.InsertAsync(new Abstractions.Special
                    {
                        Id = 0,
                        SiteId = siteId,
                        Title = title,
                        Url = url,
                        AddDate = DateTime.Now
                    });

                    await request.AddSiteLogAsync(siteId, "新建专题", $"专题名称:{title}");
                }

                await CreateManager.CreateSpecialAsync(siteId, specialId);

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
