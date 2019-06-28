using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;

namespace SS.CMS.Core.Serialization.Components
{
    internal class TemplateIe
    {
        private readonly int _siteId;
        private readonly string _filePath;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplateIe(int siteId, string filePath)
        {
            _siteId = siteId;
            _filePath = filePath;
        }

        public async Task ExportTemplatesAsync()
        {
            var feed = AtomUtility.GetEmptyFeed();

            var templateInfoList = _templateRepository.GetTemplateInfoListBySiteId(_siteId);

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

            var templateInfoList = _templateRepository.GetTemplateInfoListBySiteId(_siteId);

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

        private async Task<AtomEntry> ExportTemplateInfoAsync(TemplateInfo templateInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            var siteInfo = _siteRepository.GetSiteInfo(_siteId);

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TemplateInfo.Id), "TemplateID" }, templateInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TemplateInfo.SiteId), "PublishmentSystemID" }, templateInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.TemplateName), templateInfo.TemplateName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.Type), templateInfo.Type.Value);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName), templateInfo.RelatedFileName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName), templateInfo.CreatedFileFullName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName), templateInfo.CreatedFileExtName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.IsDefault), templateInfo.IsDefault.ToString());

            var templateContent = await _templateRepository.GetTemplateContentAsync(siteInfo, templateInfo);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Content", AtomUtility.Encrypt(templateContent));

            return entry;
        }

        public async Task ImportTemplatesAsync(bool overwrite, string administratorName)
        {
            if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var siteInfo = _siteRepository.GetSiteInfo(_siteId);
            foreach (AtomEntry entry in feed.Entries)
            {
                var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.TemplateName));
                if (string.IsNullOrEmpty(templateName)) continue;

                var templateInfo = new TemplateInfo
                {
                    SiteId = _siteId,
                    TemplateName = templateName,
                    Type =
                        TemplateType.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.Type))),
                    RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName)),
                    CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName)),
                    CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName)),
                    IsDefault = false
                };

                var templateContent = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Content"));

                var srcTemplateInfo = _templateRepository.GetTemplateInfoByTemplateName(_siteId, templateInfo.Type, templateInfo.TemplateName);

                int templateId;

                if (srcTemplateInfo != null)
                {
                    if (overwrite)
                    {
                        srcTemplateInfo.RelatedFileName = templateInfo.RelatedFileName;
                        srcTemplateInfo.Type = templateInfo.Type;
                        srcTemplateInfo.CreatedFileFullName = templateInfo.CreatedFileFullName;
                        srcTemplateInfo.CreatedFileExtName = templateInfo.CreatedFileExtName;
                        _templateRepository.Update(siteInfo, srcTemplateInfo, templateContent, administratorName);
                        templateId = srcTemplateInfo.Id;
                    }
                    else
                    {
                        templateInfo.TemplateName = _templateRepository.GetImportTemplateName(_siteId, templateInfo.TemplateName);
                        templateId = _templateRepository.Insert(templateInfo, templateContent, administratorName);
                    }
                }
                else
                {
                    templateId = _templateRepository.Insert(templateInfo, templateContent, administratorName);
                }

                if (templateInfo.Type == TemplateType.FileTemplate)
                {
                    await _createManager.AddCreateFileTaskAsync(_siteId, templateId);
                }
            }
        }

    }
}
