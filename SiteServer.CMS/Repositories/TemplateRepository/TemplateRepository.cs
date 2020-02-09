using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class TemplateRepository : DataProviderBase, IRepository
    {
        private readonly Repository<Template> _repository;

        public TemplateRepository()
        {
            _repository = new Repository<Template>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string IsDefault = nameof(IsDefault);
        }

        private string GetListKey(int siteId)
        {
            return Caching.GetListKey(_repository.TableName, siteId);
        }

        private string GetEntityKey(int templateId)
        {
            return Caching.GetEntityKey(_repository.TableName, templateId);
        }

        public async Task<int> InsertAsync(Site site, Template template, string templateContent, string administratorName)
        {
            if (template.Default)
            {
                await SetAllTemplateDefaultToFalseAsync(site.Id, template.TemplateType);
            }

            template.Id = await _repository.InsertAsync(template, Q
                .CachingRemove(GetListKey(site.Id))
            );

            await WriteContentToTemplateFileAsync(site, template, templateContent, administratorName);

            return template.Id;
        }

        public async Task UpdateAsync(Site site, Template template, string templateContent, string administratorName)
        {
            var original = await GetAsync(template.Id);
            if (original.Default != template.Default && template.Default)
            {
                await SetAllTemplateDefaultToFalseAsync(site.Id, template.TemplateType);
            }

            await _repository.UpdateAsync(template, Q
                .CachingRemove(GetListKey(site.Id), GetEntityKey(template.Id))
            );

            await WriteContentToTemplateFileAsync(site, template, templateContent, administratorName);
        }

        private async Task SetAllTemplateDefaultToFalseAsync(int siteId, TemplateType templateType)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.IsDefault, false.ToString())
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
                .CachingRemove(GetListKey(siteId))
            );
        }

        public async Task SetDefaultAsync(int templateId)
        {
            var template = await GetAsync(templateId);
            await SetAllTemplateDefaultToFalseAsync(template.SiteId, template.TemplateType);

            await _repository.UpdateAsync(Q
                .Set(Attr.IsDefault, true.ToString())
                .Where(nameof(Template.Id), templateId)
                .CachingRemove(GetListKey(template.SiteId), GetEntityKey(template.Id))
            );
        }

        public async Task DeleteAsync(Site site, int templateId)
        {
            var template = await GetAsync(templateId);
            var filePath = GetTemplateFilePath(site, template);

            await _repository.DeleteAsync(templateId, Q.
                CachingRemove(GetListKey(site.Id), GetEntityKey(templateId))
            );
            FileUtils.DeleteFileIfExists(filePath);
        }

        public async Task<string> GetImportTemplateNameAsync(int siteId, string templateName)
        {
            string importTemplateName;
            if (templateName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var templateNameCount = 0;
                var lastTemplateName = templateName.Substring(templateName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstTemplateName = templateName.Substring(0, templateName.Length - lastTemplateName.Length);
                try
                {
                    templateNameCount = int.Parse(lastTemplateName);
                }
                catch
                {
                    // ignored
                }
                templateNameCount++;
                importTemplateName = firstTemplateName + templateNameCount;
            }
            else
            {
                importTemplateName = templateName + "_1";
            }

            var exists = await _repository.ExistsAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateName), importTemplateName)
            );
            if (exists)
            {
                importTemplateName = await GetImportTemplateNameAsync(siteId, importTemplateName);
            }

            return importTemplateName;
        }

        public async Task<List<Template>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .OrderBy(nameof(Template.TemplateType), nameof(Template.RelatedFileName))
                .CachingGet(GetListKey(siteId))
            );
        }

        public async Task<Template> GetAsync(int templateId)
        {
            return await _repository.GetAsync(templateId, Q
                .CachingGet(GetEntityKey(templateId))
            );
        }

        public async Task CreateDefaultTemplateAsync(Site site, string administratorName)
        {
            var templateList = new List<Template>();

            var template = new Template
            {
                Id = 0,
                SiteId = site.Id,
                TemplateName = "系统首页模板",
                TemplateType = TemplateType.IndexPageTemplate,
                RelatedFileName = "T_系统首页模板.html",
                CreatedFileFullName = "@/index.html",
                CreatedFileExtName = ".html",
                Default = true
            };
            templateList.Add(template);

            template = new Template
            {
                Id = 0,
                SiteId = site.Id,
                TemplateName = "系统栏目模板",
                TemplateType = TemplateType.ChannelTemplate,
                RelatedFileName = "T_系统栏目模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                Default = true
            };
            templateList.Add(template);

            template = new Template
            {
                Id = 0,
                SiteId = site.Id,
                TemplateName = "系统内容模板",
                TemplateType = TemplateType.ContentTemplate,
                RelatedFileName = "T_系统内容模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                Default = true
            };
            templateList.Add(template);

            foreach (var theTemplate in templateList)
            {
                await InsertAsync(site, theTemplate, theTemplate.Content, administratorName);
            }
        }
    }
}
