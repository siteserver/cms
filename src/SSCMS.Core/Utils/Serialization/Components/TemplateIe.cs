using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
	internal class TemplateIe
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly Site _site;
		private readonly string _filePath;

		public TemplateIe(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string filePath)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _site = site;
			_filePath = filePath;
		}

		public async Task ExportTemplatesAsync()
		{
			var feed = AtomUtility.GetEmptyFeed();

            var summaries = await _databaseManager.TemplateRepository.GetSummariesAsync(_site.Id);
            foreach (var summary in summaries)
            {
                var template = await _databaseManager.TemplateRepository.GetAsync(summary.Id);
                var entry = await ExportTemplateInfoAsync(template);
                feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

        public async Task ExportTemplatesAsync(List<int> templateIdList)
        {
            var feed = AtomUtility.GetEmptyFeed();

            foreach (var templateId in templateIdList)
            {
				var template = await _databaseManager.TemplateRepository.GetAsync(templateId);
                var entry = await ExportTemplateInfoAsync(template);
                feed.Entries.Add(entry);
			}
            feed.Save(_filePath);
        }

		private async Task<AtomEntry> ExportTemplateInfoAsync(Template template)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(Template.Id), "TemplateID" }, template.Id.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Template.SiteId), "PublishmentSystemID" }, template.SiteId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.TemplateName), template.TemplateName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.TemplateType), template.TemplateType.GetValue());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.RelatedFileName), template.RelatedFileName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.CreatedFileFullName), template.CreatedFileFullName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.CreatedFileExtName), template.CreatedFileExtName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Charset", "utf-8");
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.DefaultTemplate), template.DefaultTemplate.ToString());

            var templateContent = await _pathManager.GetTemplateContentAsync(_site, template);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.Content), AtomUtility.Encrypt(templateContent));

			return entry;
		}

		public async Task ImportTemplatesAsync(bool overwrite, int adminId, string guid)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var templates = new List<Template>();

			foreach (AtomEntry entry in feed.Entries)
			{
				var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.TemplateName));
			    if (string.IsNullOrEmpty(templateName)) continue;

                var template = new Template
                {
                    SiteId = _site.Id,
                    TemplateName = templateName,
                    TemplateType =
                        TranslateUtils.ToEnum(
                            AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.TemplateType)),
                            TemplateType.IndexPageTemplate),
                    RelatedFileName =
                        AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.RelatedFileName)),
                    CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        nameof(Template.CreatedFileFullName)),
                    CreatedFileExtName =
                        AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.CreatedFileExtName)),
                    DefaultTemplate = false,
                    Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.Content)))
                };

                var exists =
                    await _databaseManager.TemplateRepository.ExistsAsync(_site.Id, template.TemplateType,
                        templateName);

			    if (exists)
			    {
			        if (overwrite)
			        {
                        var info = await _databaseManager.TemplateRepository.GetTemplateByTemplateNameAsync(_site.Id, template.TemplateType, template.TemplateName);

						info.RelatedFileName = template.RelatedFileName;
			            info.TemplateType = template.TemplateType;
			            info.CreatedFileFullName = template.CreatedFileFullName;
			            info.CreatedFileExtName = template.CreatedFileExtName;
                        info.Content = template.Content;

                        templates.Add(info);
                    }
			        else
			        {
			            template.TemplateName = await _databaseManager.TemplateRepository.GetImportTemplateNameAsync(_site.Id, template.TemplateType, template.TemplateName);
                        templates.Add(template);
                    }
			    }
			    else
			    {
                    templates.Add(template);
			    }
			}

            foreach (var template in templates)
            {
                _caching.SetProcess(guid, $"导入模板文件: {template.TemplateName}");

                if (template.Id > 0)
                {
                    await _databaseManager.TemplateRepository.UpdateAsync(template);
				}
                else
                {
                    template.Id = await _databaseManager.TemplateRepository.InsertAsync(template);
                }

                await _pathManager.WriteContentToTemplateFileAsync(_site, template, template.Content, adminId);
            }
		}

	}
}
