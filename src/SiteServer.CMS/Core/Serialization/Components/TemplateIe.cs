using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.Serialization.Atom.Atom.Core;

namespace SiteServer.CMS.Serialization.Components
{
	internal class TemplateIe
	{
		private readonly Site _site;
		private readonly string _filePath;

		public TemplateIe(Site site, string filePath)
		{
            _site = site;
			_filePath = filePath;
		}

		public async Task ExportTemplatesAsync()
		{
			var feed = AtomUtility.GetEmptyFeed();

            var summaries = await DataProvider.TemplateRepository.GetSummariesAsync(_site.Id);
            foreach (var summary in summaries)
            {
                var template = await DataProvider.TemplateRepository.GetAsync(summary.Id);
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
				var template = await DataProvider.TemplateRepository.GetAsync(templateId);
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
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.Default), template.Default.ToString());

            var templateContent = await DataProvider.TemplateRepository.GetTemplateContentAsync(_site, template);
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
                    Default = false,
                    Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.Content)))
                };

                var exists =
                    await DataProvider.TemplateRepository.ExistsAsync(_site.Id, template.TemplateType,
                        templateName);

			    if (exists)
			    {
			        if (overwrite)
			        {
                        var info = await DataProvider.TemplateRepository.GetTemplateByTemplateNameAsync(_site.Id, template.TemplateType, template.TemplateName);

						info.RelatedFileName = template.RelatedFileName;
			            info.TemplateType = template.TemplateType;
			            info.CreatedFileFullName = template.CreatedFileFullName;
			            info.CreatedFileExtName = template.CreatedFileExtName;
                        info.Content = template.Content;

                        templates.Add(info);
                    }
			        else
			        {
			            template.TemplateName = await DataProvider.TemplateRepository.GetImportTemplateNameAsync(_site.Id, template.TemplateType, template.TemplateName);
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
                Caching.SetProcess(guid, $"导入模板文件: {template.TemplateName}");

                if (template.Id > 0)
                {
                    await DataProvider.TemplateRepository.UpdateAsync(_site, template, template.Content, adminId);
				}
                else
                {
                    await DataProvider.TemplateRepository.InsertAsync(_site, template, template.Content, adminId);
                }
            }
		}

	}
}
