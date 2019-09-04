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

            var templateInfoList = await _templateRepository.GetTemplateInfoListBySiteIdAsync(_siteId);

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

            var templateInfoList = await _templateRepository.GetTemplateInfoListBySiteIdAsync(_siteId);

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

        private async Task<AtomEntry> ExportTemplateInfoAsync(Template templateInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            var siteInfo = await _siteRepository.GetSiteAsync(_siteId);

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Template.Id), "TemplateID" }, templateInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Template.SiteId), "PublishmentSystemID" }, templateInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.TemplateName), templateInfo.TemplateName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.Type), templateInfo.Type.Value);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.RelatedFileName), templateInfo.RelatedFileName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.CreatedFileFullName), templateInfo.CreatedFileFullName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.CreatedFileExtName), templateInfo.CreatedFileExtName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Template.IsDefault), templateInfo.IsDefault.ToString());

            var templateContent = await _templateRepository.GetTemplateContentAsync(siteInfo, templateInfo);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Content", AtomUtility.Encrypt(templateContent));

            return entry;
        }

        public async Task ImportTemplatesAsync(bool overwrite, int userId)
        {
            if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var siteInfo = await _siteRepository.GetSiteAsync(_siteId);
            foreach (AtomEntry entry in feed.Entries)
            {
                var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.TemplateName));
                if (string.IsNullOrEmpty(templateName)) continue;

                var templateInfo = new Template
                {
                    SiteId = _siteId,
                    TemplateName = templateName,
                    Type =
                        TemplateType.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.Type))),
                    RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.RelatedFileName)),
                    CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.CreatedFileFullName)),
                    CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Template.CreatedFileExtName)),
                    IsDefault = false
                };

                var templateContent = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Content"));

                var srcTemplateInfo = await _templateRepository.GetTemplateInfoByTemplateNameAsync(_siteId, templateInfo.Type, templateInfo.TemplateName);

                int templateId;

                if (srcTemplateInfo != null)
                {
                    if (overwrite)
                    {
                        srcTemplateInfo.RelatedFileName = templateInfo.RelatedFileName;
                        srcTemplateInfo.Type = templateInfo.Type;
                        srcTemplateInfo.CreatedFileFullName = templateInfo.CreatedFileFullName;
                        srcTemplateInfo.CreatedFileExtName = templateInfo.CreatedFileExtName;
                        await _templateRepository.UpdateAsync(siteInfo, srcTemplateInfo, templateContent, userId);
                        templateId = srcTemplateInfo.Id;
                    }
                    else
                    {
                        templateInfo.TemplateName = await _templateRepository.GetImportTemplateNameAsync(_siteId, templateInfo.TemplateName);
                        templateId = await _templateRepository.InsertAsync(templateInfo, templateContent, userId);
                    }
                }
                else
                {
                    templateId = await _templateRepository.InsertAsync(templateInfo, templateContent, userId);
                }

                if (templateInfo.Type == TemplateType.FileTemplate)
                {
                    await _createManager.AddCreateFileTaskAsync(_siteId, templateId);
                }
            }
        }

    }
}
