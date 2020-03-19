using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Services.PathManager
{
    public partial class PathManager
    {
        public async Task<string> GetSpecialUrlAsync(Site site, int specialId, bool isLocal)
        {
            var specialUrl = await GetSpecialUrlAsync(site, specialId);

            var url = isLocal
                ? GetLocalSpecialUrl(site.Id, specialId)
                : await ParseNavigationUrlAsync(site, specialUrl, false);

            return RemoveDefaultFileName(site, url);
        }

        public async Task<string> GetSpecialDirectoryPathAsync(Site site, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            return await MapPathAsync(site, virtualPath);
        }

        private async Task<string> GetSpecialUrlAsync(Site site, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            if (!IsVirtualUrl(virtualPath))
            {
                virtualPath = $"@/{StringUtils.TrimSlash(virtualPath)}";
            }
            return await ParseNavigationUrlAsync(site, virtualPath, false);
        }

        public async Task<string> GetSpecialUrlAsync(Site site, int specialId)
        {
            var special = await _specialRepository.GetSpecialAsync(site.Id, specialId);
            return await GetSpecialUrlAsync(site, special.Url);
        }

        public string GetSpecialZipFilePath(string title, string directoryPath)
        {
            return PathUtils.Combine(directoryPath, $"{title}.zip");
        }

        public async Task<string> GetSpecialZipFileUrlAsync(Site site, Special special)
        {
            return await ParseNavigationUrlAsync(site, $"@/{special.Url}/{special.Title}.zip", true);
        }

        public string GetSpecialSrcDirectoryPath(string directoryPath)
        {
            return PathUtils.Combine(directoryPath, "_src");
        }

        public async Task<Special> DeleteSpecialAsync(Site site, int specialId)
        {
            var special = await _specialRepository.GetSpecialAsync(site.Id, specialId);

            if (!string.IsNullOrEmpty(special.Url) && special.Url != "/")
            {
                var directoryPath = await GetSpecialDirectoryPathAsync(site, special.Url);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            }

            await _specialRepository.DeleteAsync(site.Id, specialId);

            return special;
        }

        public async Task<List<Template>> GetSpecialTemplateListAsync(Site site, int specialId)
        {
            var list = new List<Template>();

            var special = await _specialRepository.GetSpecialAsync(site.Id, specialId);
            if (special != null)
            {
                var directoryPath = await GetSpecialDirectoryPathAsync(site, special.Url);
                var srcDirectoryPath = GetSpecialSrcDirectoryPath(directoryPath);
                if (!DirectoryUtils.IsDirectoryExists(srcDirectoryPath)) return list;

                var htmlFilePaths = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                foreach (var htmlFilePath in htmlFilePaths)
                {
                    var relatedPath = PathUtils.GetPathDifference(srcDirectoryPath, htmlFilePath);

                    var template = new Template
                    {
                        Content = GetSpecialContentByFilePath(htmlFilePath),
                        CreatedFileExtName = ".html",
                        CreatedFileFullName = PathUtils.Combine(special.Url, relatedPath),
                        Id = 0,
                        DefaultTemplate = false,
                        RelatedFileName = string.Empty,
                        SiteId = site.Id,
                        TemplateType = TemplateType.FileTemplate,
                        TemplateName = relatedPath
                    };

                    list.Add(template);
                }
            }

            return list;
        }

        private string GetSpecialContentByFilePath(string filePath)
        {
            try
            {
                var content = CacheUtils.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, Encoding.UTF8);
                }

                CacheUtils.Insert(filePath, content, filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
