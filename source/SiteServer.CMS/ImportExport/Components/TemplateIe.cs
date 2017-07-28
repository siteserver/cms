using System.Collections.Generic;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class TemplateIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _filePath;

		public TemplateIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

		public void ExportTemplates()
		{
			var feed = AtomUtility.GetEmptyFeed();

			var templateInfoArrayList = DataProvider.TemplateDao.GetTemplateInfoArrayListByPublishmentSystemId(_publishmentSystemId);

			foreach (TemplateInfo templateInfo in templateInfoArrayList)
			{
				var entry = ExportTemplateInfo(templateInfo);
				feed.Entries.Add(entry);
			}
			feed.Save(_filePath);
		}

        public void ExportTemplates(List<int> templateIdList)
        {
            var feed = AtomUtility.GetEmptyFeed();

            var templateInfoArrayList = DataProvider.TemplateDao.GetTemplateInfoArrayListByPublishmentSystemId(_publishmentSystemId);

            foreach (TemplateInfo templateInfo in templateInfoArrayList)
            {
                if (templateIdList.Contains(templateInfo.TemplateId))
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

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(_publishmentSystemId);

			AtomUtility.AddDcElement(entry.AdditionalElements, "TemplateID", templateInfo.TemplateId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", templateInfo.PublishmentSystemId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "TemplateName", templateInfo.TemplateName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "TemplateType", ETemplateTypeUtils.GetValue(templateInfo.TemplateType));
            AtomUtility.AddDcElement(entry.AdditionalElements, "RelatedFileName", templateInfo.RelatedFileName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "CreatedFileFullName", templateInfo.CreatedFileFullName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "CreatedFileExtName", templateInfo.CreatedFileExtName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "Charset", ECharsetUtils.GetValue(templateInfo.Charset));
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsDefault", templateInfo.IsDefault.ToString());

            var templateContent = StlCacheManager.FileContent.GetTemplateContent(publishmentSystemInfo, templateInfo);
			AtomUtility.AddDcElement(entry.AdditionalElements, "Content", AtomUtility.Encrypt(templateContent));

			return entry;
		}

		public void ImportTemplates(bool overwrite, string administratorName)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			var fso = new FileSystemObject(_publishmentSystemId);
			foreach (AtomEntry entry in feed.Entries)
			{
				var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TemplateName");
				if (!string.IsNullOrEmpty(templateName))
				{
				    var templateInfo = new TemplateInfo
				    {
				        PublishmentSystemId = _publishmentSystemId,
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
					
					var srcTemplateInfo = TemplateManager.GetTemplateInfoByTemplateName(_publishmentSystemId, templateInfo.TemplateType, templateInfo.TemplateName);

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
							DataProvider.TemplateDao.Update(fso.PublishmentSystemInfo, srcTemplateInfo, templateContent, administratorName);
							templateId = srcTemplateInfo.TemplateId;
						}
						else
						{
							templateInfo.TemplateName = DataProvider.TemplateDao.GetImportTemplateName(_publishmentSystemId, templateInfo.TemplateName);
							templateId = DataProvider.TemplateDao.Insert(templateInfo, templateContent, administratorName);
						}
					}
					else
					{
						templateId = DataProvider.TemplateDao.Insert(templateInfo, templateContent, administratorName);
					}

					if (templateInfo.TemplateType == ETemplateType.FileTemplate)
					{
						fso.CreateFile(templateId);
					}
				}
			}
		}

	}
}
