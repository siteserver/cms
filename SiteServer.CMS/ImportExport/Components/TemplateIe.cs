using System.Collections.Generic;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
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

			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(TemplateInfo.Id), "TemplateID" }, templateInfo.Id.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TemplateInfo.SiteId), "PublishmentSystemID" }, templateInfo.SiteId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.TemplateName), templateInfo.TemplateName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.TemplateType), templateInfo.TemplateType.Value);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName), templateInfo.RelatedFileName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName), templateInfo.CreatedFileFullName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName), templateInfo.CreatedFileExtName);
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.Charset), ECharsetUtils.GetValue(templateInfo.Charset));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.IsDefault), templateInfo.IsDefault.ToString());

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
			        TemplateType =
			            TemplateTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.TemplateType))),
			        RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName)),
			        CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName)),
			        CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName)),
			        Charset = ECharsetUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.Charset))),
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

			    if (templateInfo.TemplateType == TemplateType.FileTemplate)
			    {
			        CreateManager.CreateFile(_siteId, templateId);
			    }
			}
		}

	}
}
