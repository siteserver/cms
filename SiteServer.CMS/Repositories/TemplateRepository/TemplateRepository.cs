using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public class TemplateRepository : DataProviderBase, IRepository
    {
        private readonly Repository<Template> _repository;

        public TemplateRepository()
        {
            _repository = new Repository<Template>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public static class Attr
        {
            public const string IsDefault = nameof(IsDefault);
        }

        public async Task<int> InsertAsync(Template template, string templateContent, string administratorName)
        {
            if (template.Default)
            {
                await SetAllTemplateDefaultToFalseAsync(template.SiteId, template.TemplateType);
            }

            template.Id = await _repository.InsertAsync(template);

            var site = await DataProvider.SiteRepository.GetAsync(template.SiteId);
            await TemplateManager.WriteContentToTemplateFileAsync(site, template, templateContent, administratorName);

            TemplateManager.RemoveCache(template.SiteId);

            return template.Id;
        }

        public async Task UpdateAsync(Site site, Template template, string templateContent, string administratorName)
        {
            if (template.Default)
            {
                await SetAllTemplateDefaultToFalseAsync(site.Id, template.TemplateType);
            }

            await _repository.UpdateAsync(template);

            await TemplateManager.WriteContentToTemplateFileAsync(site, template, templateContent, administratorName);

            TemplateManager.RemoveCache(template.SiteId);
        }

        private async Task SetAllTemplateDefaultToFalseAsync(int siteId, TemplateType templateType)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.IsDefault, false.ToString())
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
            );
        }

        public async Task SetDefaultAsync(int siteId, int templateId)
        {
            var template = await TemplateManager.GetTemplateAsync(siteId, templateId);
            await SetAllTemplateDefaultToFalseAsync(template.SiteId, template.TemplateType);

            await _repository.UpdateAsync(Q
                .Set(Attr.IsDefault, true.ToString())
                .Where(nameof(Template.Id), templateId)
            );

            TemplateManager.RemoveCache(siteId);
        }

        public async Task DeleteAsync(int siteId, int id)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var template = await TemplateManager.GetTemplateAsync(siteId, id);
            var filePath = TemplateManager.GetTemplateFilePath(site, template);

            await _repository.DeleteAsync(id);
            FileUtils.DeleteFileIfExists(filePath);

            TemplateManager.RemoveCache(siteId);
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

        public async Task<Dictionary<TemplateType, int>> GetCountDictionaryAsync(int siteId)
        {
            var dictionary = new Dictionary<TemplateType, int>();

            var dataList = await _repository.GetAllAsync<(string Type, int Count)>(Q
                .Select(nameof(Template.TemplateType))
                .SelectRaw("COUNT(*) as Count")
                .Where(nameof(Template.SiteId), siteId)
                .GroupBy(nameof(Template.TemplateType)));

            foreach (var (type, count) in dataList)
            {
                var templateType = TranslateUtils.ToEnum(type, TemplateType.IndexPageTemplate);

                if (dictionary.ContainsKey(templateType))
                {
                    dictionary[templateType] += count;
                }
                else
                {
                    dictionary[templateType] = count;
                }
            }

            return dictionary;
        }

        public async Task<IEnumerable<Template>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .OrderBy(nameof(Template.TemplateType), nameof(Template.RelatedFileName))
            );
        }

        public async Task<List<int>> GetIdListByTypeAsync(int siteId, TemplateType templateType)
        {
            var list = await GetTemplateListByTypeAsync(siteId, templateType);
            return list.Select(x => x.Id).ToList();
        }

        public async Task<List<Template>> GetTemplateListByTypeAsync(int siteId, TemplateType templateType)
        {
            var templateEntityList = await _repository.GetAllAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
                .OrderBy(nameof(Template.RelatedFileName))
            );

            return templateEntityList.ToList();
        }

        public async Task<List<Template>> GetTemplateListBySiteIdAsync(int siteId)
        {
            var templateEntityList = await _repository.GetAllAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .OrderBy(nameof(Template.TemplateType), nameof(Template.RelatedFileName))
            );

            return templateEntityList.ToList();
        }

        public async Task<List<string>> GetTemplateNameListAsync(int siteId, TemplateType templateType)
        {
            return (await _repository.GetAllAsync<string>(Q
                .Select(nameof(Template.TemplateName))
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
            )).ToList();
        }

        public async Task<List<string>> GetRelatedFileNameListAsync(int siteId, TemplateType templateType)
        {
            return (await _repository.GetAllAsync<string>(Q
                .Select(nameof(Template.RelatedFileName))
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
            )).ToList();
        }

        public async Task CreateDefaultTemplateAsync(int siteId, string administratorName)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);

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
                await InsertAsync(theTemplate, theTemplate.Content, administratorName);
            }
        }

        public async Task<Dictionary<int, Template>> GetTemplateDictionaryBySiteIdAsync(int siteId)
        {
            var dictionary = new Dictionary<int, Template>();

            var list = await _repository.GetAllAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .OrderBy(nameof(Template.TemplateType), nameof(Template.RelatedFileName))
            );

            foreach (var template in list)
            {
                dictionary[template.Id] = template;
            }

            return dictionary;
        }

        public async Task<Template> GetTemplateByUrlTypeAsync(int siteId, TemplateType templateType, string createdFileFullName)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
                .Where(nameof(Template.CreatedFileFullName), createdFileFullName)
            );
        }

        public async Task<Template> GetAsync(int templateId)
        {
            return await _repository.GetAsync(templateId);
        }
    }
}
