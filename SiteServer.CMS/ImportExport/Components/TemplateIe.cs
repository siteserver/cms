using System.Collections.Generic;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
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

			var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListBySiteId(_siteId);

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

            var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListBySiteId(_siteId);

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

			AtomUtility.AddDcElement(entry.AdditionalElements, "Id", templateInfo.Id.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "SiteId", templateInfo.SiteId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "TemplateName", templateInfo.TemplateName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "TemplateType", ETemplateTypeUtils.GetValue(templateInfo.TemplateType));
            AtomUtility.AddDcElement(entry.AdditionalElements, "RelatedFileName", templateInfo.RelatedFileName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "CreatedFileFullName", templateInfo.CreatedFileFullName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "CreatedFileExtName", templateInfo.CreatedFileExtName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "Charset", ECharsetUtils.GetValue(templateInfo.Charset));
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsDefault", templateInfo.IsDefault.ToString());

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
				var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TemplateName");
			    if (string.IsNullOrEmpty(templateName)) continue;

			    var templateInfo = new TemplateInfo
			    {
                    SiteId = _siteId,
			        TemplateName = templateName,
			        TemplateType =
			            ETemplateTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, "TemplateType")),
			        RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "RelatedFileName"),
			        CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "CreatedFileFullName"),
			        CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "CreatedFileExtName"),
			        Charset = ECharsetUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Charset")),
			        IsDefault = false
			    };

			    var templateContent = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Content"));
					
			    var srcTemplateInfo = TemplateManager.GetTemplateInfoByTemplateName(_siteId, templateInfo.TemplateType, templateInfo.TemplateName);

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
			            DataProvider.TemplateDao.Update(siteInfo, srcTemplateInfo, templateContent, administratorName);
			            templateId = srcTemplateInfo.Id;
			        }
			        else
			        {
			            templateInfo.TemplateName = DataProvider.TemplateDao.GetImportTemplateName(_siteId, templateInfo.TemplateName);
			            templateId = DataProvider.TemplateDao.Insert(templateInfo, templateContent, administratorName);
			        }
			    }
			    else
			    {
			        templateId = DataProvider.TemplateDao.Insert(templateInfo, templateContent, administratorName);
			    }

			    if (templateInfo.TemplateType == ETemplateType.FileTemplate)
			    {
			        CreateManager.CreateFile(_siteId, templateId);
			    }
			}
		}

	}
}
