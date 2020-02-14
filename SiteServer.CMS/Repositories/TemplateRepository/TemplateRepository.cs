using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

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

        public async Task<int> InsertAsync(Site site, Template template, string templateContent, int adminId)
        {
            if (template.Default)
            {
                var defaultTemplate = await GetDefaultTemplateAsync(site.Id, template.TemplateType);
                if (defaultTemplate != null)
                {
                    defaultTemplate.Default = false;
                    await _repository.UpdateAsync(defaultTemplate, Q
                        .CachingRemove(GetListKey(site.Id, defaultTemplate.TemplateType))
                        .CachingRemove(GetEntityKey(defaultTemplate.Id))
                    );
                }
            }

            template.Id = await _repository.InsertAsync(template, Q
                .CachingRemove(GetListKey(site.Id, template.TemplateType))
            );

            await WriteContentToTemplateFileAsync(site, template, templateContent, adminId);

            return template.Id;
        }

        public async Task UpdateAsync(Site site, Template template, string templateContent, int adminId)
        {
            var original = await GetAsync(template.Id);
            if (original.Default != template.Default && template.Default)
            {
                var defaultTemplate = await GetDefaultTemplateAsync(site.Id, template.TemplateType);
                if (defaultTemplate != null)
                {
                    defaultTemplate.Default = false;
                    await _repository.UpdateAsync(defaultTemplate, Q
                        .CachingRemove(GetListKey(site.Id, defaultTemplate.TemplateType))
                        .CachingRemove(GetEntityKey(defaultTemplate.Id))
                    );
                }
            }

            await _repository.UpdateAsync(template, Q
                .CachingRemove(GetListKey(site.Id, template.TemplateType))
                .CachingRemove(GetEntityKey(template.Id))
            );

            await WriteContentToTemplateFileAsync(site, template, templateContent, adminId);
        }

        //private async Task SetAllTemplateDefaultToFalseAsync(int siteId, TemplateType templateType)
        //{
        //    await _repository.UpdateAsync(Q
        //        .Set(nameof(Template.Default), false)
        //        .Where(nameof(Template.SiteId), siteId)
        //        .Where(nameof(Template.TemplateType), templateType.GetValue())
        //        .CachingRemove(GetListKey(siteId))
        //    );
        //}

        public async Task SetDefaultAsync(int templateId)
        {
            var template = await GetAsync(templateId);

            var defaultTemplate = await GetDefaultTemplateAsync(template.SiteId, template.TemplateType);
            if (defaultTemplate != null && defaultTemplate.Id != templateId)
            {
                defaultTemplate.Default = false;
                await _repository.UpdateAsync(defaultTemplate, Q
                    .CachingRemove(GetListKey(template.SiteId, defaultTemplate.TemplateType))
                    .CachingRemove(GetEntityKey(defaultTemplate.Id))
                );
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Template.Default), true)
                .Where(nameof(Template.Id), templateId)
                .CachingRemove(GetListKey(template.SiteId, template.TemplateType))
                .CachingRemove(GetEntityKey(template.Id))
            );
        }

        public async Task DeleteAsync(Site site, int templateId)
        {
            var template = await GetAsync(templateId);
            var filePath = await GetTemplateFilePathAsync(site, template);

            await _repository.DeleteAsync(templateId, Q
                .CachingRemove(GetListKey(template.SiteId, template.TemplateType))
                .CachingRemove(GetEntityKey(template.Id))
            );
            FileUtils.DeleteFileIfExists(filePath);
        }

        public async Task CreateDefaultTemplateAsync(Site site, int adminId)
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
                await InsertAsync(site, theTemplate, theTemplate.Content, adminId);
            }
        }
    }
}
