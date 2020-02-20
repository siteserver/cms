using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    
    [RoutePrefix("pages/cms/templates/templatesSpecial")]
    public partial class PagesSpecialController : ApiController
    {
        private const string Route = "";
        private const string RouteId = "{siteId:int}/{specialId:int}";
        private const string RouteDownload = "actions/download";
        private const string RouteUpload = "actions/upload";

        private readonly ICreateManager _createManager;

        public PagesSpecialController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> List()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var specialInfoList = await DataProvider.SpecialRepository.GetSpecialListAsync(siteId);

                return Ok(new
                {
                    Value = specialInfoList,
                    SiteUrl = PageUtility.GetSiteUrlAsync(site, true)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.GetPostInt("siteId");
                var specialId = request.GetPostInt("specialId");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var specialInfo = await DataProvider.SpecialRepository.DeleteSpecialAsync(site, specialId);

                await request.AddSiteLogAsync(siteId,
                    "删除专题",
                    $"专题名称:{specialInfo.Title}");

                var specialInfoList = await DataProvider.SpecialRepository.GetSpecialListAsync(siteId);

                return Ok(new
                {
                    Value = specialInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteDownload)]
        public async Task<IHttpActionResult> Download()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetPostInt("siteId");
                var specialId = request.GetPostInt("specialId");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var specialInfo = await DataProvider.SpecialRepository.GetSpecialAsync(siteId, specialId);

                var directoryPath = await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
                var srcDirectoryPath = DataProvider.SpecialRepository.GetSpecialSrcDirectoryPath(directoryPath);
                var zipFilePath = DataProvider.SpecialRepository.GetSpecialZipFilePath(specialInfo.Title, directoryPath);

                FileUtils.DeleteFileIfExists(zipFilePath);
                ZipUtils.CreateZip(zipFilePath, srcDirectoryPath);
                var url = await DataProvider.SpecialRepository.GetSpecialZipFileUrlAsync(site, specialInfo);

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteId)]
        public async Task<GetSpecialResult> GetSpecial(int siteId, int specialId)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Specials))
            {
                return Request.Unauthorized<GetSpecialResult>();
            }

            Special special = null;
            if (specialId > 0)
            {
                special = await DataProvider.SpecialRepository.GetSpecialAsync(siteId, specialId);
            }

            return new GetSpecialResult
            {
                Special = special,
                Guid = StringUtils.GetShortGuid(false),
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<StringResult> SpecialUpload([FromUri] UploadRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Request.Unauthorized<StringResult>();
            }

            var fileName = auth.HttpRequest["fileName"];
            var file = auth.HttpRequest.Files[0];

            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var filePath = PathUtility.GetTemporaryFilesPath($"{request.Guid}/{fileName}");
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            return new StringResult
            {
                Value = fileName
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ObjectResult<IEnumerable<Special>>> SpecialSubmit([FromBody]SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Request.Unauthorized<ObjectResult<IEnumerable<Special>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var specialId = request.Id;

            if (specialId > 0 && request.IsEditOnly)
            {
                var specialInfo = await DataProvider.SpecialRepository.GetSpecialAsync(request.SiteId, specialId);
                var oldDirectoryPath = string.Empty;
                var newDirectoryPath = string.Empty;

                if (specialInfo.Title != request.Title && await DataProvider.SpecialRepository.IsTitleExistsAsync(request.SiteId, request.Title))
                {
                    return Request.BadRequest<ObjectResult<IEnumerable<Special>>>("专题修改失败，专题名称已存在！");
                }
                if (specialInfo.Url != request.Url)
                {
                    if (await DataProvider.SpecialRepository.IsUrlExistsAsync(request.SiteId, request.Url))
                    {
                        return Request.BadRequest<ObjectResult<IEnumerable<Special>>>("专题修改失败，专题访问地址已存在！");
                    }

                    oldDirectoryPath = await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
                    newDirectoryPath = await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, request.Url);
                }

                specialInfo.Title = request.Title;
                specialInfo.Url = request.Url;
                await DataProvider.SpecialRepository.UpdateAsync(specialInfo);

                if (oldDirectoryPath != newDirectoryPath)
                {
                    DirectoryUtils.MoveDirectory(oldDirectoryPath, newDirectoryPath, true);
                }
            }
            else if (specialId > 0 && request.IsUploadOnly)
            {
                var specialInfo = await DataProvider.SpecialRepository.GetSpecialAsync(request.SiteId, specialId);

                var directoryPath = await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
                var srcDirectoryPath = DataProvider.SpecialRepository.GetSpecialSrcDirectoryPath(directoryPath);
                DirectoryUtils.CreateDirectoryIfNotExists(srcDirectoryPath);

                var uploadDirectoryPath = PathUtility.GetTemporaryFilesPath(request.Guid);
                foreach (var filePath in DirectoryUtils.GetFilePaths(uploadDirectoryPath))
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!StringUtils.ContainsIgnoreCase(request.FileNames, fileName)) continue;

                    if (FileUtils.IsZip(PathUtils.GetExtension(filePath)))
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
                if (await DataProvider.SpecialRepository.IsTitleExistsAsync(request.SiteId, request.Title))
                {
                    return Request.BadRequest<ObjectResult<IEnumerable<Special>>>("专题添加失败，专题名称已存在！");
                }
                if (await DataProvider.SpecialRepository.IsUrlExistsAsync(request.SiteId, request.Url))
                {
                    return Request.BadRequest<ObjectResult<IEnumerable<Special>>>("专题添加失败，专题访问地址已存在！");
                }

                var directoryPath = await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, request.Url);
                var srcDirectoryPath = DataProvider.SpecialRepository.GetSpecialSrcDirectoryPath(directoryPath);
                DirectoryUtils.CreateDirectoryIfNotExists(srcDirectoryPath);

                var uploadDirectoryPath = PathUtility.GetTemporaryFilesPath(request.Guid);
                foreach (var filePath in DirectoryUtils.GetFilePaths(uploadDirectoryPath))
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!StringUtils.ContainsIgnoreCase(request.FileNames, fileName)) continue;

                    if (FileUtils.IsZip(PathUtils.GetExtension(filePath)))
                    {
                        ZipUtils.ExtractZip(filePath, srcDirectoryPath);
                    }
                    else
                    {
                        FileUtils.MoveFile(filePath, PathUtils.Combine(srcDirectoryPath, fileName), true);
                    }
                }

                DirectoryUtils.Copy(srcDirectoryPath, directoryPath);

                specialId = await DataProvider.SpecialRepository.InsertAsync(new Special
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    Title = request.Title,
                    Url = request.Url,
                    AddDate = DateTime.Now
                });

                await auth.AddSiteLogAsync(request.SiteId, "新建专题", $"专题名称:{request.Title}");
            }

            await _createManager.CreateSpecialAsync(request.SiteId, specialId);

            var specialInfoList = await DataProvider.SpecialRepository.GetSpecialListAsync(request.SiteId);

            return new ObjectResult<IEnumerable<Special>>
            {
                Value = specialInfoList
            };
        }
    }
}
