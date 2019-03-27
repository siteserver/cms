using System.Collections.Generic;
using Atom.Core;
using SiteServer.CMS.Caches;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

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

		public void ExportTemplates()
		{
			var feed = AtomUtility.GetEmptyFeed();

			var templateInfoList = DataProvider.Template.GetTemplateInfoListBySiteId(_siteId);

			foreach (var templateInfo in templateInfoList)
			{
				var entry = ExportTemplateInfo(templateInfo);
				feed.Entries.Add(entry);
			}
			feed.Save(_filePath);
		}

        public void ExportTemplates(List<int> templateIdList)
        {
            var feed = AtomUtility.GetEmptyFeed();

            var templateInfoList = DataProvider.Template.GetTemplateInfoListBySiteId(_siteId);

            foreach (var templateInfo in templateInfoList)
            {
                if (templateIdList.Contains(templateInfo.Id))
                {
                    var entry = ExportTemplateInfo(templateInfo);
                    feed.Entries.Add(entry);
                }
            }
            feed.Save(_filePath);
        }

		private AtomEntry ExportTemplateInfo(TemplateInfo templateInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

            var siteInfo = SiteManager.GetSiteInfo(_siteId);

			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(TemplateInfo.Id), "TemplateID" }, templateInfo.Id.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TemplateInfo.SiteId), "PublishmentSystemID" }, templateInfo.SiteId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.TemplateName), templateInfo.TemplateName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.Type), templateInfo.Type.Value);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName), templateInfo.RelatedFileName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName), templateInfo.CreatedFileFullName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName), templateInfo.CreatedFileExtName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.Default), templateInfo.Default.ToString());

            var templateContent = TemplateManager.GetTemplateContent(siteInfo, templateInfo);
			AtomUtility.AddDcElement(entry.AdditionalElements, "Content", AtomUtility.Encrypt(templateContent));

			return entry;
		}

		public void ImportTemplates(bool overwrite, string administratorName)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

		    var siteInfo = SiteManager.GetSiteInfo(_siteId);
			foreach (AtomEntry entry in feed.Entries)
			{
				var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.TemplateName));
			    if (string.IsNullOrEmpty(templateName)) continue;

			    var templateInfo = new TemplateInfo
			    {
                    SiteId = _siteId,
			        TemplateName = templateName,
			        Type =
			            TemplateTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.Type))),
			        RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName)),
			        CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName)),
			        CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName)),
			        Default = false
			    };

			    var templateContent = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Content"));
					
			    var srcTemplateInfo = TemplateManager.GetTemplateInfoByTemplateName(_siteId, templateInfo.Type, templateInfo.TemplateName);

			    int templateId;

			    if (srcTemplateInfo != null)
			    {
			        if (overwrite)
			        {
			            srcTemplateInfo.RelatedFileName = templateInfo.RelatedFileName;
			            srcTemplateInfo.Type = templateInfo.Type;
			            srcTemplateInfo.CreatedFileFullName = templateInfo.CreatedFileFullName;
			            srcTemplateInfo.CreatedFileExtName = templateInfo.CreatedFileExtName;
			            DataProvider.Template.Update(siteInfo, srcTemplateInfo, templateContent, administratorName);
			            templateId = srcTemplateInfo.Id;
			        }
			        else
			        {
			            templateInfo.TemplateName = DataProvider.Template.GetImportTemplateName(_siteId, templateInfo.TemplateName);
			            templateId = DataProvider.Template.Insert(templateInfo, templateContent, administratorName);
			        }
			    }
			    else
			    {
			        templateId = DataProvider.Template.Insert(templateInfo, templateContent, administratorName);
			    }

			    if (templateInfo.Type == TemplateType.FileTemplate)
			    {
			        CreateManager.CreateFile(_siteId, templateId);
			    }
			}
		}

	}
}
