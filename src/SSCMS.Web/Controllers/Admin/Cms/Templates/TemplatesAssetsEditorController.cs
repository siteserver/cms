using System.IO;
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
        private const string RouteUpdate = "cms/templates/templatesAssetsEditor/actions/update";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public TemplatesAssetsEditorController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            ISiteRepository siteRepository
        )
        {
            _settingsManager = settingsManager;
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

        /// <summary>
        /// 根据文件类型获取允许访问的资源根目录名（相对站点目录），不支持的类型返回 null。
        /// </summary>
        private static string GetAssetsDir(Site site, string fileType)
        {
            if (StringUtils.EqualsIgnoreCase(fileType, "html")) return site.TemplatesAssetsIncludeDir;
            if (StringUtils.EqualsIgnoreCase(fileType, "css")) return site.TemplatesAssetsCssDir;
            if (StringUtils.EqualsIgnoreCase(fileType, "js")) return site.TemplatesAssetsJsDir;
            return null;
        }

        /// <summary>
        /// 解析并校验资源文件路径：文件名不得含路径成分，扩展名必须与类型一致，
        /// 规范化后的完整路径必须位于该类型的资源目录内。校验失败返回 null。
        /// </summary>
        private async Task<string> ResolveAssetPathAsync(Site site, string fileType, string directoryPath, string fileName)
        {
            var assetsDir = GetAssetsDir(site, fileType);
            if (string.IsNullOrEmpty(assetsDir) || !AssetPathUtils.IsSafeFileName(fileName)) return null;
            if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(fileName), "." + fileType)) return null;
            if (!AssetPathUtils.IsSafeRelativePath(directoryPath)) return null;

            var rootPath = await _pathManager.GetSitePathAsync(site, assetsDir);
            var filePath = await _pathManager.GetSitePathAsync(site, directoryPath, fileName);
            return AssetPathUtils.IsUnder(rootPath, filePath) ? Path.GetFullPath(filePath) : null;
        }

        private async Task<ActionResult<ContentResult>> SaveFile(ContentRequest request, Site site, bool isAdd)
        {
            var assetsDir = GetAssetsDir(site, request.FileType);
            if (string.IsNullOrEmpty(assetsDir))
            {
                return this.Error("文件保存失败，必须为Html/Css/Js文件！");
            }

            // request.Path 为相对资源目录的文件名（不含扩展名），可包含子目录
            var relativePath = request.Path?.Replace('\\', '/').Trim('/');
            if (string.IsNullOrEmpty(relativePath) || !AssetPathUtils.IsSafeRelativePath(relativePath))
            {
                return this.Error("文件保存失败，路径不正确！");
            }
            var slashIndex = relativePath.LastIndexOf('/');
            var subDirectory = slashIndex >= 0 ? relativePath.Substring(0, slashIndex) : string.Empty;
            var newDirectory = string.IsNullOrEmpty(subDirectory) ? assetsDir : assetsDir.TrimEnd('/') + "/" + subDirectory;
            var newFileName = relativePath.Substring(slashIndex + 1) + "." + request.FileType.ToLower();
            var filePath = await ResolveAssetPathAsync(site, request.FileType, newDirectory, newFileName);
            if (filePath == null)
            {
                return this.Error("文件保存失败，路径不正确！");
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
                // 原文件也必须通过校验，否则可借重命名删除任意文件
                var originalFilePath = await ResolveAssetPathAsync(site, request.FileType, request.DirectoryPath, request.FileName);
                if (originalFilePath == null)
                {
                    return this.Error("文件保存失败，原文件路径不正确！");
                }
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
