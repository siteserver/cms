using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesAssetsController : ControllerBase
    {
        private const string Route = "cms/templates/templatesAssets";
        private const string RouteConfig = "cms/templates/templatesAssets/actions/config";

        private const string ExtInclude = "html";
        private const string ExtCss = "css";
        private const string ExtJs = "js";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public TemplatesAssetsController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string FileType { get; set; }
        }

        public class GetResult
        {
            public List<Cascade<string>> Directories { get; set; }
            public List<AssetFile> Files { get; set; }
            public string SiteUrl { get; set; }
            public string IncludeDir { get; set; }
            public string CssDir { get; set; }
            public string JsDir { get; set; }
        }

        public class FileRequest
        {
            public int SiteId { get; set; }
            public string FileType { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        public class ConfigRequest
        {
            public int SiteId { get; set; }
            public string FileType { get; set; }
            public string IncludeDir { get; set; }
            public string CssDir { get; set; }
            public string JsDir { get; set; }
        }

        public class AssetFile
        {
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
            public string FileType { get; set; }
        }

        private async Task GetDirectoriesAndFilesAsync(List<Cascade<string>> directories, List<AssetFile> files, Site site, string virtualPath, string fileType)
        {
            var extName = "." + fileType;
            var directoryPath = await _pathManager.GetSitePathAsync(site, virtualPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            var fileNames = DirectoryUtils.GetFileNames(directoryPath);
            foreach (var fileName in fileNames)
            {
                if (StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(fileName), extName))
                {
                    files.Add(new AssetFile
                    {
                        DirectoryPath = virtualPath,
                        FileName = fileName,
                        FileType = fileType
                    });
                }
            }

            var dir = new Cascade<string>
            {
                Label = PathUtils.GetDirectoryName(directoryPath, false),
                Value = virtualPath
            };

            var children = DirectoryUtils.GetDirectoryNames(directoryPath);
            dir.Children = new List<Cascade<string>>();
            foreach (var directoryName in children)
            {
                await GetDirectoriesAndFilesAsync(dir.Children, files, site, PageUtils.Combine(virtualPath, directoryName), fileType);
            }

            directories.Add(dir);
        }
    }
}
