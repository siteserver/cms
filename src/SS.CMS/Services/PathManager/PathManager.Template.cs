using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
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
                filePath = await GetSitePathAsync(site, DirectoryUtils.PublishmentSytem.Template, DirectoryUtils.PublishmentSytem.Content, template.RelatedFileName);
            }
            else
            {
                filePath = await GetSitePathAsync(site, DirectoryUtils.PublishmentSytem.Template, template.RelatedFileName);
            }
            return filePath;
        }

        public async Task WriteContentToTemplateFileAsync(Site site, Template template, string content, int adminId)
        {
            if (content == null) content = string.Empty;
            var filePath = await GetTemplateFilePathAsync(site, template);
            FileUtils.WriteText(filePath, content);

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

        public async Task<string> GetTemplateContentAsync(Site site, Template template)
        {
            var filePath = await GetTemplateFilePathAsync(site, template);
            return await GetContentByFilePathAsync(filePath);
        }

        public async Task<string> GetIncludeContentAsync(Site site, string file)
        {
            var filePath = await MapPathAsync(site, AddVirtualToPath(file));
            return await GetContentByFilePathAsync(filePath);
        }

        public async Task<string> GetContentByFilePathAsync(string filePath)
        {
            try
            {
                var content = CacheUtils.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = await FileUtils.ReadTextAsync(filePath);
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
