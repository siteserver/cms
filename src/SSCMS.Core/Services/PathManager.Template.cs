using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public async Task<string> GetTemplateFilePathAsync(Site site, Template template)
        {
            string filePath;
            if (template.TemplateType == TemplateType.IndexPageTemplate)
            {
                filePath = await GetSitePathAsync(site, template.RelatedFileName);
            }
            else if (template.TemplateType == TemplateType.ContentTemplate)
            {
                filePath = await GetSitePathAsync(site, DirectoryUtils.Site.Template, DirectoryUtils.Site.Content, template.RelatedFileName);
            }
            else
            {
                filePath = await GetSitePathAsync(site, DirectoryUtils.Site.Template, template.RelatedFileName);
            }
            return filePath;
        }

        public async Task<string> GetTemplateContentAsync(Site site, Template template)
        {
            var filePath = await GetTemplateFilePathAsync(site, template);
            return GetContentByFilePath(filePath);
        }

        public async Task WriteContentToTemplateFileAsync(Site site, Template template, string content, int adminId)
        {
            if (content == null) content = string.Empty;
            var filePath = await GetTemplateFilePathAsync(site, template);
            await FileUtils.WriteTextAsync(filePath, content);

            if (template.Id > 0)
            {
                var logInfo = new TemplateLog
                {
                    Id = 0,
                    TemplateId = template.Id,
                    SiteId = template.SiteId,
                    AdminId = adminId,
                    ContentLength = content.Length,
                    TemplateContent = content
                };
                await _templateLogRepository.InsertAsync(logInfo);
            }
        }

        public async Task<string> GetIncludeContentAsync(Site site, string file)
        {
            var filePath = await ParseSitePathAsync(site, AddVirtualToPath(file));
            return GetContentByFilePath(filePath);
        }

        public async Task WriteContentToIncludeFileAsync(Site site, string file, string content)
        {
            if (content == null) content = string.Empty;
            var filePath = await GetSitePathAsync(site, file);
            await FileUtils.WriteTextAsync(filePath, content);
        }

        public string GetContentByFilePath(string filePath)
        {
            return _cacheManager.GetByFilePath(filePath);
        }
    }
}
