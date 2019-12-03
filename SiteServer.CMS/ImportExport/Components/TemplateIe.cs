using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.ImportExport.Components
{
	internal class TemplateIe
	{
		private readonly int _siteId;
		private readonly string _filePath;

		public TemplateIe(int siteId, string filePath)
		{
			_siteId = siteId;
			_filePath = filePath;
		}

		public async Task ExportTemplatesAsync()
		{
			var feed = AtomUtility.GetEmptyFeed();

			var templateInfoList = await DataProvider.TemplateRepository.GetTemplateListBySiteIdAsync(_siteId);

			foreach (var templateInfo in templateInfoList)
			{
				var entry = await ExportTemplateInfoAsync(templateInfo);
				feed.Entries.Add(entry);
			}
			feed.Save(_filePath);
		}

        public async Task ExportTemplatesAsync(List<int> templateIdList)
        {
            var feed = AtomUtility.GetEmptyFeed();

            var templateInfoList = await DataProvider.TemplateRepository.GetTemplateListBySiteIdAsync(_siteId);

            foreach (var templateInfo in templateInfoList)
            {
                if (templateIdList.Contains(templateInfo.Id))
                {
                    var entry = await ExportTemplateInfoAsync(templateInfo);
                    feed.Entries.Add(entry);
                }
            }
            feed.Save(_filePath);
        }

		private async Task<AtomEntry> ExportTemplateInfoAsync(Template template)
		{
			var entry = AtomUtility.GetEmptyEntry();

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);

			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(Template.Id), "TemplateID" }, template.Id.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Template.SiteId), "PublishmentSystemID" }, template.SiteId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.TemplateName), template.TemplateName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.TemplateType), template.Type.Value);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.RelatedFileName), template.RelatedFileName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.CreatedFileFullName), template.CreatedFileFullName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.CreatedFileExtName), template.CreatedFileExtName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.Charset), ECharsetUtils.GetValue(template.CharsetType));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.Default), template.Default.ToString());

            var templateContent = TemplateManager.GetTemplateContent(site, template);
			AtomUtility.AddDcElement(entry.AdditionalElements, "Body", AtomUtility.Encrypt(templateContent));

			return entry;
		}

		public async Task ImportTemplatesAsync(bool overwrite, string administratorName)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

		    var site = await DataProvider.SiteRepository.GetAsync(_siteId);
			foreach (AtomEntry entry in feed.Entries)
			{
				var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.TemplateName));
			    if (string.IsNullOrEmpty(templateName)) continue;

			    var templateInfo = new Template
			    {
                    SiteId = _siteId,
			        TemplateName = templateName,
                    Type =
			            TemplateTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.TemplateType))),
			        RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.RelatedFileName)),
			        CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.CreatedFileFullName)),
			        CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.CreatedFileExtName)),
                    CharsetType = ECharsetUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.Charset))),
			        Default = false
			    };

			    var templateContent = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Body"));
					
			    var srcTemplateInfo = await TemplateManager.GetTemplateByTemplateNameAsync(_siteId, templateInfo.Type, templateInfo.TemplateName);

			    int templateId;

			    if (srcTemplateInfo != null)
			    {
			        if (overwrite)
			        {
			            srcTemplateInfo.RelatedFileName = templateInfo.RelatedFileName;
			            srcTemplateInfo.TemplateType = templateInfo.TemplateType;
			            srcTemplateInfo.CreatedFileFullName = templateInfo.CreatedFileFullName;
			            srcTemplateInfo.CreatedFileExtName = templateInfo.CreatedFileExtName;
			            srcTemplateInfo.Charset = templateInfo.Charset;
			            await DataProvider.TemplateRepository.UpdateAsync(site, srcTemplateInfo, templateContent, administratorName);
			            templateId = srcTemplateInfo.Id;
			        }
			        else
			        {
			            templateInfo.TemplateName = await DataProvider.TemplateRepository.GetImportTemplateNameAsync(_siteId, templateInfo.TemplateName);
			            templateId = await DataProvider.TemplateRepository.InsertAsync(templateInfo, templateContent, administratorName);
			        }
			    }
			    else
			    {
			        templateId = await DataProvider.TemplateRepository.InsertAsync(templateInfo, templateContent, administratorName);
			    }

			    if (templateInfo.Type == TemplateType.FileTemplate)
			    {
			        await CreateManager.CreateFileAsync(_siteId, templateId);
			    }
			}
		}

	}
}
