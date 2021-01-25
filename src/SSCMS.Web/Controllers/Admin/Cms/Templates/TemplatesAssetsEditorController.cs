using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesAssetsEditorController : ControllerBase
    {
        private const string Route = "cms/templates/templatesAssetsEditor";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public TemplatesAssetsEditorController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
            public string FileType { get; set; }
        }

        public class GetResult
        {
            public string TemplatesAssetsIncludeDir { get; set; }
            public string TemplatesAssetsJsDir { get; set; }
            public string TemplatesAssetsCssDir { get; set; }
            public string Path { get; set; }
            public string Content { get; set; }
        }

        public class ContentRequest
        {
            public int SiteId { get; set; }
            public string Path { get; set; }
            public string FileType { get; set; }
            public string Content { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        public class ContentResult
        {
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        private async Task<ActionResult<ContentResult>> SaveFile(ContentRequest request, Site site, bool isAdd)
        {
            var filePath = string.Empty;
            if (StringUtils.EqualsIgnoreCase(request.FileType, "html"))
            {
                filePath = await _pathManager.GetSitePathAsync(site, site.TemplatesAssetsIncludeDir, request.Path + ".html");
            }
            else if (StringUtils.EqualsIgnoreCase(request.FileType, "css"))
            {
                filePath = await _pathManager.GetSitePathAsync(site, site.TemplatesAssetsCssDir, request.Path + ".css");
            }
            else if (StringUtils.EqualsIgnoreCase(request.FileType, "js"))
            {
                filePath = await _pathManager.GetSitePathAsync(site, site.TemplatesAssetsJsDir, request.Path + ".js");
            }

            var filePathToDelete = string.Empty;
            if (isAdd)
            {
                if (FileUtils.IsFileExists(filePath))
                {
                    return this.Error("文件新增失败，同名文件已存在！");
                }
            }
            else
            {
                var originalFilePath = await _pathManager.GetSitePathAsync(site, request.DirectoryPath, request.FileName);
                if (!StringUtils.EqualsIgnoreCase(originalFilePath, filePath))
                {
                    filePathToDelete = originalFilePath;
                    if (FileUtils.IsFileExists(filePath))
                    {
                        return this.Error("文件编辑失败，同名文件已存在！");
                    }
                }
            }

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            await FileUtils.WriteTextAsync(filePath, request.Content);
            if (!string.IsNullOrEmpty(filePathToDelete))
            {
                FileUtils.DeleteFileIfExists(filePathToDelete);
            }

            var fileName = PathUtils.GetFileName(filePath);
            var sitePath = await _pathManager.GetSitePathAsync(site);
            var directoryPath = StringUtils.ReplaceStartsWithIgnoreCase(filePath, sitePath, string.Empty);
            directoryPath = StringUtils.ReplaceEndsWithIgnoreCase(directoryPath, fileName, string.Empty);
            directoryPath = StringUtils.TrimSlash(directoryPath);

            return new ContentResult
            {
                DirectoryPath = directoryPath,
                FileName = fileName
            };
        }
    }
}
