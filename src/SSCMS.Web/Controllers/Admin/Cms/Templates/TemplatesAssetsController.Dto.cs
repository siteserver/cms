using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        public class GetResult
        {
            public List<Cascade<string>> Directories { get; set; }
            public List<KeyValuePair<string, string>> Files { get; set; }
            public string SiteUrl { get; set; }
            public string IncludeDir { get; set; }
            public string CssDir { get; set; }
            public string JsDir { get; set; }
        }

        public class FileRequest
        {
            public int SiteId { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        public class ConfigRequest
        {
            public int SiteId { get; set; }
            public string IncludeDir { get; set; }
            public string CssDir { get; set; }
            public string JsDir { get; set; }
        }

        private async Task GetDirectoriesAndFilesAsync(List<Cascade<string>> directories, List<KeyValuePair<string, string>> files, Site site, string virtualPath, string extName)
        {
            var directoryPath = await _pathManager.GetSitePathAsync(site, virtualPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            var fileNames = DirectoryUtils.GetFileNames(directoryPath);
            foreach (var fileName in fileNames)
            {
                if (StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(fileName), extName))
                {
                    files.Add(new KeyValuePair<string, string>(virtualPath, fileName));
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
                await GetDirectoriesAndFilesAsync(dir.Children, files, site, PageUtils.Combine(virtualPath, directoryName), extName);
            }

            directories.Add(dir);
        }
    }
}
