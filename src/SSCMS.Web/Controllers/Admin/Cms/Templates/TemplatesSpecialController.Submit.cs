using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialId = request.Id;

            if (specialId > 0 && request.IsEditOnly)
            {
                var specialInfo = await _specialRepository.GetSpecialAsync(request.SiteId, specialId);
                var oldDirectoryPath = string.Empty;
                var newDirectoryPath = string.Empty;

                if (specialInfo.Title != request.Title && await _specialRepository.IsTitleExistsAsync(request.SiteId, request.Title))
                {
                    return this.Error("专题修改失败，专题名称已存在！");
                }
                if (specialInfo.Url != request.Url)
                {
                    if (await _specialRepository.IsUrlExistsAsync(request.SiteId, request.Url))
                    {
                        return this.Error("专题修改失败，专题访问地址已存在！");
                    }

                    oldDirectoryPath = await _pathManager.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
                    newDirectoryPath = await _pathManager.GetSpecialDirectoryPathAsync(site, request.Url);
                }

                specialInfo.Title = request.Title;
                specialInfo.Url = request.Url;
                await _specialRepository.UpdateAsync(specialInfo);

                if (oldDirectoryPath != newDirectoryPath)
                {
                    DirectoryUtils.MoveDirectory(oldDirectoryPath, newDirectoryPath, true);
                }
            }
            else if (specialId > 0 && request.IsUploadOnly)
            {
                var specialInfo = await _specialRepository.GetSpecialAsync(request.SiteId, specialId);

                var directoryPath = await _pathManager.GetSpecialDirectoryPathAsync(site, specialInfo.Url);
                var srcDirectoryPath = _pathManager.GetSpecialSrcDirectoryPath(directoryPath);
                DirectoryUtils.CreateDirectoryIfNotExists(srcDirectoryPath);

                var uploadDirectoryPath = _pathManager.GetTemporaryFilesPath(request.Guid);
                foreach (var filePath in DirectoryUtils.GetFilePaths(uploadDirectoryPath))
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!ListUtils.ContainsIgnoreCase(request.FileNames, fileName)) continue;

                    if (FileUtils.IsZip(PathUtils.GetExtension(filePath)))
                    {
                        _pathManager.ExtractZip(filePath, srcDirectoryPath);
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
                if (await _specialRepository.IsTitleExistsAsync(request.SiteId, request.Title))
                {
                    return this.Error("专题添加失败，专题名称已存在！");
                }
                if (await _specialRepository.IsUrlExistsAsync(request.SiteId, request.Url))
                {
                    return this.Error("专题添加失败，专题访问地址已存在！");
                }

                var directoryPath = await _pathManager.GetSpecialDirectoryPathAsync(site, request.Url);
                var srcDirectoryPath = _pathManager.GetSpecialSrcDirectoryPath(directoryPath);
                DirectoryUtils.CreateDirectoryIfNotExists(srcDirectoryPath);

                var uploadDirectoryPath = _pathManager.GetTemporaryFilesPath(request.Guid);
                foreach (var filePath in DirectoryUtils.GetFilePaths(uploadDirectoryPath))
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!ListUtils.ContainsIgnoreCase(request.FileNames, fileName)) continue;

                    if (FileUtils.IsZip(PathUtils.GetExtension(filePath)))
                    {
                        _pathManager.ExtractZip(filePath, srcDirectoryPath);
                    }
                    else
                    {
                        FileUtils.MoveFile(filePath, PathUtils.Combine(srcDirectoryPath, fileName), true);
                    }
                }

                DirectoryUtils.Copy(srcDirectoryPath, directoryPath);

                specialId = await _specialRepository.InsertAsync(new Special
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    Title = request.Title,
                    Url = request.Url,
                    AddDate = DateTime.Now
                });

                await _authManager.AddSiteLogAsync(request.SiteId, "新建专题", $"专题名称:{request.Title}");
            }

            await _createManager.CreateSpecialAsync(request.SiteId, specialId);

            var specials = await _specialRepository.GetSpecialsAsync(request.SiteId);
            foreach (var special in specials)
            {
                var filePath = PathUtils.Combine(await _pathManager.GetSpecialDirectoryPathAsync(site, special.Url), "index.html");
                special.Set("editable", FileUtils.IsFileExists(filePath));
            }

            return new SubmitResult
            {
                Specials = specials
            };
        }
    }
}