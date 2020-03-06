using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Repositories
{
    public partial class TemplateRepository : ITemplateRepository
    {
        private readonly Repository<Template> _repository;
        private readonly ITemplateLogRepository _templateLogRepository;

        public TemplateRepository(ISettingsManager settingsManager, ITemplateLogRepository templateLogRepository)
        {
            _repository = new Repository<Template>(settingsManager.Database, settingsManager.Redis);
            _templateLogRepository = templateLogRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(IPathManager pathManager, Site site, Template template, string templateContent, int adminId)
        {
            if (template.DefaultTemplate)
            {
                var defaultTemplate = await GetDefaultTemplateAsync(site.Id, template.TemplateType);
                if (defaultTemplate != null)
                {
                    defaultTemplate.DefaultTemplate = false;
                    await _repository.UpdateAsync(defaultTemplate, Q
                        .CachingRemove(GetListKey(site.Id))
                        .CachingRemove(GetEntityKey(defaultTemplate.Id))
                    );
                }
            }

            template.Id = await _repository.InsertAsync(template, Q
                .CachingRemove(GetListKey(site.Id))
            );

            await pathManager.WriteContentToTemplateFileAsync(site, template, templateContent, adminId);

            return template.Id;
        }

        public async Task UpdateAsync(IPathManager pathManager, Site site, Template template, string templateContent, int adminId)
        {
            var original = await GetAsync(template.Id);
            if (original.DefaultTemplate != template.DefaultTemplate && template.DefaultTemplate)
            {
                var defaultTemplate = await GetDefaultTemplateAsync(site.Id, template.TemplateType);
                if (defaultTemplate != null)
                {
                    defaultTemplate.DefaultTemplate = false;
                    await _repository.UpdateAsync(defaultTemplate, Q
                        .CachingRemove(GetListKey(site.Id))
                        .CachingRemove(GetEntityKey(defaultTemplate.Id))
                    );
                }
            }

            await _repository.UpdateAsync(template, Q
                .CachingRemove(GetListKey(site.Id))
                .CachingRemove(GetEntityKey(template.Id))
            );

            await pathManager.WriteContentToTemplateFileAsync(site, template, templateContent, adminId);
        }

        public async Task SetDefaultAsync(int templateId)
        {
            var template = await GetAsync(templateId);

            var defaultTemplate = await GetDefaultTemplateAsync(template.SiteId, template.TemplateType);
            if (defaultTemplate != null && defaultTemplate.Id != templateId)
            {
                defaultTemplate.DefaultTemplate = false;
                await _repository.UpdateAsync(defaultTemplate, Q
                    .CachingRemove(GetListKey(template.SiteId))
                    .CachingRemove(GetEntityKey(defaultTemplate.Id))
                );
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Template.DefaultTemplate), true)
                .Where(nameof(Template.Id), templateId)
                .CachingRemove(GetListKey(template.SiteId))
                .CachingRemove(GetEntityKey(template.Id))
            );
        }

        public async Task DeleteAsync(IPathManager pathManager, Site site, int templateId)
        {
            var template = await GetAsync(templateId);
            var filePath = await pathManager.GetTemplateFilePathAsync(site, template);

            await _repository.DeleteAsync(templateId, Q
                .CachingRemove(GetListKey(template.SiteId))
                .CachingRemove(GetEntityKey(template.Id))
            );
            FileUtils.DeleteFileIfExists(filePath);
        }

        public async Task CreateDefaultTemplateAsync(IPathManager pathManager, Site site, int adminId)
        {
            await InsertAsync(pathManager, site, new Template
            {
                Id = 0,
                SiteId = site.Id,
                TemplateName = "系统首页模板",
                TemplateType = TemplateType.IndexPageTemplate,
                RelatedFileName = "T_系统首页模板.html",
                CreatedFileFullName = "@/index.html",
                CreatedFileExtName = ".html",
                DefaultTemplate = true
            }, string.Empty, adminId);

            await InsertAsync(pathManager, site, new Template
            {
                Id = 0,
                SiteId = site.Id,
                TemplateName = "系统栏目模板",
                TemplateType = TemplateType.ChannelTemplate,
                RelatedFileName = "T_系统栏目模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                DefaultTemplate = true
            }, string.Empty, adminId);

            await InsertAsync(pathManager, site, new Template
            {
                Id = 0,
                SiteId = site.Id,
                TemplateName = "系统内容模板",
                TemplateType = TemplateType.ContentTemplate,
                RelatedFileName = "T_系统内容模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                DefaultTemplate = true
            }, string.Empty, adminId);
        }
    }
}
