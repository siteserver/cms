using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesSpecial")]
    public partial class TemplatesSpecialController : ControllerBase
    {
        private const string Route = "";
        private const string RouteId = "{siteId:int}/{specialId:int}";
        private const string RouteDownload = "actions/download";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public TemplatesSpecialController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var specialInfoList = await DataProvider.SpecialRepository.GetSpecialListAsync(request.SiteId);

            return new ListResult
            {
                Specials = specialInfoList,
                SiteUrl = await PageUtility.GetSiteUrlAsync(site, true)
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody]SpecialIdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var specialInfo = await DataProvider.SpecialRepository.DeleteSpecialAsync(site, request.SpecialId);

            await auth.AddSiteLogAsync(request.SiteId,
                "删除专题",
                $"专题名称:{specialInfo.Title}");

            var specialInfoList = await DataProvider.SpecialRepository.GetSpecialListAsync(request.SiteId);

            return new DeleteResult
            {
                Specials = specialInfoList
            };
        }

        [HttpPost, Route(RouteDownload)]
        public async Task<ActionResult<StringResult>> Download([FromBody]SpecialIdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var specialInfo = await DataProvider.SpecialRepository.GetSpecialAsync(request.SiteId, request.SpecialId);

            var directoryPath = await DataProvider.SpecialRepository.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
            var srcDirectoryPath = DataProvider.SpecialRepository.GetSpecialSrcDirectoryPath(directoryPath);
            var zipFilePath = DataProvider.SpecialRepository.GetSpecialZipFilePath(specialInfo.Title, directoryPath);

            FileUtils.DeleteFileIfExists(zipFilePath);
            ZipUtils.CreateZip(zipFilePath, srcDirectoryPath);
            var url = await DataProvider.SpecialRepository.GetSpecialZipFileUrlAsync(site, specialInfo);

            return new StringResult
            {
                Value = url
            };
        }

        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<GetSpecialResult>> GetSpecial(int siteId, int specialId)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
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
        public async Task<ActionResult<StringResult>> SpecialUpload([FromBody] UploadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            if (request.File == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(request.File.FileName);

            var filePath = PathUtility.GetTemporaryFilesPath($"{request.Guid}/{fileName}");
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            request.File.CopyTo(new FileStream(filePath, FileMode.Create));

            return new StringResult
            {
                Value = fileName
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<ObjectResult<IEnumerable<Special>>>> SpecialSubmit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Specials))
            {
                return Unauthorized();
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
                    return this.Error("专题修改失败，专题名称已存在！");
                }
                if (specialInfo.Url != request.Url)
                {
                    if (await DataProvider.SpecialRepository.IsUrlExistsAsync(request.SiteId, request.Url))
                    {
                        return this.Error("专题修改失败，专题访问地址已存在！");
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
                    return this.Error("专题添加失败，专题名称已存在！");
                }
                if (await DataProvider.SpecialRepository.IsUrlExistsAsync(request.SiteId, request.Url))
                {
                    return this.Error("专题添加失败，专题访问地址已存在！");
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
